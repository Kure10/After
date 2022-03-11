using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BattlePathFinding 
{
    const int CROSS_COST = 14;
    const int DIAGONAL_COST = 10;

    private BattleGridController _battleGridController;
    public BattlePathFinding(BattleGridController bgc)
    {
        _battleGridController = bgc;
    }

    public void EvaluateGridCost(Squar startSquare, Squar endSquare)
    {
        foreach (Squar square in BattleGridController.GetSquarsFromBattleField)
        {
            square.PathStats.CalculateCosts(square, startSquare, endSquare);

            // for testing purpose
            square.ShowCosts(square.PathStats);
        }
    }

    public List<Squar> FindPath (Squar startingPoint, Squar endPoint)
    {
        Squar nextSquer = startingPoint;
        List<Squar> closeSquares = new List<Squar>();
        List<Squar> openSquares = new List<Squar>();
        List<Squar> finalPath = new List<Squar>();

        while (nextSquer != endPoint)
        {

            List<Squar> adjectedSearchSquers = _battleGridController.GetTheAdjacentWalkAbleSquare(nextSquer);

            foreach (Squar sq in adjectedSearchSquers)
            {
                if(!openSquares.Contains(sq))
                {
                    openSquares.Add(sq);
                    sq.PathStats.PreviousSquare = nextSquer;
                }
            }

            // Todo There should be aclucalation for Squares Cost

            Squar closesSquare = FindClosestSquareOnPath(openSquares, closeSquares);

            if (closesSquare == null)
            {
                Debug.LogError("There is no path to end Square");
                return null;
            }

            closeSquares.Add(closesSquare);
            nextSquer = closesSquare;

            //if (closesSquare == endPoint)
            //    return closeSquares; 
        }

        // Creating a Path from collected squears 
        Squar previusSquare = closeSquares[closeSquares.Count - 1];
        finalPath.Add(previusSquare);
        while (startingPoint != previusSquare)
        {
            finalPath.Add(previusSquare.PathStats.PreviousSquare);
            previusSquare = previusSquare.PathStats.PreviousSquare;
        }
        finalPath.Reverse();
        finalPath.Remove(startingPoint);

        return finalPath;
    }

    private Squar FindClosestSquareOnPath(List<Squar> openSquares , List<Squar> closeSquares)
    {
        Squar closestSquare = null;

        List<Squar> searchingSquares = new List<Squar>();

        foreach (Squar sq in openSquares)
        {
            if(!closeSquares.Contains(sq))
                searchingSquares.Add(sq);
        }

        List<Squar> lowestFCostSquares = TakeLowerFCostSquares(searchingSquares);
        List<Squar> lowestHCostSquares = TakeLowerHCostSquares(lowestFCostSquares);

        if(lowestHCostSquares.Count > 0)
        {
            int i = UnityEngine.Random.Range(0, lowestHCostSquares.Count);
            closestSquare = lowestHCostSquares[i];
        }

        return closestSquare;
    }

    private List<Squar> TakeLowerFCostSquares(List<Squar> squares)
    {
        List<Squar> lowestFCostSquares = new List<Squar>();

        int min = 0;

        if (squares.Count > 0)
        {
            min = squares[0].PathStats.FCost;
            lowestFCostSquares.Add(squares[0]);
            squares.Remove(squares[0]);
        }

        foreach (Squar sq in squares)
        {
           
            if (sq.PathStats.FCost < min)
            {
                lowestFCostSquares.Clear();
                min = sq.PathStats.FCost;
                lowestFCostSquares.Add(sq);
                continue;
            }

            if (sq.PathStats.FCost == min)
            {
                lowestFCostSquares.Add(sq);
                continue;
            }
        }

        return lowestFCostSquares;
    }

    private List<Squar> TakeLowerHCostSquares(List<Squar> squares)
    {
        List<Squar> lowestHCostSquares = new List<Squar>();

        int min = 0;

        if (squares.Count > 0)
        {
            lowestHCostSquares.Add(squares[0]);
            min = squares[0].PathStats.HCost;
            squares.Remove(squares[0]);
        }

        foreach (Squar sq in squares)
        {
            if (sq.PathStats.HCost < min)
            {
                lowestHCostSquares.Clear();
                min = sq.PathStats.FCost;
                lowestHCostSquares.Add(sq);
                continue;
            }

            if (sq.PathStats.HCost == min)
            {
                lowestHCostSquares.Add(sq);
                continue;
            }
        }
        return lowestHCostSquares;
    }

    // Testing testing
    private bool IsSquareOnDiagonalPath(Squar centerSquare, Squar posibleSQ)
    {
        int xResult = Mathf.Abs(centerSquare.xCoordinate - posibleSQ.xCoordinate);
        int yResult = Mathf.Abs(centerSquare.yCoordinate - posibleSQ.yCoordinate);

        if (xResult == yResult)
            return true;

        return false;
    }

    private bool IsSquareOnStraithPath(Squar centerSquare, Squar posibleSQ)
    {
        if (centerSquare.xCoordinate == posibleSQ.xCoordinate || centerSquare.yCoordinate == posibleSQ.yCoordinate)
        {
            return true;
        }

        return false;
    }
    /// <summary>
    /// ////
    /// </summary>

    public class AAlgoritmStats
    {
        public Squar PreviousSquare = null;

        public int GCost = 0; // how far is squer from start node (10 point traverse , 14 cross)
        public int HCost = 0; //  How far is squer from end node (10 point traverse , 14 cross)
        public int FCost = 0; // G + H

        public void CalculateCosts(Squar thisSquare, Squar startSquare, Squar targetSquare)
        {
            CalcGCost(thisSquare, startSquare);
            CalcHCost(thisSquare, targetSquare);
            CalcFCost();
        }

        private void CalcGCost(Squar thisSquare, Squar startSquare)
        {
            GCost = DIAGONAL_COST * (Math.Abs(thisSquare.xCoordinate - startSquare.xCoordinate) + Math.Abs(thisSquare.yCoordinate - startSquare.yCoordinate));
        }

        private void CalcHCost(Squar thisSquare, Squar endSquare)
        {
            HCost = DIAGONAL_COST * (Math.Abs(thisSquare.xCoordinate - endSquare.xCoordinate) + Math.Abs(thisSquare.yCoordinate - endSquare.yCoordinate));
        }

        private void CalcFCost()
        {
            FCost = GCost + HCost;
        }
    }

}
