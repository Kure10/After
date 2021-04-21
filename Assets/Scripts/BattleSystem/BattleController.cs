using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.Events;

public class BattleController : MonoBehaviour
{
    private BattleStartData battleData = new BattleStartData();

    [Header("Info Panels")]
    [SerializeField] private BattleInfoPanel battleInfoPanel;
    [SerializeField] private UnitInfoPanel leftUnitInfo;
    [SerializeField] private UnitInfoPanel rightUnitInfo;
    [SerializeField] private BattleLogPanel battleLog;

    [Header("Dimensions")]
    [SerializeField] List<GameObject> rows = new List<GameObject>();
    [Space]
    [SerializeField] int collumCount = 16;

    [Space]
    [Header("Others")]
    private Squar[,] _squaresInBattleField; 

    public List<Unit> unitsOnBattleField = new List<Unit>();



    [Space]
    public GameObject _unit;

    public GameObject squarTemplate;

    //

    /// <summary>
    /// ////////////////////
    ///   FOR now needed we will see 
    ///    musim v tom udelat poradek neco budu potrebovat neco ne
    /// </summary>
    /// 

    bool _turnIsOver = false;
    private bool _isBattleOnline = false;
    private bool _isPlayerTurn = false;

    private Unit _activeUnit = null;

    private int order = 0;

    private int roundCount = 1;

    private List<Squar> _squaresInUnitRange = new List<Squar>();

    private List<Squar> _squaresInUnitAttackRange = new List<Squar>();

    private void Start()
    {
        CreateBattleField();
        InitBattle();
        TestStartBattle();
    }

    private void Update()
    {
        if (_isBattleOnline)
        {
            if (_isPlayerTurn)
            {
                if (DetectPlayerInputs())
                {
                    _turnIsOver = true;
                    _isPlayerTurn = false;
                }
            }
            else
            {
                // OnSimpleAIMove();

                if (DetectPlayerInputs())
                {
                    _turnIsOver = true;
                    _isPlayerTurn = true;
                }

                // AI is on the move 
                // start corotine
                // switch to player turn
            }

            if (_turnIsOver)
            {
                SetSquaresOutOfAttackReach();
                SetSquaresUnvisited();
                ShowSquaresWithinRange(false);

                battleLog.AddLog("New Turn : " + ++roundCount);

                order++;

                if (order >= unitsOnBattleField.Count)
                    order = 0;

                UpdateActiveUnit();

                FindSquaresInUnitMoveRange();
                ShowSquaresWithinRange(true);


                FindSquaresInUnitAttackRange();
                ShowSquaresWithingAttackRange();

                leftUnitInfo.UpdateStats(_activeUnit);

                _turnIsOver = false;

            }
        }
    }

    public bool DetectPlayerInputs()
    {
        bool result = false;
        BattleAction action = BattleAction.None;
        Squar squarToMove = null;
        Unit unitOnSquare = null;

        if (Input.GetMouseButtonDown(0))
        {
            squarToMove = RaycastTargetSquar();
            if (squarToMove != null)
            {
                action = OnClickIntoGrid(squarToMove);
                unitOnSquare = squarToMove.unitInSquar;
                result = true;
            }
        }

        if(result)
        {
            switch (action)
            {
                case BattleAction.None:
                    Debug.Log("None Action");
                    result = false;
                    break;
                case BattleAction.Move:
                    MoveToSquar(squarToMove);
                    battleLog.AddLog($"{_activeUnit._name} moved to square {squarToMove.xCoordinate} / {squarToMove.yCoordinate}");
                    result = true;
                    break;
                case BattleAction.Attack:
                    AttackInfo attackInfo = null;
                    attackInfo = AttackToUnit(unitOnSquare);
                    battleLog.AddLog($"{_activeUnit._name} attacked to unit {unitOnSquare._name} with {attackInfo.dices} dices and {attackInfo.success} damage");
                    result = true;
                    result = true;
                    break;
                case BattleAction.Heal:
                    Debug.Log("Heal action");
                    result = false;
                    break;
                default:
                    break;
            }

            if (action == BattleAction.Move || action == BattleAction.Attack)
            {

            }
        }

        return result;
    }

    // for buttons
    public void SkipUnitTurn()
    {
        _turnIsOver = true;
    }

    public void CreateBattleField()
    {

        _squaresInBattleField = new Squar[rows.Count, collumCount];

        int j = 0;
        foreach (GameObject row in rows)
        {
            for (int i = 0; i < collumCount; i++)
            {
                GameObject squarGameObject = Instantiate(squarTemplate, row.transform);
                Squar square = squarGameObject.GetComponent<Squar>();
                square.SetCoordinates(j, i);
                
                _squaresInBattleField[j, i] = square;

                square.InitEvent(delegate (Squar squ) 
                {
                    SetCursor(squ);
                    UpdateRightPanel(squ);
                });
            }
            j++;
        }

    }

    public void InitBattle()
    {
        battleLog.AddLog("Battle Start");
        // something is for testing
        _isPlayerTurn = true;

        battleData = InitTestBattleData();
        order = 0;

        int amountEnemies = battleData.aiData.enemieUnits.Count;

        int amountPlayers = battleData.playerData.playerUnits.Count;

        int uniqNumber = 0;

        for (int i = 0; i < amountEnemies; i++)
        {
            DataUnit dataUnit = battleData.aiData.enemieUnits[i];
            Squar squar = GetSquareFromGrid(dataUnit.StartPos.XPosition, dataUnit.StartPos.YPosition);

            if (squar == null)
            {
                Debug.LogError("critical Error while inicialization Battle");
            }

            GameObject unt = Instantiate(_unit, squar.container.transform);
            Unit newUnit = unt.GetComponent<Unit>();

            newUnit.InitUnit(dataUnit._name, dataUnit.health, dataUnit.damage, dataUnit.threat, dataUnit.range, dataUnit.StartPos, uniqNumber, dataUnit._movement,dataUnit.imageName,Unit.Team.Demon);
            squar.unitInSquar = newUnit;
            unitsOnBattleField.Add(newUnit);
            uniqNumber++;
        }

        for (int i = 0; i < amountPlayers; i++)
        {
            DataUnit dataUnit = battleData.playerData.playerUnits[i];
            Squar squar = GetSquareFromGrid(dataUnit.StartPos.XPosition, dataUnit.StartPos.YPosition);

            if (squar == null)
            {
                Debug.LogError("critical Error while inicialization Battle");
            }

            GameObject unit1 = Instantiate(_unit, squar.container.transform);
            Unit newUnit = unit1.GetComponent<Unit>();

            newUnit.InitUnit(dataUnit._name, dataUnit.health, dataUnit.damage, dataUnit.threat, dataUnit.range, dataUnit.StartPos, uniqNumber, dataUnit._movement,dataUnit.imageName,Unit.Team.Human);

            squar.unitInSquar = newUnit;
            unitsOnBattleField.Add(newUnit);
            uniqNumber++;
        }

        // Sort unit order.
        SortUnitAccordingIniciation();
        battleInfoPanel.InitStartOrder(unitsOnBattleField, _unit);

    }

    public BattleStartData InitTestBattleData()
    {
        BattleStartData battleStartData = new BattleStartData();

        DataUnit newA = new DataUnit();
        DataUnit newB = new DataUnit();
        DataUnit newC = new DataUnit();
        DataUnit newx = new DataUnit();
        DataUnit newy = new DataUnit();
        // newA
        newA.StartPos.XPosition = 6;
        newA.StartPos.YPosition = 13;
        newA._name = "Player1";
        newA.imageName = "Gargoyle";
        newA.health = 14;
        newA.damage = 6;
        newA.threat = 3;
        newA.range = 0;
        newA._movement = 2;



        newB.StartPos.XPosition = 0;
        newB.StartPos.YPosition = 0;
        newB._name = "Zombie1";
        newB.imageName = "Zombie 1";
        newB.health = 9;
        newB.damage = 4;
        newB.threat = 5;
        newB.range = 2;
        newB._movement = 1;

        newC.StartPos.XPosition = 0;
        newC.StartPos.YPosition = 6;
        newC._name = "Zombie2";
        newC.imageName = "Zombie 2";
        newC.health = 8;
        newC.damage = 3;
        newC.threat = 1;
        newC.range = 1;

        newx.StartPos.XPosition = 5;
        newx.StartPos.YPosition = 2;
        newx._name = "Zombie3";
        newx.imageName = "Zombie 1";
        newx.health = 8;
        newx.damage = 3;
        newx.threat = 2;
        newx.range = 0;


        newy.StartPos.XPosition = 4;
        newy.StartPos.YPosition = 6;
        newy._name = "Player2";
        newy.imageName = "Gargoyle";
        newy.health = 11;
        newy.damage = 1;
        newy.threat = 2;
        newy.range = 2;
        newy._movement = 3;

        battleStartData.playerData.playerUnits.Add(newA);
        battleStartData.playerData.playerUnits.Add(newy);
        battleStartData.aiData.enemieUnits.Add(newB);
        battleStartData.aiData.enemieUnits.Add(newC);
        battleStartData.aiData.enemieUnits.Add(newx);

        return battleStartData;
    }

    private void TestStartBattle()
    {
        _isBattleOnline = true;

        if (unitsOnBattleField.Count > 0)
        {
            _activeUnit = unitsOnBattleField[order];
            _activeUnit.IsActive = true;
            _activeUnit.UpdateAnim();
            battleInfoPanel.UpdateUnitOrder(_activeUnit, true);

            //
            FindSquaresInUnitMoveRange();
            ShowSquaresWithinRange(true);

            // testing fire range
            FindSquaresInUnitAttackRange();
            ShowSquaresWithingAttackRange();

            leftUnitInfo.UpdateStats(_activeUnit);
        }
        else
        {
            Debug.LogError("No units");
        }
    }

    private Squar RaycastTargetSquar()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultsList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultsList);

        for (int i = 0; i < raycastResultsList.Count; i++)
        {
            var sq = raycastResultsList[i].gameObject.GetComponent<Squar>();

            if (sq != null)
            {
                return sq;
            }

        }

        return null;
    }

    // this is TMP method
    private Squar FindSquarForAI()
    {
        int rowsCount = rows.Count - 1;

        while (true)
        {
            int xCor = Random.Range(0, rowsCount);
            int yCor = Random.Range(0, collumCount);

            if (_squaresInBattleField[xCor, yCor].unitInSquar == null)
            {
                return _squaresInBattleField[xCor, yCor];
            }
        }
    }


    private BattleAction OnClickIntoGrid(Squar squarToMove)
    {
        BattleAction action = BattleAction.None;

        if (squarToMove.unitInSquar != null)
        {
            bool isFriendlyUnit = squarToMove.unitInSquar._team == _activeUnit._team;

            if(!isFriendlyUnit && _squaresInUnitAttackRange.Contains(squarToMove))
            {
                action = BattleAction.Attack;
            }

            if (isFriendlyUnit && _squaresInUnitAttackRange.Contains(squarToMove))
            {
                action = BattleAction.Heal;
            }

            return action;
        }
        else
        {
            if (_squaresInUnitRange.Contains(squarToMove))
            {
                action = BattleAction.Move;
            }

            return action;
        }
    }
    private void OnSimpleAIMove()
    {
        Squar squarToMove = FindSquarForAI();
        MoveToSquar(squarToMove);
    }

    private void MoveToSquar(Squar squarToMove)
    {
        Squar selectedUnitSquar = GetSquareFromGrid(_activeUnit.CurrentPos.XPosition,_activeUnit.CurrentPos.YPosition);

        selectedUnitSquar.unitInSquar.transform.SetParent(squarToMove.container.transform);
        selectedUnitSquar.unitInSquar.SetNewCurrentPosition(squarToMove.xCoordinate, squarToMove.yCoordinate);
        squarToMove.unitInSquar = selectedUnitSquar.unitInSquar;
        selectedUnitSquar.unitInSquar = null;
    }

    private AttackInfo AttackToUnit(Unit defendUnit)
    {
        AttackInfo attackInfo = new AttackInfo();

        int dices = BattleSystem.CalculateAmountDices(_activeUnit);
        int success = BattleSystem.CalculateAmountSuccess(dices , defendUnit._threat);

        defendUnit.CurrentHealth = defendUnit.CurrentHealth - success;

        var isDead = defendUnit.CheckIfUnitIsNotDead();

        if (isDead)
        {
            KillUnitOnBattleField(defendUnit);
            battleInfoPanel.DeleteUnitFromOrder(defendUnit);
        }

        // for info
        attackInfo.dices = dices;
        attackInfo.success = success;

        return attackInfo;
    }

    private void KillUnitOnBattleField(Unit unit)
    {
        unitsOnBattleField.Remove(unit);

        var sq = GetSquareFromGrid(unit.CurrentPos.XPosition, unit.CurrentPos.YPosition);

        Destroy(sq.unitInSquar.gameObject, 0.5f);

        sq.unitInSquar = null;

    }

    // in this method i can mark squares and maybe change method for move..  Can be more easy to find..
    private void FindSquaresInUnitMoveRange()
    {
        _squaresInUnitRange.Clear();

        int moveRange = _activeUnit._movement;
        Squar centerSquar = _squaresInBattleField[_activeUnit.CurrentPos.XPosition, _activeUnit.CurrentPos.YPosition];

        _squaresInUnitRange.AddRange(GetTheAdjacentSquare(centerSquar));

        List<Squar> adjectedSq = new List<Squar>();
        adjectedSq.AddRange(_squaresInUnitRange);

        for (int i = 1; i < moveRange; i++)
        {
            List<Squar> adjectedSquarsInCurrentRange = new List<Squar>();

            foreach (Squar sq in adjectedSq)
            {
                adjectedSquarsInCurrentRange.AddRange(GetTheAdjacentSquare(sq));
            }

            foreach (Squar sq in adjectedSq)
            {
                sq.isVisited = true;
            }

            adjectedSq.Clear();
            adjectedSq.AddRange(adjectedSquarsInCurrentRange);

            _squaresInUnitRange.AddRange(adjectedSq);
        }

        _squaresInUnitRange.Add(centerSquar);
    }

    private void FindSquaresInUnitAttackRange()
    {
        _squaresInUnitAttackRange.Clear();

        int attackRange = _activeUnit._range;
        Squar centerSquar = _squaresInBattleField[_activeUnit.CurrentPos.XPosition, _activeUnit.CurrentPos.YPosition];

        _squaresInUnitAttackRange.AddRange(GetTheAdjacentAttackSquare(centerSquar));

        //if (attackRange >= 1)
        //{
        //    _squaresInUnitAttackRange.AddRange(GetTheAdjacentCrossSquare(centerSquar));
        //}

        List<Squar> lastAdjectedSq = new List<Squar>();
        lastAdjectedSq.AddRange(_squaresInUnitAttackRange);

        for (int i = 1; i < attackRange; i++)
        {
            List<Squar> adjectedSquarsInCurrentRange = new List<Squar>();

            foreach (Squar sq in lastAdjectedSq)
            {
                adjectedSquarsInCurrentRange.AddRange(GetTheAdjacentAttackSquare(sq));
               // adjectedSquarsInCurrentRange.AddRange(GetTheAdjacentCrossSquare(sq));
            }

            lastAdjectedSq.Clear();
            lastAdjectedSq.AddRange(adjectedSquarsInCurrentRange);

            _squaresInUnitAttackRange.AddRange(lastAdjectedSq);
        }

        _squaresInUnitAttackRange.Add(centerSquar);

        // search and activate borders for attack range
        //foreach (Squar squ in _squaresInUnitAttackRange)
        //{
        //    GetTheAdjacentAttackSquare(squ, true);
        //}
    }

    private void ShowSquaresWithinRange(bool makeVisible)
    {
        foreach (Squar sq in _squaresInUnitRange)
        {
            if (sq.unitInSquar == null)
            {
                sq.inRangeBackground.SetActive(makeVisible);
            }
        }
    }

    private void ShowSquaresWithingAttackRange()
    {
        foreach (Squar squ in _squaresInUnitAttackRange)
        {
            GetTheAdjacentAttackSquare(squ, true);
        }
    }

    public List<Squar> GetTheAdjacentSquare(Squar centerSquar) 
    {
        List<Squar> checkedSquars = new List<Squar>();

        Squar rightSquare = null;
        Squar leftSquare = null;
        Squar upSquare = null;
        Squar downSquare = null;

        // check up direction
        if (centerSquar.xCoordinate + 1 >= rows.Count)
            upSquare = null;
        else
            upSquare = GetSquareFromGrid(centerSquar.xCoordinate + 1, centerSquar.yCoordinate);

        // check down direction
        if (centerSquar.xCoordinate - 1 < 0)
            downSquare = null;
        else
            downSquare = GetSquareFromGrid(centerSquar.xCoordinate - 1, centerSquar.yCoordinate);

        // check right direction
        if (centerSquar.yCoordinate + 1 >= collumCount)
            rightSquare = null;
        else
            rightSquare = GetSquareFromGrid(centerSquar.xCoordinate, centerSquar.yCoordinate + 1);

        // check left direction
        if (centerSquar.yCoordinate - 1 < 0)
            leftSquare = null;
        else
            leftSquare = GetSquareFromGrid(centerSquar.xCoordinate, centerSquar.yCoordinate - 1);

        if (rightSquare != null && !rightSquare.isVisited)
            checkedSquars.Add(rightSquare);
        if (leftSquare != null && !leftSquare.isVisited)
            checkedSquars.Add(leftSquare);
        if (upSquare != null && !upSquare.isVisited)
            checkedSquars.Add(upSquare);
        if (downSquare != null && !downSquare.isVisited)
            checkedSquars.Add(downSquare);

        centerSquar.isVisited = true;

        foreach (Squar sq in checkedSquars)
        {
            sq.isVisited = true;
        }

        return checkedSquars;
    }

    public List<Squar> GetTheAdjacentAttackSquare(Squar centerSquar, bool searchForBorders = false)
    {
        List<Squar> checkedSquars = new List<Squar>();

        Squar rightSquare = null;
        Squar leftSquare = null;
        Squar upSquare = null;
        Squar downSquare = null;

        // check up direction
        if (centerSquar.xCoordinate + 1 >= rows.Count)
            upSquare = null;
        else
            upSquare = GetSquareFromGrid(centerSquar.xCoordinate + 1, centerSquar.yCoordinate);

        // check down direction
        if (centerSquar.xCoordinate - 1 < 0)
            downSquare = null;
        else
            downSquare = GetSquareFromGrid(centerSquar.xCoordinate - 1, centerSquar.yCoordinate);

        // check right direction
        if (centerSquar.yCoordinate + 1 >= collumCount)
            rightSquare = null;
        else
            rightSquare = GetSquareFromGrid(centerSquar.xCoordinate, centerSquar.yCoordinate + 1);

        // check left direction
        if (centerSquar.yCoordinate - 1 < 0)
            leftSquare = null;
        else
            leftSquare = GetSquareFromGrid(centerSquar.xCoordinate, centerSquar.yCoordinate - 1);

        if(searchForBorders)
        {
            if (leftSquare == null || !leftSquare.isInReach)
                centerSquar.leftBorder.SetActive(true);

            if (rightSquare == null || !rightSquare.isInReach)
                centerSquar.rightBorder.SetActive(true);

            if (downSquare == null || !downSquare.isInReach)
                centerSquar.downBorder.SetActive(true);

            if (upSquare == null || !upSquare.isInReach)
                centerSquar.upBorder.SetActive(true);
        }
        else
        {
            if (rightSquare != null && !rightSquare.isInReach)
                checkedSquars.Add(rightSquare);
            if (leftSquare != null && !leftSquare.isInReach)
                checkedSquars.Add(leftSquare);
            if (upSquare != null && !upSquare.isInReach)
                checkedSquars.Add(upSquare);
            if (downSquare != null && !downSquare.isInReach)
                checkedSquars.Add(downSquare);

            centerSquar.isInReach = true;

            foreach (Squar sq in checkedSquars)
            {
                sq.isInReach = true;
            }
        }

        return checkedSquars;
    }

    public List<Squar> GetTheAdjacentCrossSquare(Squar centerSquar)
    {
        List<Squar> checkedSquars = new List<Squar>();
        centerSquar.isInReach = true;

        Squar rightCrossSquare = null;
        Squar leftCrossSquare = null;
        Squar upCrossSquare = null;
        Squar downCrossSquare = null;

        // check upLeft direction
        if (centerSquar.xCoordinate + 1 >= rows.Count || centerSquar.yCoordinate - 1 < 0)
            upCrossSquare = null;
        else
            upCrossSquare = GetSquareFromGrid(centerSquar.xCoordinate + 1, centerSquar.yCoordinate - 1);

        // check downLeft direction
        if (centerSquar.xCoordinate - 1 < 0 || centerSquar.yCoordinate - 1 < 0)
            downCrossSquare = null;
        else
            downCrossSquare = GetSquareFromGrid(centerSquar.xCoordinate - 1, centerSquar.yCoordinate - 1);

        // check upRight direction
        if (centerSquar.xCoordinate + 1 >= rows.Count || centerSquar.yCoordinate + 1 >= collumCount)
            rightCrossSquare = null;
        else
            rightCrossSquare = GetSquareFromGrid(centerSquar.xCoordinate + 1, centerSquar.yCoordinate + 1);

        // check downRight direction
        if (centerSquar.xCoordinate - 1 < 0 || centerSquar.yCoordinate + 1 >= collumCount)
            leftCrossSquare = null;
        else
            leftCrossSquare = GetSquareFromGrid(centerSquar.xCoordinate - 1, centerSquar.yCoordinate + 1);

        if (rightCrossSquare != null && !rightCrossSquare.isInReach)
            checkedSquars.Add(rightCrossSquare);
        if (leftCrossSquare != null && !leftCrossSquare.isInReach)
            checkedSquars.Add(leftCrossSquare);
        if (upCrossSquare != null && !upCrossSquare.isInReach)
            checkedSquars.Add(upCrossSquare);
        if (downCrossSquare != null && !downCrossSquare.isInReach)
            checkedSquars.Add(downCrossSquare);

        foreach (Squar sq in checkedSquars)
        {
            sq.isInReach = true;
        }

        return checkedSquars;
    }

    private Squar GetSquareFromGrid(int x , int y)
    {
        Squar sq = _squaresInBattleField[x, y];
        return sq;
    }

    private void UpdateActiveUnit()
    {
        battleInfoPanel.UpdateUnitOrder(_activeUnit, false);
        _activeUnit.IsActive = false;
        _activeUnit.UpdateAnim();
        _activeUnit = unitsOnBattleField[order];
        _activeUnit.IsActive = true;
        _activeUnit.UpdateAnim();
        battleInfoPanel.UpdateUnitOrder(_activeUnit, true);
    }

    private void SetSquaresUnvisited()
    {
        foreach (Squar item in _squaresInUnitRange)
        {
            item.inRangeBackground.SetActive(false);
            item.isVisited = false;
        }
    }

    private void SetSquaresOutOfAttackReach()
    {
        foreach (Squar item in _squaresInUnitAttackRange)
        {
            item.DisableAttackBorders();
            item.isInReach = false;
        }
    }

    public void SetCursor(Squar sq)
    {
        sq.CursorEvent.canAttack = false;
        sq.CursorEvent.isInMoveRange = false;

        if (sq.unitInSquar is null)
        {
            if (_squaresInUnitRange.Contains(sq))
            {
                sq.CursorEvent.isInMoveRange = true;
            }
        }
        else
        {
            if(_squaresInUnitAttackRange.Contains(sq) && sq.unitInSquar._team != _activeUnit._team)
            {
                sq.CursorEvent.canAttack = true;
            }
        }
        
    }

    private void SortUnitAccordingIniciation()
    {
        unitsOnBattleField.Sort((x, y) => y._iniciation.CompareTo(x._iniciation));
    }

    public void UpdateRightPanel (Squar sq)
    {
        Unit unit = sq.unitInSquar;

        if (unit != null)
        {
            rightUnitInfo.UpdateStats(unit);
            rightUnitInfo.gameObject.SetActive(true);

        }
        //else
        //{
        //   // rightUnitInfo.DisablePanel();
        //}
    }

    enum BattleAction 
    {
        None,
        Move,
        Attack,
        Heal
    }

    private class AttackInfo
    {
        public int dices = 0;
        public int success = 0;
    }



}
