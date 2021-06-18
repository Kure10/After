using System.Collections;
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

    private int _collumCount = 12;
    private int _rowsCount = 4;

    public Squar[,] GetSquarsFromBattleField { get { return _squaresInBattleField; } }

    public void CreateBattleField(BattleStartData data)
    {
        this._collumCount = data.Collumn;
        this._rowsCount = data.Rows;

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
            }
        }
    }

    public void MoveToSquar(Unit unit, Squar squarToMove)
    {
        Squar selectedUnitSquar = GetSquareFromGrid(unit.CurrentPos.XPosition, unit.CurrentPos.YPosition);

        selectedUnitSquar.unitInSquar.transform.SetParent(squarToMove.container.transform);
        selectedUnitSquar.unitInSquar.SetNewCurrentPosition(squarToMove.xCoordinate, squarToMove.yCoordinate);
        squarToMove.unitInSquar = selectedUnitSquar.unitInSquar;
        selectedUnitSquar.unitInSquar = null;
    }

    public AttackInfo AttackToUnit(Unit attackingUnit,Unit defendingUnit)
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
        sq.unitInSquar.gameObject.SetActive(false);

        // deadUnitsOnBattleField.Add(sq.unitInSquar);
        // Destroy(sq.unitInSquar.gameObject, 0.5f);

        sq.unitInSquar = null;
    }

    public List<Squar> FindSquaresInUnitMoveRange(Unit unit)
    {
        List<Squar> squaresInUnitMoveRange = new List<Squar>();
  
        int moveRange = unit._movement;
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
        if (centerSquar.xCoordinate + 1 >= _rowsCount || centerSquar.yCoordinate + 1 >= _collumCount)
            rightCrossSquare = null;
        else
            rightCrossSquare = GetSquareFromGrid(centerSquar.xCoordinate + 1, centerSquar.yCoordinate + 1);

        // check downRight direction
        if (centerSquar.xCoordinate - 1 < 0 || centerSquar.yCoordinate + 1 >= _collumCount)
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

    public class AttackInfo
    {
        public int dices = 0;
        public int success = 0;

        public bool unitDied = false;

        public List<int> dicesValueRoll = new List<int>();
    }
}
