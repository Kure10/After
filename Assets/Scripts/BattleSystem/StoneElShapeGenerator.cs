using UnityEditor;
using UnityEngine;
using static BattleGridController;

namespace Assets.Scripts.BattleSystem
{
    public class StoneElShapeGenerator : RandomTerrainGenerator
    {

        private int _groupStoneBlockCount = 0;
        private int _groupStoneSize = 0;
        private int _stoneCount = 5;

        public void GenerateStoneElShape()
        {
            int countElShapes = 2; // Kolik utvaru se spawnuje
            int size = 6;          // celkova velikost utvaru na pocet kostek

            bool isOnRight = false;
            bool isOnTop = false;
            int longSize = Mathf.RoundToInt((size * 0.66f));
            int shortSize = size - longSize;

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

                if (!wasTriggeredSafeCounter)
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


    }
}