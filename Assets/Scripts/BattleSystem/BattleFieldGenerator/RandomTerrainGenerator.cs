using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleGridController;

namespace Assets.Scripts.BattleSystem
{
    public class RandomTerrainGenerator
    {
        protected int _columnCount;
        protected int _rowsCount;

        protected int _maxStoneInRow = 0;
        protected int _maxStoneInColumn = 0;

        protected List<GameObject> _obstaclesList = new List<GameObject>();

        public void InitGenerator(int column, int rows , List<GameObject> obstaclesList)
        {
            _columnCount = column;
            _rowsCount = rows;

            _maxStoneInColumn = column - 2;
            _maxStoneInRow = rows - 2;

            _obstaclesList.Clear();
            foreach (GameObject obs in obstaclesList)
            {
                _obstaclesList.Add(obs);
            }
        }


        protected bool CreateStone(int x, int y) // x pos , y pos
        {
            bool isInsideBorders = IsInsideBorder(x,y);  // This condition prevent stone Be out Of battleField
            bool maxLimitCondition = CheckMaxLimitCondition(x, y, isInsideBorders);  // This condition prevent stones blocked road from one side to other.

            if (isInsideBorders && maxLimitCondition)
            {
                Squar squar = BattleGridController.GetSquarsFromBattleField[x, y];

                if (!squar.IsSquearBlocked)
                {
                    // Create stone. In future it will be good if stone prefab will be difretent.
                    GameObject prefab = ChoiseObstacle();
                    squar.SetObstacle(prefab);
                    return true;
                }
            }
            else
            {
                Debug.LogError("I tryed create block out of condition border");
                Debug.LogError(" x: " + x + " y: " + y);
            }

            return false;
        }

        private bool CheckMaxLimitCondition(int x, int y, bool isInsideBorders)
        {
            bool maxLimitCondition = false;

            if (isInsideBorders)
            {
                int columnRockCounter = 0;
                int rowRockCounter = 0;

                for (int i = 0; i < _rowsCount - 1; i++)
                {
                    if (BattleGridController.GetSquarsFromBattleField[i, y].IsSquearBlocked)
                    {
                        rowRockCounter++;
                    }
                }

                for (int i = 0; i < _columnCount - 1; i++)
                {
                    if (BattleGridController.GetSquarsFromBattleField[x, i].IsSquearBlocked)
                    {
                        columnRockCounter++;
                    }
                }

                maxLimitCondition = rowRockCounter <= _maxStoneInRow && columnRockCounter <= _maxStoneInColumn;
            }

            return maxLimitCondition;
        }

        private bool IsInsideBorder (int x, int y)
        {
            return x < _rowsCount && x >= 0 && y >= 0 && y < _columnCount;
        }

        // in Progresss
        private GameObject ChoiseObstacle()
        {
            int rng = Random.Range(0, _obstaclesList.Count);
            GameObject ob = _obstaclesList[rng];
            return ob;
        }
    }
}