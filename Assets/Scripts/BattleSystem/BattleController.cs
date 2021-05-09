using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.Events;

public class BattleController : MonoBehaviour
{
    [Header("Info Panels")]
    [SerializeField] private BattleInfoPanel battleInfoPanel;
    [SerializeField] private UnitInfoPanel leftUnitInfo;
    [SerializeField] private UnitInfoPanel rightUnitInfo;
    [SerializeField] private BattleLogPanel battleLog;

    [Header("Info Popup")]
    [SerializeField] private DetailUnitPopup detailPopup;

    [Header("Dimensions")]
    [SerializeField] List<GameObject> _rows = new List<GameObject>();
    [Space]
    [SerializeField] int _collumCount = 16;
    [SerializeField] int _rowsCount = 16;

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

    private ResourceSpriteLoader spriteLoader = null;

    private void Awake()
    {
        
    }
    private void Start()
    {
        // This is for testing purpose only when u are in BATTLEGROUND scene!!

        //BattleStartData battleStartData = InitTestBattleData();

        //CreateBattleField(battleStartData.Rows, battleStartData.Collumn);
        //InitBattle(battleStartData);
        //TestStartBattle();
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

                 order++;

                if (order >= unitsOnBattleField.Count)
                {
                    order = 0;
                    roundCount++;
                    battleLog.AddBattleLog($"<---------- Turn {roundCount} ---------->");

                    // nova iniciativa a novy order jednotek..

                    foreach (Unit unit in unitsOnBattleField)
                    {
                       unit._iniciation =  unit.CalculateIniciation();
                    }

                    SortUnitAccordingIniciation();
                    battleInfoPanel.UpdateUnitNewTurnOrder(unitsOnBattleField, _unit);
                }

                    
                UpdateActiveUnit();

                FindSquaresInUnitMoveRange();
                ShowSquaresWithinRange(true);


                FindSquaresInUnitAttackRange();
                ShowSquaresWithingAttackRange();

                leftUnitInfo.UpdateStats(_activeUnit);

                _turnIsOver = false;

                // check if is a victory.
            }
        }
    }

    private bool DetectPlayerInputs()
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

        if (Input.GetMouseButtonDown(1))
        {
            squarToMove = RaycastTargetSquar();
            if (squarToMove != null)
            {
                if(squarToMove.unitInSquar != null)
                {
                    detailPopup.ShowPopup(squarToMove.unitInSquar);
                }

                Debug.Log("Info unit panel popup");
            }
        }

        if (result)
        {
            switch (action)
            {
                case BattleAction.None:
                    Debug.Log("None Action");
                    result = false;
                    break;
                case BattleAction.Move:
                    MoveToSquar(squarToMove);
                    //battleLog.AddLog($"{_activeUnit._name} moved to square {squarToMove.xCoordinate} / {squarToMove.yCoordinate}");
                    battleLog.AddBattleLog($"{_activeUnit._name} moved");
                    result = true;
                    break;
                case BattleAction.Attack:
                    AttackInfo attackInfo = null;
                    attackInfo = AttackToUnit(unitOnSquare);
                    battleLog.AddAttackBattleLog(attackInfo, _activeUnit, unitOnSquare);
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
        }

        return result;
    }

    // for buttons
    public void SkipUnitTurn()
    {
        _turnIsOver = true;
        battleLog.AddBattleLog($"{_activeUnit._name} skip round");
    }

    public void StartBattle(BattleStartData battleStartData)
    {
        spriteLoader = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceSpriteLoader>();

        // this is for testing purpose .. Right now is not decided how we will set battlefield Size.
        // minumum is 6 and 10 
        battleStartData.Rows = 8;
        battleStartData.Collumn = 16;

        CreateBattleField(battleStartData.Rows, battleStartData.Collumn);
        SetUnitPosition(battleStartData);
        InitBattle(battleStartData);
        TestStartBattle();
    }

    private void CreateBattleField(int rowsCount, int collumCount)
    {
        this._collumCount = collumCount;
        this._rowsCount = rowsCount;

        _squaresInBattleField = new Squar[this._rowsCount, this._collumCount];

        for (int j = 0; j < this._rowsCount; j++)
        {
            GameObject row = _rows[j];

            for (int i = 0; i < this._collumCount; i++)
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
        }
    }

    private void InitBattle(BattleStartData battleData)
    {
        battleLog.AddBattleLog("Battle Start");
       
        _isPlayerTurn = true; // todo for now is not decided who will turn first .. ? ?? Now player...

        order = 0;
        int amountEnemies = battleData.enemyData.enemieUnits.Count;
        int amountPlayers = battleData.playerData.playerUnits.Count;

        int uniqNumber = 0;

        for (int i = 0; i < amountEnemies; i++)
        {
            DataUnit dataUnit = battleData.enemyData.enemieUnits[i];
            Squar squar = GetSquareFromGrid(dataUnit.StartXPosition, dataUnit.StartYPosition); // tady musí byt chech jestli nejsem mimo pole

            GameObject unt = Instantiate(_unit, squar.container.transform);
            Unit newUnit = unt.GetComponent<Unit>();

            var sprite = spriteLoader.LoadUnitSprite(dataUnit.ImageName);

            newUnit.InitUnit(dataUnit.Name, dataUnit.Health, dataUnit.Damage, dataUnit.Threat, dataUnit.RangeMax, dataUnit.StartYPosition,
                dataUnit.StartXPosition, uniqNumber, dataUnit.Movement, sprite, Unit.Team.Demon, dataUnit.RangeMin);


            squar.unitInSquar = newUnit;
            unitsOnBattleField.Add(newUnit);
            uniqNumber++;
        }

        for (int i = 0; i < amountPlayers; i++)
        {
            DataUnit dataUnit = battleData.playerData.playerUnits[i];
            Squar squar = GetSquareFromGrid(dataUnit.StartXPosition, dataUnit.StartYPosition);  // tady musí byt chech jestli nejsem mimo pole

            GameObject unit1 = Instantiate(_unit, squar.container.transform);
            Unit newUnit = unit1.GetComponent<Unit>();

            var sprite = spriteLoader.LoadSpecialistSprite(dataUnit.ImageName);

            newUnit.InitUnit(dataUnit.Name, dataUnit.Health, dataUnit.Damage, dataUnit.Threat, dataUnit.RangeMax, dataUnit.StartYPosition,
                dataUnit.StartXPosition, uniqNumber, dataUnit.Movement, sprite, Unit.Team.Human, dataUnit.RangeMin);

            squar.unitInSquar = newUnit;
            unitsOnBattleField.Add(newUnit);
            uniqNumber++;
        }

        // Sort unit order.
        SortUnitAccordingIniciation();
        battleInfoPanel.InitStartOrder(unitsOnBattleField, _unit);

    }

    private void SetUnitPosition(BattleStartData battleStartData)
    {

        SetPlayersUnitPosition(battleStartData);

        if (battleStartData.isRandomEnemyPosition)
        {
            SetEnemyRandomPosition(battleStartData);
        }
        else
        {
            // TODo
            SetEnemyRandomPosition(battleStartData); // třeba když je ambush tak by se měli nastavit specialni pozice atd..
        }
    }

    private void SetPlayersUnitPosition (BattleStartData battleStartData)
    {
        List<(int x, int y)> posiblePositions = new List<(int x , int y)>();

        int startSeachXPosition = 0;
        int radius = 3; // Hard coded not good..

        if (battleStartData.Rows % 2 == 0)
        {
            startSeachXPosition = (battleStartData.Rows -1 )/ 2 - 1;
        }
        else
        {
            startSeachXPosition = (battleStartData.Rows - 1) / 2 ;
        }

        for (int i = startSeachXPosition; i < startSeachXPosition + radius; i++)
        {
            (int x, int y) position = (0,0);

            for (int j = 0; j < radius; j++)
            {
                position = (i, j);
                posiblePositions.Add(position);
            }
        }

        // todo
        if (battleStartData.playerData.playerUnits.Count > posiblePositions.Count)
        {
            Debug.LogError("Number of Players is bigger than posiblePositions to put unit on BattleField Critical Error");
        }

        foreach (DataUnit dataUnit in battleStartData.playerData.playerUnits)
        {
            (int x, int y) occupiedPosition = dataUnit.SetRandomStartingPosition(posiblePositions);

            posiblePositions.Remove(occupiedPosition);
        }
    }


    private void SetEnemyRandomPosition (BattleStartData battleStartData)
    {
        List<(int x, int y)> posiblePositions = new List<(int x, int y)>();

        int startSeachYPosition = 0;
        int radius = 3; // Hard coded not good..
        int minDistanceFromPlayer = 5; // Hard coded not good..

        foreach (DataUnit dataUnit in battleStartData.playerData.playerUnits)
        {
            if(dataUnit.StartYPosition > startSeachYPosition)
            {
                startSeachYPosition = dataUnit.StartYPosition;
            }
        }

        startSeachYPosition += minDistanceFromPlayer;

        for (int i = 0; i < battleStartData.Rows -1; i++)
        {
            (int x, int y) position = (0, 0);

            for (int j = startSeachYPosition; j < battleStartData.Collumn -1; j++)
            {
                position = (i, j);
                posiblePositions.Add(position);
            }
        }

        // todo
        if (battleStartData.enemyData.enemieUnits.Count > posiblePositions.Count)
        {
            Debug.LogError("Number of Enemis is bigger than posiblePositions to put unit on BattleField Critical Error");
        }

        foreach (DataUnit dataUnit in battleStartData.enemyData.enemieUnits)
        {
            (int x, int y) occupiedPosition = dataUnit.SetRandomStartingPosition(posiblePositions);

            posiblePositions.Remove(occupiedPosition);
        }
    }

    private BattleStartData InitTestBattleData()
    {
        BattleStartData battleStartData = new BattleStartData();

        battleStartData.Rows = 8;
        battleStartData.Collumn = 16;

        DataUnit newA = new DataUnit(2,3,10,5,3,3,2, "Player1", "Gargoyle", 2);
        DataUnit newB = new DataUnit(4, 5, 5, 5, 7, 5, 2, "Player2", "Gargoyle", 2);
        DataUnit newC = new DataUnit(3, 0, 6, 2, 2, 1, 2, "Zombie1", "Zombie 1", 1);
        DataUnit newx = new DataUnit(2, 7, 8, 2, 1, 2, 1, "Zombie2", "Zombie 2", 0);
        DataUnit newy = new DataUnit(5, 10, 6, 1, 3, 1, 1, "Zombie3", "Zombie 1", 0);

        battleStartData.playerData.playerUnits.Add(newA);
        battleStartData.playerData.playerUnits.Add(newB);
        battleStartData.enemyData.enemieUnits.Add(newC);
        battleStartData.enemyData.enemieUnits.Add(newx);
        battleStartData.enemyData.enemieUnits.Add(newy);

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
        int rowsCount = _rows.Count - 1;

        while (true)
        {
            int xCor = Random.Range(0, rowsCount);
            int yCor = Random.Range(0, _collumCount);

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
        int success = BattleSystem.CalculateAmountSuccess(dices , defendUnit._threat, out attackInfo.dicesRoll);

        defendUnit.CurrentHealth = defendUnit.CurrentHealth - success;

        var isDead = defendUnit.CheckIfUnitIsNotDead();

        if (isDead)
        {
            KillUnitOnBattleField(defendUnit);
            battleInfoPanel.DeleteUnitFromOrder(defendUnit);
            battleLog.AddBattleLog($"{defendUnit._name} is dead");
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

    private bool VictoryConditionCheck ()
    {
        int humanUnit = 0;
        int demonUnit = 0;
        int neutralUnit = 0;

        foreach (var unit in unitsOnBattleField)
        {
            if(unit._team == Unit.Team.Human)
            {
                humanUnit++;
            }
            else if (unit._team == Unit.Team.Demon)
            {
                demonUnit++;
            }
            else if (unit._team == Unit.Team.Demon)
            {
                neutralUnit++;
            }
        }

        if(humanUnit == unitsOnBattleField.Count -1)
        {
            // human wins
        }

        if (demonUnit == unitsOnBattleField.Count -1)
        {
            // demon wins
        }

        if (neutralUnit == unitsOnBattleField.Count -1)
        {
            // neutral wins
        }


        return false;
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

        int attackMaxRange = _activeUnit._rangeMax;
        int attackMinRange = _activeUnit._rangeMin;

        Squar centerSquar = _squaresInBattleField[_activeUnit.CurrentPos.XPosition, _activeUnit.CurrentPos.YPosition];

        _squaresInUnitAttackRange.AddRange(GetTheAdjacentAttackSquare(centerSquar));

        List<Squar> squaresToMinAtackRange = new List<Squar>();

        List<Squar> lastAdjectedSq = new List<Squar>();
        lastAdjectedSq.AddRange(_squaresInUnitAttackRange);

        squaresToMinAtackRange.AddRange(_squaresInUnitAttackRange);


        for (int i = 1; i < attackMaxRange; i++)
        {
            List<Squar> adjectedSquarsInCurrentRange = new List<Squar>();

            foreach (Squar sq in lastAdjectedSq)
            {
                adjectedSquarsInCurrentRange.AddRange(GetTheAdjacentAttackSquare(sq));
            }

            lastAdjectedSq.Clear();
            lastAdjectedSq.AddRange(adjectedSquarsInCurrentRange);

            if (i < attackMinRange)  //  i <= by znamenalo ze zahrnuje hranici minimalniho range.
            {
                squaresToMinAtackRange.AddRange(adjectedSquarsInCurrentRange);
            }

            _squaresInUnitAttackRange.AddRange(lastAdjectedSq);
        }

        _squaresInUnitAttackRange.Add(centerSquar);

        if (attackMinRange != 0 && attackMinRange < attackMaxRange)
        {
            _squaresInUnitAttackRange.Remove(centerSquar);
            centerSquar.isInReach = false;

            foreach (Squar squar in squaresToMinAtackRange)
            {
                _squaresInUnitAttackRange.Remove(squar);
                squar.isInReach = false;
            }
        }
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

    private List<Squar> GetTheAdjacentSquare(Squar centerSquar) 
    {
        List<Squar> checkedSquars = new List<Squar>();

        Squar rightSquare = null;
        Squar leftSquare = null;
        Squar upSquare = null;
        Squar downSquare = null;

        // check up direction
        if (centerSquar.xCoordinate + 1 >= _rows.Count)
            upSquare = null;
        else
            upSquare = GetSquareFromGrid(centerSquar.xCoordinate + 1, centerSquar.yCoordinate);

        // check down direction
        if (centerSquar.xCoordinate - 1 < 0)
            downSquare = null;
        else
            downSquare = GetSquareFromGrid(centerSquar.xCoordinate - 1, centerSquar.yCoordinate);

        // check right direction
        if (centerSquar.yCoordinate + 1 >= _collumCount)
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

    private List<Squar> GetTheAdjacentAttackSquare(Squar centerSquar, bool searchForBorders = false)
    {
        List<Squar> checkedSquars = new List<Squar>();

        Squar rightSquare = null;
        Squar leftSquare = null;
        Squar upSquare = null;
        Squar downSquare = null;

        // check up direction
        if (centerSquar.xCoordinate + 1 >= _rowsCount)
            upSquare = null;
        else
            upSquare = GetSquareFromGrid(centerSquar.xCoordinate + 1, centerSquar.yCoordinate);

        // check down direction
        if (centerSquar.xCoordinate - 1 < 0)
            downSquare = null;
        else
            downSquare = GetSquareFromGrid(centerSquar.xCoordinate - 1, centerSquar.yCoordinate);

        // check right direction
        if (centerSquar.yCoordinate + 1 >= _collumCount)
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

    // Get Cross Squares (maybe will be useable)
    private List<Squar> GetTheAdjacentCrossSquare(Squar centerSquar)
    {
        List<Squar> checkedSquars = new List<Squar>();
        centerSquar.isInReach = true;

        Squar rightCrossSquare = null;
        Squar leftCrossSquare = null;
        Squar upCrossSquare = null;
        Squar downCrossSquare = null;

        // check upLeft direction
        if (centerSquar.xCoordinate + 1 >= _rows.Count || centerSquar.yCoordinate - 1 < 0)
            upCrossSquare = null;
        else
            upCrossSquare = GetSquareFromGrid(centerSquar.xCoordinate + 1, centerSquar.yCoordinate - 1);

        // check downLeft direction
        if (centerSquar.xCoordinate - 1 < 0 || centerSquar.yCoordinate - 1 < 0)
            downCrossSquare = null;
        else
            downCrossSquare = GetSquareFromGrid(centerSquar.xCoordinate - 1, centerSquar.yCoordinate - 1);

        // check upRight direction
        if (centerSquar.xCoordinate + 1 >= _rows.Count || centerSquar.yCoordinate + 1 >= _collumCount)
            rightCrossSquare = null;
        else
            rightCrossSquare = GetSquareFromGrid(centerSquar.xCoordinate + 1, centerSquar.yCoordinate + 1);

        // check downRight direction
        if (centerSquar.xCoordinate - 1 < 0 || centerSquar.yCoordinate + 1 >= _collumCount)
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

        battleLog.AddBattleLog($"{_activeUnit._name} has turn");
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

    private void SetCursor(Squar sq)
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

    private void UpdateRightPanel (Squar sq)
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

    public class AttackInfo
    {
        public int dices = 0;
        public int success = 0;

        public List<int> dicesRoll = new List<int>();      
    }



}
