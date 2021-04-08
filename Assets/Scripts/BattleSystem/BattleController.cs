using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.Events;

public class BattleController : MonoBehaviour
{
    BattleStartData battleData = new BattleStartData();

    [SerializeField] private BattleInfoPanel battleInfoPanel;

    public List<GameObject> rows = new List<GameObject>();

   // public List<Squar> squars = new List<Squar>();

    public Squar[,] squaresInBattleField; 

    public List<Unit> unitsOnBattleField = new List<Unit>();

    [Space]
    public GameObject _unit;

    [Space]
    public int collumCount = 16;

    public GameObject squarTemplate;

    /// <summary>
    /// ////////////////////
    /// 
    /// </summary>
    /// 

    private bool _isBattleOnline = false;
    private bool _isPlayerTurn = false;

    private Unit _activeUnit = null;

    private int order = 0;

    private List<Squar> _squaresInUnitRange = new List<Squar>();


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
            bool turnIsOver = false;

            if (_isPlayerTurn)
            {
                if (DetectPlayerInputs())
                {
                    turnIsOver = true;
                    _isPlayerTurn = false;
                }
            }
            else
            {
                // OnSimpleAIMove();

                if (DetectPlayerInputs())
                {
                    turnIsOver = true;
                    _isPlayerTurn = true;
                }

                // AI is on the move 
                // start corotine
                // switch to player turn
            }

            if (turnIsOver)
            {
               // SetSquaresUnvisited();
                // default all squesr

                order++;

                if (order >= unitsOnBattleField.Count)
                    order = 0;

                UpdateActiveUnit();

                FindSquaresInUnitRange();
                ShowSquaresWithinRange(true);

                // order change
                // so on

            }
        }
    }

    public bool DetectPlayerInputs()
    {
        bool result = false;

        if (Input.GetMouseButtonDown(0))
        {
            Squar squarToMove = RaycastTargetSquar();
            if (squarToMove != null)
            {
                result = OnMoveUnit(squarToMove);
            }
        }

        if(result)
        {
            ShowSquaresWithinRange(false);
        }

        return result;
    }

    public void CreateBattleField()
    {

        squaresInBattleField = new Squar[rows.Count, collumCount];

        int j = 0;
        foreach (GameObject row in rows)
        {
            for (int i = 0; i < collumCount; i++)
            {
                GameObject squarGameObject = Instantiate(squarTemplate, row.transform);
                Squar square = squarGameObject.GetComponent<Squar>();
                square.SetCoordinates(j, i);
                
                squaresInBattleField[j, i] = square;

                // (delegate () { ShowMissionPanel(choisedMission); });

                square.InitEvent(delegate (Squar squ) 
                { 
                    QQWEQE(squ);
                    square.m_MyEvent.isInRange = true;
                    squ.m_MyEvent.isTesting2 = true;
                });
            }
            j++;
        }

    }

    public void InitBattle()
    {
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

            newUnit.InitUnit(dataUnit._name, dataUnit.health, dataUnit.damage, dataUnit.threat, dataUnit.range, dataUnit.StartPos, uniqNumber, dataUnit._movement);
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

            newUnit.InitUnit(dataUnit._name, dataUnit.health, dataUnit.damage, dataUnit.threat, dataUnit.range, dataUnit.StartPos, uniqNumber, dataUnit._movement);
            squar.unitInSquar = newUnit;
            unitsOnBattleField.Add(newUnit);
            uniqNumber++;
        }


        battleInfoPanel.InitStartOrder(unitsOnBattleField, _unit);

        // sort unitsonbattlefilds according iniciation 
        // so on.
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
        newA.StartPos.XPosition = 7;
        newA.StartPos.YPosition = 15;
        newA._name = "Player1";
        newA.imageName = "Player1";
        newA.health = 7;
        newA.damage = 6;
        newA.threat = 3;
        newA.range = 0;

        newB.StartPos.XPosition = 0;
        newB.StartPos.YPosition = 0;
        newB._name = "Zombie1";
        newB.imageName = "Zombie1";
        newB.health = 5;
        newB.damage = 4;
        newB.threat = 5;
        newB.range = 2;

        newC.StartPos.XPosition = 0;
        newC.StartPos.YPosition = 6;
        newC._name = "Zombie2";
        newC.imageName = "Zombie2";
        newC.health = 2;
        newC.damage = 3;
        newC.threat = 1;
        newC.range = 1;

        newx.StartPos.XPosition = 5;
        newx.StartPos.YPosition = 2;
        newx._name = "Zombie3";
        newx.imageName = "Zombie3";
        newx.health = 2;
        newx.damage = 3;
        newx.threat = 2;
        newx.range = 1;


        newy.StartPos.XPosition = 4;
        newy.StartPos.YPosition = 6;
        newy._name = "Player2";
        newy.imageName = "Player2";
        newy.health = 1;
        newy.damage = 1;
        newy.threat = 2;
        newy.range = 1;

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
            FindSquaresInUnitRange();
            ShowSquaresWithinRange(true);
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

            if (squaresInBattleField[xCor, yCor].unitInSquar == null)
            {
                return squaresInBattleField[xCor, yCor];
            }
        }
    }

    // Buttons
    private bool OnMoveUnit(Squar squarToMove)
    {
        bool canMove = false;

        if (squarToMove.unitInSquar != null)
        {
            return false;
        }

        if (squarToMove.xCoordinate < 0 || squarToMove.yCoordinate < 0 || squarToMove.xCoordinate >= rows.Count || squarToMove.yCoordinate >= collumCount)
        {
            return false;
        }

        // canMove = IsSquarInMoveRange(squarToMove);

        // check if my Square is in Range
        if (_squaresInUnitRange.Contains(squarToMove))
        {
            canMove = true;
        }

        if (canMove)
        {
            SetSquaresUnvisited();
            MoveToSquar(squarToMove);
        }
        else
        {
            // You can not move that far... Nejake info pro hrace ze tam se nemuže pohnout..
        }

        return canMove;
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

    // This check only range 1
    //public bool IsSquarInMoveRange(Squar squarToCheck)
    //{
    //    List<Squar> totalSquarsInRange = new List<Squar>();

    //    int moveRange = _activeUnit._movement;
    //    bool isInRange = false;

    //    Squar centerSquar = squaresInBattleField[_activeUnit.CurrentPos.XPosition, _activeUnit.CurrentPos.YPosition];

    //    totalSquarsInRange.AddRange(GetTheAdjacentSquare(centerSquar));

    //    List<Squar> adjectedSq = new List<Squar>();
    //    adjectedSq.AddRange(totalSquarsInRange);

    //    for (int i = 1; i < moveRange; i++)
    //    {
    //        List<Squar> adjectedSquarsInCurrentRange = new List<Squar>();

    //        foreach (Squar sq in adjectedSq)
    //        {
    //            adjectedSquarsInCurrentRange.AddRange(GetTheAdjacentSquare(sq));
    //        }

    //        adjectedSq.Clear();
    //        adjectedSq.AddRange(adjectedSquarsInCurrentRange);

    //        totalSquarsInRange.AddRange(adjectedSq);
    //    }

    //    // check if my Square is in Range
    //    if (totalSquarsInRange.Contains(squarToCheck))
    //    {
    //        isInRange = true;
    //    }

    //    // setVisited Sq to false
    //    foreach (Squar item in totalSquarsInRange)
    //    {
    //        item.isVisited = false;
    //    }
    //    centerSquar.isVisited = false;

    //    return isInRange;
    //}

    // in this method i can mark squares and maybe change method for move..  Can be more easy to find..
    private void FindSquaresInUnitRange()
    {
        _squaresInUnitRange.Clear();

        int moveRange = _activeUnit._movement;
        Squar centerSquar = squaresInBattleField[_activeUnit.CurrentPos.XPosition, _activeUnit.CurrentPos.YPosition];

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

            adjectedSq.Clear();
            adjectedSq.AddRange(adjectedSquarsInCurrentRange);

            _squaresInUnitRange.AddRange(adjectedSq);
        }

        // remove full Squares
        for (int i = _squaresInUnitRange.Count -1 ; i >= 0; i--)
        {
            Squar squareInRange = _squaresInUnitRange[i];

            if (squareInRange.unitInSquar != null)
            {
                _squaresInUnitRange.Remove(squareInRange);
            }
        }
    }

    private void ShowSquaresWithinRange(bool makeVisible)
    {
        foreach (Squar sq in _squaresInUnitRange)
        {
            sq.inRangeBackground.SetActive(makeVisible);
        }
    }

    public List<Squar> GetTheAdjacentSquare(Squar centerSquar) 
    {
        List<Squar> checkedSquars = new List<Squar>();
        centerSquar.isVisited = true;

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

        foreach (Squar sq in checkedSquars)
        {
            sq.isVisited = true;
        }

        return checkedSquars;
    }

    private Squar GetSquareFromGrid(int x , int y)
    {
        Squar sq = squaresInBattleField[x, y];
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

        // setVisited Sq to false
        Squar centerSquar = squaresInBattleField[_activeUnit.CurrentPos.XPosition, _activeUnit.CurrentPos.YPosition];
        centerSquar.isVisited = false;
        centerSquar.inRangeBackground.SetActive(false);

        foreach (Squar item in _squaresInUnitRange)
        {
            item.inRangeBackground.SetActive(false);
            item.isVisited = false;
        }
    }

    public void QQWEQE(Squar sq)
    {
        sq.ShowCircle();
       // sq.OnPointerEnter();
    }

}
