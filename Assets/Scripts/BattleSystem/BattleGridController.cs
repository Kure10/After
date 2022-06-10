using Assets.Scripts.BattleSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

public class BattleGridController : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject squarTemplate;

    [Header("Prefabs Obstacles")]
    [SerializeField] List<GameObject> _obstaclesList = new List<GameObject>();

    [Header("Dimensions")]
    [SerializeField] List<GameObject> _rows = new List<GameObject>();

    static private Squar[,] _squaresInBattleField;

    private int _columnCount = 12;
    private int _rowsCount = 4;

    public static Squar[,] GetSquarsFromBattleField { get { return _squaresInBattleField; } }

    public void CreateBattleField(BattleStartData data)
    {
        this._columnCount = data.Collumn;
        this._rowsCount = data.Rows;

        _squaresInBattleField = new Squar[this._rowsCount, this._columnCount];

        for (int x = 0; x < this._rowsCount; x++)
        {
            GameObject row = _rows[x];

            for (int y = 0; y < this._columnCount; y++)
            {
                GameObject squarGameObject = Instantiate(squarTemplate, row.transform);
                Squar square = squarGameObject.GetComponent<Squar>();
                square.SetCoordinates(x, y);
                square.gameObject.SetActive(true);

                _squaresInBattleField[x, y] = square;
            }
        }

        // modify slots // water // blocked // so on
        GenerateRandomTerrain();
    }

    public void MoveToSquar(Unit unit, Squar squarToMove)
    {
        Squar selectedUnitSquar = GetSquareFromGrid(unit.CurrentPos.XPosition, unit.CurrentPos.YPosition);

        selectedUnitSquar.UnitInSquar.transform.SetParent(squarToMove.GetContainer.transform);
        selectedUnitSquar.UnitInSquar.SetNewCurrentPosition(squarToMove.xCoordinate, squarToMove.yCoordinate);
        squarToMove.UnitInSquar = selectedUnitSquar.UnitInSquar;
        selectedUnitSquar.UnitInSquar = null;
    }

    public async Task MoveToPathAsync(Unit unit, List<Squar> path, int delayTimeWalk, List<Squar> squaresInMoveRange, List<Squar> squaresInAttackRange, BattleController battleController)
    {
        List<Squar> tmpSquaresInMoverange = new List<Squar>(squaresInMoveRange);
        SetSquaresOutOfAttackReach(squaresInAttackRange);

        foreach (Squar nextSq in path)
        {
            unit.DecreaseMovementPoints(1);
            ShowSquaresWithinMoveRange(tmpSquaresInMoverange, false);
            SetSquaresOutOfMoveRange(tmpSquaresInMoverange);
            MoveToSquar(unit, nextSq);
            tmpSquaresInMoverange = FindSquaresInUnitMoveRange(unit);
            ShowSquaresWithinMoveRange(tmpSquaresInMoverange, true);
            await Task.Delay(delayTimeWalk);
        }
        squaresInMoveRange.Clear();
        squaresInMoveRange.AddRange(tmpSquaresInMoverange);
        await Task.Yield();
        battleController.IsPerformingAction = false;
    }

    public void ShowSquaresWithinMoveRange(List<Squar> squaresInMoveRange, bool makeVisible)
    {
        foreach (Squar sq in squaresInMoveRange)
        {
            if (sq.UnitInSquar == null)
            {
                sq.inRangeBackground.SetActive(makeVisible);
            }
        }
    }

    public void ShowSquaresWithingAttackRange(List<Squar> squaresInAttackRange)
    {
        foreach (Squar squ in squaresInAttackRange)
        {
            GetTheAdjacentAttackSquare(squ, true);
        }
    }

    public void SetSquaresOutOfMoveRange(List<Squar> squaresInMoveRange)
    {
        foreach (Squar squar in squaresInMoveRange)
        {
            squar.inRangeBackground.SetActive(false);
            squar.isInMoveRange = false;
        }
        squaresInMoveRange.Clear();
    }

    public void SetSquaresOutOfAttackReach(List<Squar> SquaresInAttackRange)
    {
        foreach (Squar item in SquaresInAttackRange)
        {
            item.DisableAttackBorders();
            item.isInAttackReach = false;
        }
        SquaresInAttackRange.Clear();
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
        int moveRange = unit.GetMovementPoints;
        Squar centerSquar = _squaresInBattleField[unit.CurrentPos.XPosition, unit.CurrentPos.YPosition];
        List<Squar> squaresInUnitMoveRange = new List<Squar>();

        if (moveRange > 0)
        {
            squaresInUnitMoveRange.AddRange(GetTheAdjacentMoveSquare(centerSquar));

            List<Squar> adjectedSq = new List<Squar>();
            adjectedSq.AddRange(squaresInUnitMoveRange);

            for (int i = 1; i < moveRange; i++)
            {
                List<Squar> adjSq = new List<Squar>();

                foreach (Squar sq in adjectedSq)
                {
                    adjSq.AddRange(GetTheAdjacentMoveSquare(sq));
                }

                squaresInUnitMoveRange.AddRange(adjSq);
                adjectedSq.Clear();
                adjectedSq.AddRange(adjSq);
            }
        }
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

    public List<Squar> GetTheAdjacentWalkAbleSquare(Squar centerSquar)
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
        if (centerSquar.yCoordinate + 1 >= _columnCount)
            rightSquare = null;
        else
            rightSquare = GetSquareFromGrid(centerSquar.xCoordinate, centerSquar.yCoordinate + 1);

        // check left direction
        if (centerSquar.yCoordinate - 1 < 0)
            leftSquare = null;
        else
            leftSquare = GetSquareFromGrid(centerSquar.xCoordinate, centerSquar.yCoordinate - 1);

        if (IsSquareWalkAble(rightSquare))
            checkedSquars.Add(rightSquare);

        if (IsSquareWalkAble(leftSquare))
            checkedSquars.Add(leftSquare);

        if (IsSquareWalkAble(upSquare))
            checkedSquars.Add(upSquare);

        if (IsSquareWalkAble(downSquare))
            checkedSquars.Add(downSquare);

        return checkedSquars;
    }

    private bool IsSquareWalkAble(Squar sq)
    {
        if (sq != null && !sq.IsSquearBlocked && sq.UnitInSquar == null)
            return true;

        return false;
    }

    public List<Squar> GetTheAdjacentSquare(Squar centerSquar)
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
        if (centerSquar.yCoordinate + 1 >= _columnCount)
            rightSquare = null;
        else
            rightSquare = GetSquareFromGrid(centerSquar.xCoordinate, centerSquar.yCoordinate + 1);


        // check left direction
        if (centerSquar.yCoordinate - 1 < 0)
            leftSquare = null;
        else
            leftSquare = GetSquareFromGrid(centerSquar.xCoordinate, centerSquar.yCoordinate - 1);

        if (rightSquare != null)
            checkedSquars.Add(rightSquare);
        if (leftSquare != null)
            checkedSquars.Add(leftSquare);
        if (upSquare != null)
            checkedSquars.Add(upSquare);
        if (downSquare != null)
            checkedSquars.Add(downSquare);

        return checkedSquars;
    }

    public List<Squar> GetTheAdjacentMoveSquare(Squar centerSquar)
    {
        List<Squar> allAdjecentSquares = GetTheAdjacentSquare(centerSquar);
        List<Squar> moveSquars = new List<Squar>();

        foreach (var sq in allAdjecentSquares)
        {
            if (IsSquareWalkAble(sq) && !sq.isInMoveRange)
            {
                moveSquars.Add(sq);
                sq.isInMoveRange = true;
            }
        }

        return moveSquars;
    }

    public bool IsUnitInAttackRange(Squar targetSquare, List<Squar> unitRangeSquares)
    {
        foreach (Squar sq in unitRangeSquares)
        {
            if (targetSquare == sq)
                return true;
        }
        return false;
    }

    public List<Squar> GetTheAdjacentAttackSquare(Squar centerSquar, bool showAttackBorders = false)
    {
        List<Squar> checkedSquars = new List<Squar>();

        Squar upSquare = null;
        Squar downSquare = null;
        Squar rightSquare = null;
        Squar leftSquare = null;

        // check right direction
        if (centerSquar.xCoordinate + 1 >= _rowsCount)
            upSquare = null;
        else
            upSquare = GetSquareFromGrid(centerSquar.xCoordinate + 1, centerSquar.yCoordinate);

        // check left direction
        if (centerSquar.xCoordinate - 1 < 0)
            downSquare = null;
        else
            downSquare = GetSquareFromGrid(centerSquar.xCoordinate - 1, centerSquar.yCoordinate);

        // check up direction
        if (centerSquar.yCoordinate + 1 >= _columnCount)
            rightSquare = null;
        else
            rightSquare = GetSquareFromGrid(centerSquar.xCoordinate, centerSquar.yCoordinate + 1);

        // check down direction
        if (centerSquar.yCoordinate - 1 < 0)
            leftSquare = null;
        else
            leftSquare = GetSquareFromGrid(centerSquar.xCoordinate, centerSquar.yCoordinate - 1);

        if (showAttackBorders)
        {
            if (downSquare == null || !downSquare.isInAttackReach)
                centerSquar.downBorder.SetActive(true);

            if (upSquare == null || !upSquare.isInAttackReach)
                centerSquar.upBorder.SetActive(true);

            if (leftSquare == null || !leftSquare.isInAttackReach)
                centerSquar.leftBorder.SetActive(true);

            if (rightSquare == null || !rightSquare.isInAttackReach)
                centerSquar.rightBorder.SetActive(true);
        }
        else
        {
            if (upSquare != null && !upSquare.isInAttackReach)
                checkedSquars.Add(upSquare);
            if (downSquare != null && !downSquare.isInAttackReach)
                checkedSquars.Add(downSquare);
            if (rightSquare != null && !rightSquare.isInAttackReach)
                checkedSquars.Add(rightSquare);
            if (leftSquare != null && !leftSquare.isInAttackReach)
                checkedSquars.Add(leftSquare);

            centerSquar.isInAttackReach = true;

            foreach (Squar sq in checkedSquars)
            {
                sq.isInAttackReach = true;
            }
        }

        return checkedSquars;
    }

    // For now we are not using that..
    public List<Squar> GetTheAdjacentCrossSquare(Squar centerSquar)
    {
        List<Squar> checkedSquars = new List<Squar>();
  
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

        centerSquar.isInAttackReach = true;

        foreach (var item in checkedSquars)
        {
            item.isInAttackReach = true;
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
        bool isInside = IsSquareInsideBorders(x, y);

        Squar sq = _squaresInBattleField[x, y];
        return sq;
    }

    public Squar GetSquareFromGrid(Vector2Int vec)
    {
        bool isInside = IsSquareInsideBorders(vec.x, vec.y);

        Squar sq = _squaresInBattleField[vec.x, vec.y];
        return sq;
    }

    public Squar GetUnBlockedSquareFromGrid(int x, int y)
    {
        // Todo pro sledovaní erroru který neni ošetřen. Někdy se stava že je mimo hranice nevím proc.
        bool isPositionOutOfBorders = IsSquareInsideBorders(x, y);

        Squar sq = null;
        if(_squaresInBattleField[x, y].IsSquearBlocked || _squaresInBattleField[x, y].UnitInSquar != null)
        {
            Squar centerSquare = _squaresInBattleField[x, y];
            // choise other...
            while (sq == null)
            {
                List<Squar> adjactedSquares = GetTheAdjacentMoveSquare(centerSquare);
                foreach (Squar squar in adjactedSquares)
                {
                    if (!squar.IsSquearBlocked && squar.UnitInSquar == null)
                    {
                        sq = squar;
                        return sq;
                    }
                }

                centerSquare = adjactedSquares[Random.Range(0, adjactedSquares.Count)];
            }

            return sq;
        }
        else
        {
            return _squaresInBattleField[x, y];
        }
    }

    private bool IsSquareInsideBorders(int x, int y)
    {
        if(x <= this._rowsCount || y <= this._columnCount)
        {
            return true;
        }
        else
        {
            Debug.LogError("Square is out of borders pos -> " + x + "  " + y);
            Debug.LogError("Grid size -> rows / column " + _rowsCount + "  " + _columnCount);
            return false;
        }
    }

    public List<Vector2> GetPointsBetweenActiveUnitAndTargetSquare(Vector2 start, Vector2 end)
    {
        int distance = 20;
        bool direction = false;

        List<Vector2> points = new List<Vector2>();

        // no slope (vertical line)
        if (start.x == end.x)
        {
            for (float y = start.y; y <= end.y; y = y + distance)
            {
                Vector2 vec = new Vector2(start.x, y);
                points.Add(vec);
            }
        }
        else
        {
            // set direction -> according i substrack or add distance
            if ((start.x < end.x && start.y > end.y) || (start.x > end.x && start.y < end.y))
            {
                direction = true;
            }

            // swap p1 and p2 if p2.X < p1.X
            if (end.x < start.x)
            {
                Vector2 temp = start;
                start = end;
                end = temp;
            }

            float deltaX = end.x - start.x;
            float deltaY = end.y - start.y;
            float error = -1.0f;
            float deltaErr = System.Math.Abs(deltaY / deltaX);

            float y = start.y;
            for (float x = start.x; x <= end.x; x = x + distance)
            {
                Vector2 vec = new Vector2(x, y);
                points.Add(vec);
                error += deltaErr;

                if (direction)
                {
                    while (error >= 0.0f)
                    {
                        y = y - distance;
                        points.Add(new Vector2(x, y));
                        error -= 1.0f;
                    }
                }
                else
                {
                    while (error >= 0.0f)
                    {
                        y = y + distance;
                        points.Add(new Vector2(x, y));
                        error -= 1.0f;
                    }
                }
            }
        }

        return points;
    }

    public Vector2? SegmentIntersect(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        Vector2 intersectPoints = new Vector2(0, 0);
        float A1 = p1.y - p0.y;
        float B1 = p0.x - p1.x;
        float C1 = A1 * p0.x + B1 * p0.y;
        float A2 = p3.y - p2.y;
        float B2 = p2.x - p3.x;
        float C2 = A2 * p2.x + B2 * p2.y;
        float denominator = A1 * B2 - A2 * B1;

        if (denominator == 0)
        {
            return null;
        }

        float intersectX = (B2 * C1 - B1 * C2) / denominator;
        float intersectY = (A1 * C2 - A2 * C1) / denominator;
        float rx0 = (intersectX - p0.x) / (p1.x - p0.x);
        float ry0 = (intersectY - p0.y) / (p1.y - p0.y);

        float rx1 = (intersectX - p2.x) / (p3.x - p2.x);
        float ry1 = (intersectY - p2.y) / (p3.y - p2.y);

        if (((rx0 >= 0 && rx0 <= 1) || (ry0 >= 0 && ry0 <= 1)) &&
            ((rx1 >= 0 && rx1 <= 1) || (ry1 >= 0 && ry1 <= 1)))
        {
            intersectPoints.x = intersectX;
            intersectPoints.y = intersectY;
            return intersectPoints;
        }
        else
        {
            return null;
        }
    }

    public bool? DoesIntersectionPointsOnAdjacentSides(List<Vector2?> sidesList)
    {
        for (int i = 0; i < sidesList.Count - 1; i++)
        {
            if (sidesList[i] != null)
            {
                for (int k = i + 1; k < sidesList.Count; k++)
                {
                    if (sidesList[k] != null)
                        return Mathf.Abs(k - i) == 2 ? true : false;
                }
            }
        }

        return null;
    }

    public Vector2[] GetPointsOfTriangle(Vector3[] squareCorners, List<Vector2?> intersectPoints)
    {
        Vector2[] points = new Vector2[3];

        if (intersectPoints[0] != null && intersectPoints[1] != null)
        {
            points[0] = squareCorners[1];
            points[1] = (Vector2)intersectPoints[0];
            points[2] = (Vector2)intersectPoints[1];
            return points;

        }
        else if (intersectPoints[1] != null && intersectPoints[2] != null)
        {
            points[0] = squareCorners[2];
            points[1] = (Vector2)intersectPoints[1];
            points[2] = (Vector2)intersectPoints[2];
            return points;
        }
        else if (intersectPoints[2] != null && intersectPoints[3] != null)
        {
            points[0] = squareCorners[3];
            points[1] = (Vector2)intersectPoints[2];
            points[2] = (Vector2)intersectPoints[3];
            return points;
        }
        else
        {
            points[0] = squareCorners[0];
            points[1] = (Vector2)intersectPoints[3];
            points[2] = (Vector2)intersectPoints[0];
            return points;
        }
    }

    public float CalculateTriangeArea(Vector2 pointA, Vector2 pointB, Vector2 pointC)
    {
        // first Calculate Sides
        float BCx, BCy;
        BCx = Mathf.Pow(pointB.x - pointC.x, 2);
        BCy = Mathf.Pow(pointB.y - pointC.y, 2);
        float sideA = Mathf.Sqrt(BCx + BCy);

        float ACx, ACy;
        ACx = Mathf.Pow(pointA.x - pointC.x, 2);
        ACy = Mathf.Pow(pointA.y - pointC.y, 2);
        float sideB = Mathf.Sqrt(ACx + ACy);

        float ABx, ABy;
        ABx = Mathf.Pow(pointA.x - pointB.x, 2);
        ABy = Mathf.Pow(pointA.y - pointB.y, 2);
        float sideC = Mathf.Sqrt(ABx + ABy);

        // Now calculate Area but i dont have everything. I need triangle Perimeter and Semiperimeter 

        float perimetr = sideA + sideB + sideC;
        float semiperimeter = perimetr / 2;
        float area = Mathf.Sqrt(semiperimeter * (semiperimeter - sideA) * (semiperimeter - sideB) * (semiperimeter - sideC));

        return area;
    }

    // Parametrs must be adjected points othervise you will calculate hypotenuse like a side
    public float CalculateSquareArea(Vector2 pointA, Vector2 pointB)
    {
        float ABx, ABy;
        ABx = Mathf.Pow(pointA.x - pointB.x, 2);
        ABy = Mathf.Pow(pointA.y - pointB.y, 2);
        float side = Mathf.Sqrt(ABx + ABy);

        return side * side;
    }


    // Map Terrain Varianty
    private void GenerateRandomTerrain()
    {

        // vyber spravny druh terenu muže byt random
        TerrainVariants terrainVariaty = TerrainVariants.StoneElShape;

        switch (terrainVariaty)
        {
            case TerrainVariants.Normal:
                break;
            case TerrainVariants.StoneElShape:

                StoneElShapeGenerator generator = new StoneElShapeGenerator();
                generator.InitGenerator(_columnCount, _rowsCount, _obstaclesList);
                generator.GenerateStoneElShape();

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



 

    #region Enums

    public enum TerrainVariants
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

    #endregion
}
