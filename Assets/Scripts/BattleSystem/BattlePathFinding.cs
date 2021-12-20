using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BattlePathFinding 
{
    public void EvaluateGridCost(Squar startSquare, Squar endSquare)
    {
        foreach (Squar square in BattleGridController.GetSquarsFromBattleField)
        {
            square.PathStats.CalculateCosts(square, startSquare, endSquare);
        }
    }



    public List<Squar> FindPath ()
    {
        List<Squar> sq = new List<Squar>();






        return sq;
    }

    public class AAlgoritmStats
    {
        public int GCost = 0; // how far is squer from start node (10 point traverse , 14 cross)
        public int HCost = 0; //  How far is squer from end node (10 point traverse , 14 cross)
        public int FCost = 0; // G + H

        public void CalculateCosts(Squar thisSquare,Squar startSquare, Squar targetSquare)
        {
            CalcGCost(thisSquare, startSquare);
            CalcHCost(thisSquare, targetSquare);
            CalcFCost();
        }

        public void CalcGCost(Squar thisSquare, Squar startSquare)
        {
           GCost = 10 * (Math.Abs(thisSquare.xCoordinate - startSquare.xCoordinate) + Math.Abs(thisSquare.yCoordinate - startSquare.yCoordinate));
        }

        public void CalcHCost(Squar thisSquare, Squar endSquare)
        {
            GCost = 10 *  (Math.Abs(thisSquare.xCoordinate - endSquare.xCoordinate) + Math.Abs(thisSquare.yCoordinate - endSquare.yCoordinate);
        }

        public void CalcFCost()
        {
           FCost = GCost + HCost;
        }
    }
}
