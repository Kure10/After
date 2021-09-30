﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleGridController : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject squarTemplate;

    [Header("Dimensions")]
    [SerializeField] List<GameObject> _rows = new List<GameObject>();

    private Squar[,] _squaresInBattleField;

    private int _columnCount = 12;
    private int _rowsCount = 4;

    // Test

    private int _groupStoneBlockCount = 0;
    private int _groupStoneSize = 0;
    private int _stoneCount = 5;
    private int _maxStoneInRow = 0;
    private int _maxStoneInColumn = 0;

    // Test block


    public Squar[,] GetSquarsFromBattleField { get { return _squaresInBattleField; } }




    public void CreateBattleField(BattleStartData data)
    {
        this._columnCount = data.Collumn;
        this._rowsCount = data.Rows;

        _squaresInBattleField = new Squar[this._columnCount, this._rowsCount];

        for (int y = 0; y < this._rowsCount; y++)
        {
            GameObject row = _rows[y];

            for (int x = 0; x < this._columnCount; x++)
            {
                GameObject squarGameObject = Instantiate(squarTemplate, row.transform);
                Squar square = squarGameObject.GetComponent<Squar>();
                square.SetCoordinates(x, y);
                square.gameObject.SetActive(true);

                _squaresInBattleField[x, y] = square;
            }
        }

        // Need this for complicated condition... 
        _maxStoneInRow = _rowsCount - 2;
        _maxStoneInColumn = _columnCount - 2;

        // modify slots // water // blocked // so on

        GenerateRandomTerrain();

    }

    public void MoveToSquar(Unit unit, Squar squarToMove)
    {
        Squar selectedUnitSquar = GetSquareFromGrid(unit.CurrentPos.XPosition, unit.CurrentPos.YPosition);

        selectedUnitSquar.UnitInSquar.transform.SetParent(squarToMove.container.transform);
        selectedUnitSquar.UnitInSquar.SetNewCurrentPosition(squarToMove.xCoordinate, squarToMove.yCoordinate);
        squarToMove.UnitInSquar = selectedUnitSquar.UnitInSquar;
        selectedUnitSquar.UnitInSquar = null;
    }

    public AttackInfo AttackToUnit(Unit attackingUnit, Unit defendingUnit)
    {
        AttackInfo attackInfo = new AttackInfo();

        int dices = BattleSystem.CalculateAmountDices(attackingUnit);
        int success = BattleSystem.CalculateAmountSuccess(dices, attackingUnit, defendingUnit, out attackInfo.dicesValueRoll);

        defendingUnit.CurrentHealth = defendingUnit.CurrentHealth - success;

        attackInfo.unitDied = defendingUnit.CheckIfUnitIsNotDead();

        // for info
        attackInfo.dices = dices;
        attackInfo.success = success;

        return attackInfo;
    }

    public void DestroyUnitFromBattleField(Unit unit)
    {
        Squar sq = GetSquareFromGrid(unit.CurrentPos.XPosition, unit.CurrentPos.YPosition);
        sq.UnitInSquar.gameObject.SetActive(false);

        // deadUnitsOnBattleField.Add(sq.unitInSquar);
        // Destroy(sq.unitInSquar.gameObject, 0.5f);

        sq.UnitInSquar = null;
    }

    public List<Squar> FindSquaresInUnitMoveRange(Unit unit)
    {
        List<Squar> squaresInUnitMoveRange = new List<Squar>();

        int moveRange = unit.GetMovementPoints;
        Squar centerSquar = _squaresInBattleField[unit.CurrentPos.XPosition, unit.CurrentPos.YPosition];

        squaresInUnitMoveRange.AddRange(GetTheAdjacentSquare(centerSquar));

        List<Squar> adjectedSq = new List<Squar>();
        adjectedSq.AddRange(squaresInUnitMoveRange);

        for (int i = 1; i < moveRange; i++)
        {
            List<Squar> adjectedSquarsInCurrentRange = new List<Squar>();

            foreach (Squar sq in adjectedSq)
            {
                adjectedSquarsInCurrentRange.AddRange(GetTheAdjacentSquare(sq));
            }

            foreach (Squar sq in adjectedSq)
            {
                sq.isInMoveRange = true;
            }

            adjectedSq.Clear();
            adjectedSq.AddRange(adjectedSquarsInCurrentRange);

            squaresInUnitMoveRange.AddRange(adjectedSq);
        }

        squaresInUnitMoveRange.Add(centerSquar);

        return squaresInUnitMoveRange;
    }

    public List<Squar> FindSquaresInUnitAttackRange(Unit unit)
    {
        Item weapon = unit.ActiveWeapon;
        int attackMaxRange = 0;
        int attackMinRange = 0;
        //todo
        if (weapon == null)
        {
            attackMinRange = unit._rangeMin;
            attackMaxRange = unit._rangeMax;
        }
        else
        {
            attackMaxRange = unit.ActiveWeapon.RangeMax;
            attackMinRange = unit.ActiveWeapon.RangeMin;
        }

        List<Squar> squaresInUnitAttackRange = new List<Squar>();

        Squar centerSquar = _squaresInBattleField[unit.CurrentPos.XPosition, unit.CurrentPos.YPosition];

        squaresInUnitAttackRange.AddRange(GetTheAdjacentAttackSquare(centerSquar));

        List<Squar> squaresToMinAtackRange = new List<Squar>();

        List<Squar> lastAdjectedSq = new List<Squar>();
        lastAdjectedSq.AddRange(squaresInUnitAttackRange);

        squaresToMinAtackRange.AddRange(squaresInUnitAttackRange);


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

            squaresInUnitAttackRange.AddRange(lastAdjectedSq);
        }

        squaresInUnitAttackRange.Add(centerSquar);

        if (attackMinRange != 0 && attackMinRange < attackMaxRange)
        {
            squaresInUnitAttackRange.Remove(centerSquar);
            centerSquar.isInAttackReach = false;

            foreach (Squar squar in squaresToMinAtackRange)
            {
                squaresInUnitAttackRange.Remove(squar);
                squar.isInAttackReach = false;
            }
        }

        return squaresInUnitAttackRange;
    }

    private List<Squar> GetTheAdjacentSquare(Squar centerSquar)
    {
        List<Squar> checkedSquars = new List<Squar>();

        Squar rightSquare = null;
        Squar leftSquare = null;
        Squar upSquare = null;
        Squar downSquare = null;
        
        // check up direction
        if (centerSquar.xCoordinate + 1 >= _columnCount)
            upSquare = null;
        else
            upSquare = GetSquareFromGrid(centerSquar.xCoordinate + 1, centerSquar.yCoordinate);

        // check down direction
        if (centerSquar.xCoordinate - 1 < 0)
            downSquare = null;
        else
            downSquare = GetSquareFromGrid(centerSquar.xCoordinate - 1, centerSquar.yCoordinate);

        // check right direction
        if (centerSquar.yCoordinate + 1 >= _rowsCount)
            rightSquare = null;
        else
        {
            var number = centerSquar.yCoordinate + 1;
            Debug.Log("x :  " + centerSquar.xCoordinate + "  y :   " + number);
            rightSquare = GetSquareFromGrid(centerSquar.xCoordinate, centerSquar.yCoordinate + 1);
        }

        // check left direction
        if (centerSquar.yCoordinate - 1 < 0)
            leftSquare = null;
        else
            leftSquare = GetSquareFromGrid(centerSquar.xCoordinate, centerSquar.yCoordinate - 1);

        if (rightSquare != null && !rightSquare.isInMoveRange)
            checkedSquars.Add(rightSquare);
        if (leftSquare != null && !leftSquare.isInMoveRange)
            checkedSquars.Add(leftSquare);
        if (upSquare != null && !upSquare.isInMoveRange)
            checkedSquars.Add(upSquare);
        if (downSquare != null && !downSquare.isInMoveRange)
            checkedSquars.Add(downSquare);

        centerSquar.isInMoveRange = true;

        foreach (Squar sq in checkedSquars)
        {
            sq.isInMoveRange = true;
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

        // check right direction
        if (centerSquar.xCoordinate + 1 >= _columnCount)
            rightSquare = null;
        else
            rightSquare = GetSquareFromGrid(centerSquar.xCoordinate + 1, centerSquar.yCoordinate);

        // check left direction
        if (centerSquar.xCoordinate - 1 < 0)
            leftSquare = null;
        else
            leftSquare = GetSquareFromGrid(centerSquar.xCoordinate - 1, centerSquar.yCoordinate);

        // check up direction
        if (centerSquar.yCoordinate + 1 >= _rowsCount)
            upSquare = null;
        else
            upSquare = GetSquareFromGrid(centerSquar.xCoordinate, centerSquar.yCoordinate + 1);

        // check down direction
        if (centerSquar.yCoordinate - 1 < 0)
            downSquare = null;
        else
            downSquare = GetSquareFromGrid(centerSquar.xCoordinate, centerSquar.yCoordinate - 1);

        if (searchForBorders)
        {
            if (leftSquare == null || !leftSquare.isInAttackReach)
                centerSquar.leftBorder.SetActive(true);

            if (rightSquare == null || !rightSquare.isInAttackReach)
                centerSquar.rightBorder.SetActive(true);

            if (downSquare == null || !downSquare.isInAttackReach)
                centerSquar.downBorder.SetActive(true);

            if (upSquare == null || !upSquare.isInAttackReach)
                centerSquar.upBorder.SetActive(true);
        }
        else
        {
            if (rightSquare != null && !rightSquare.isInAttackReach)
                checkedSquars.Add(rightSquare);
            if (leftSquare != null && !leftSquare.isInAttackReach)
                checkedSquars.Add(leftSquare);
            if (upSquare != null && !upSquare.isInAttackReach)
                checkedSquars.Add(upSquare);
            if (downSquare != null && !downSquare.isInAttackReach)
                checkedSquars.Add(downSquare);

            centerSquar.isInAttackReach = true;

            foreach (Squar sq in checkedSquars)
            {
                sq.isInAttackReach = true;
            }
        }

        return checkedSquars;
    }

    private List<Squar> GetTheAdjacentCrossSquare(Squar centerSquar)
    {
        List<Squar> checkedSquars = new List<Squar>();
        centerSquar.isInAttackReach = true;

        Squar rightCrossSquare = null;
        Squar leftCrossSquare = null;
        Squar upCrossSquare = null;
        Squar downCrossSquare = null;

        // check upLeft direction
        if (centerSquar.xCoordinate + 1 >= _rowsCount || centerSquar.yCoordinate - 1 < 0)
            upCrossSquare = null;
        else
            upCrossSquare = GetSquareFromGrid(centerSquar.xCoordinate + 1, centerSquar.yCoordinate - 1);

        // check downLeft direction
        if (centerSquar.xCoordinate - 1 < 0 || centerSquar.yCoordinate - 1 < 0)
            downCrossSquare = null;
        else
            downCrossSquare = GetSquareFromGrid(centerSquar.xCoordinate - 1, centerSquar.yCoordinate - 1);

        // check upRight direction
        if (centerSquar.xCoordinate + 1 >= _rowsCount || centerSquar.yCoordinate + 1 >= _columnCount)
            rightCrossSquare = null;
        else
            rightCrossSquare = GetSquareFromGrid(centerSquar.xCoordinate + 1, centerSquar.yCoordinate + 1);

        // check downRight direction
        if (centerSquar.xCoordinate - 1 < 0 || centerSquar.yCoordinate + 1 >= _columnCount)
            leftCrossSquare = null;
        else
            leftCrossSquare = GetSquareFromGrid(centerSquar.xCoordinate - 1, centerSquar.yCoordinate + 1);

        if (rightCrossSquare != null && !rightCrossSquare.isInAttackReach)
            checkedSquars.Add(rightCrossSquare);
        if (leftCrossSquare != null && !leftCrossSquare.isInAttackReach)
            checkedSquars.Add(leftCrossSquare);
        if (upCrossSquare != null && !upCrossSquare.isInAttackReach)
            checkedSquars.Add(upCrossSquare);
        if (downCrossSquare != null && !downCrossSquare.isInAttackReach)
            checkedSquars.Add(downCrossSquare);

        foreach (Squar sq in checkedSquars)
        {
            sq.isInAttackReach = true;
        }

        return checkedSquars;
    }

    public void ClearnSquaresInBattleField()
    {
        if (_squaresInBattleField != null)
        {
            int sizeX = _squaresInBattleField.GetLength(0);
            int sizeY = _squaresInBattleField.GetLength(1);
            for (int j = 0; j < sizeX; j++)
            {
                GameObject row = _rows[j];

                for (int i = 0; i < sizeY; i++)
                {
                    Destroy(_squaresInBattleField[j, i].gameObject);
                }
            }
            _squaresInBattleField = null;
        }
    }

    public Squar GetSquareFromGrid(int x, int y)
    {
        Squar sq = _squaresInBattleField[x, y];
        return sq;
    }

    // Map Terrain Varianty

    public void GenerateRandomTerrain()
    {
        // vyber spravny druh terenu muže byt random
        TerrainVariants terrainVariaty = TerrainVariants.StoneElShape;

        switch (terrainVariaty)
        {
            case TerrainVariants.Normal:
                break;
            case TerrainVariants.StoneElShape:
                GenerateStoneElShape();
                break;
            case TerrainVariants.StoneOneSpot:
                break;
            case TerrainVariants.StoneLine:
                break;
            case TerrainVariants.Water:
                break;
            case TerrainVariants.DeepWater:
                break;
            case TerrainVariants.WildRandom:
                break;
            case TerrainVariants.WaterPlusStones:
                break;
            case TerrainVariants.DeepWaterPlusStones:
                break;
            case TerrainVariants.CrossLine:
                break;
            case TerrainVariants.VStones:
                break;
            default:
                break;
        }
    }

    //
    private void GenerateStoneElShape()
    {
        int countElShapes = 2; // Kolik utvaru se spawnuje
        int size = 6;          // celkova velikost utvaru na pocet kostek

        bool isOnRight = false;
        bool isOnTop = false;
        int longSize = Mathf.RoundToInt((size * 0.66f));
        int shortSize =  size - longSize;

        int xHalf = _columnCount / 2;
        int yHalf = _rowsCount / 2;

        int safeCounterLimit = 10; // This is a safe counter if Stone cant be created. Counter will stop while Loop.
        bool wasTriggeredSafeCounter = false;

        for (int i = 0; i < countElShapes; i++)
        {
            int direction = Random.Range(0, 2); // 0 for vertical // 1 for horizontal -> pro short obracene.
            int xStartStonePosition = 0;
            int yStartStonePosition = 0;

            while (true)
            {
                xStartStonePosition = Random.Range(2, _columnCount - 3); // Nahodná pozice kde bude starting block.
                yStartStonePosition = Random.Range(1, _rowsCount - 1);

                bool wasCreated = CreateStone(xStartStonePosition, yStartStonePosition);

                if (xStartStonePosition > xHalf)
                {
                    isOnRight = true;
                }
                else
                {
                    isOnRight = false;
                }

                if (yStartStonePosition > yHalf)
                {
                    isOnTop = true;
                }
                else
                {
                    isOnTop = false;
                }

                if (wasCreated)
                    break;

                safeCounterLimit--;
                if (safeCounterLimit <= 0)
                {
                    Debug.Log("safe counter was trigger.  BattleFiled was not created");
                    wasTriggeredSafeCounter = true;
                    break;
                }
            }

            if(!wasTriggeredSafeCounter)
            {
                for (int l = 0; l < longSize; l++)
                {
                    if (isOnRight && isOnTop)
                    {
                        if (direction > 0)
                        {
                            CreateStone(xStartStonePosition - l, yStartStonePosition);
                        }
                        else
                        {
                            CreateStone(xStartStonePosition, yStartStonePosition - l);
                        }
                    }
                    else if (!isOnRight && isOnTop)
                    {
                        if (direction > 0)
                        {
                            CreateStone(xStartStonePosition + l, yStartStonePosition);
                        }
                        else
                        {
                            CreateStone(xStartStonePosition, yStartStonePosition - l);
                        }
                    }
                    else if (isOnRight && !isOnTop)
                    {
                        if (direction > 0)
                        {
                            CreateStone(xStartStonePosition - l, yStartStonePosition);
                        }
                        else
                        {
                            CreateStone(xStartStonePosition, yStartStonePosition + l);
                        }
                    }
                    else
                    {
                        if (direction > 0)
                        {
                            CreateStone(xStartStonePosition + l, yStartStonePosition);
                        }
                        else
                        {
                            CreateStone(xStartStonePosition, yStartStonePosition + l);
                        }
                    }
                }

                int offSet = Random.Range(0, longSize);

                for (int k = 1; k < shortSize + 1; k++)
                {
                    if (isOnRight && isOnTop)
                    {
                        if (direction > 0)
                        {
                            CreateStone(xStartStonePosition - offSet, yStartStonePosition - k);
                        }
                        else
                        {
                            CreateStone(xStartStonePosition - k, yStartStonePosition - offSet);
                        }
                    }
                    else if (!isOnRight && isOnTop)
                    {
                        if (direction > 0)
                        {
                            CreateStone(xStartStonePosition + offSet, yStartStonePosition - k);
                        }
                        else
                        {
                            CreateStone(xStartStonePosition + k, yStartStonePosition - offSet);
                        }
                    }
                    else if (isOnRight && !isOnTop)
                    {
                        if (direction > 0)
                        {
                            CreateStone(xStartStonePosition - offSet, yStartStonePosition + k);
                        }
                        else
                        {
                            CreateStone(xStartStonePosition - k, yStartStonePosition + offSet);
                        }
                    }
                    else
                    {
                        if (direction > 0)
                        {
                            CreateStone(xStartStonePosition + offSet, yStartStonePosition + k);
                        }
                        else
                        {
                            CreateStone(xStartStonePosition + k, yStartStonePosition + offSet);
                        }
                    }
                }
            }
        }
    }

    private bool CreateStone(int x, int y)
    {

       
        bool isInsideBorders = x < _columnCount && x >= 0 && y >= 0 && y < _rowsCount;  // This condition prevent stone Be out Of battleField
        bool secondCondition = false;  // This condition prevent stones blocked road from one side to other.

        if (isInsideBorders)
        {
            int columnRockCounter = 0;
            int rowsRockCounter = 0;

            for (int i = 0; i < _columnCount -1; i++)
            {
                if(_squaresInBattleField[i, y].IsSquearBlocked)
                {
                    columnRockCounter++;
                }
            }

            for (int i = 0; i < _rowsCount - 1; i++)
            {
                if (_squaresInBattleField[x, i].IsSquearBlocked)
                {
                    rowsRockCounter++;
                }
            }

            secondCondition = rowsRockCounter <= _maxStoneInRow && columnRockCounter <= _maxStoneInColumn;
        }

        if (isInsideBorders && secondCondition)
        {
            Squar squar = _squaresInBattleField[x, y];

            if (!squar.IsSquearBlocked)
            {
                // Create stone. In future it will be good if stone prefab will be difretent.

                squar.IsSquearBlocked = true;
                return true;
            }
        }
        else
        {
            Debug.Log("I tryed create block out of condition border");
            Debug.Log(" x: " + x + " y: " + y);
        }

        return false; 
    }

    #region HelperClass

    public class AttackInfo
    {
        public int dices = 0;
        public int success = 0;

        public bool unitDied = false;

        public List<int> dicesValueRoll = new List<int>();
    }

    #endregion

    enum TerrainVariants
    {
        Normal,
        StoneElShape,
        StoneOneSpot,
        StoneLine,
        Water,
        DeepWater,
        WildRandom,
        WaterPlusStones,
        DeepWaterPlusStones,
        CrossLine,
        VStones
    }
}