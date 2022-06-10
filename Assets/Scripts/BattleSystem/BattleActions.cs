using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public partial class BattleController
{
    // Selecting Battle Actions
    private BattleAction OnClickIntoGrid(Squar actionSquare)
    {
        BattleAction action = BattleAction.None;

        if (actionSquare.UnitInSquar != null)
        {
            bool isRange = false;
            bool isFriendlyUnit = actionSquare.UnitInSquar._team == _activeUnit._team;

            if (_activeUnit.ActiveWeapon != null)
                isRange = !_activeUnit.ActiveWeapon.IsMelleWeapon;

            if (!isFriendlyUnit && _squaresInUnitAttackRange.Contains(actionSquare))
            {
                if (isRange && TryGetAim(actionSquare))
                    return action = BattleAction.RangeAttack;


                action = BattleAction.Attack;
            }

            if (isFriendlyUnit && _squaresInUnitAttackRange.Contains(actionSquare))
            {
                action = BattleAction.Heal;
            }

            return action;
        }
        else
        {
            if (!actionSquare.IsSquearBlocked && _squaresInUnitMoveRange.Contains(actionSquare))
            {
                action = BattleAction.Move;
            }

            return action;
        }
    }

    private bool TryGetAim(Squar targetSquare, bool debug = false)
    {
        bool tryAim = true;

        if (!_battleGridController.IsUnitInAttackRange(targetSquare, _squaresInUnitAttackRange))
            return false;

        if (targetSquare == null && debug)
        {
            foreach (var sq in shootPathSq)
            {
                sq.TestingShowShootPath(false);
                sq.TestingShowShootPathLesserThan(false);
                sq.TestingShowShootPathNopoints(false);
                shootPathSq.Clear();
            }
        }
        else
        {
            if (debug)
            {
                foreach (var sq in shootPathSq)
                {
                    sq.TestingShowShootPath(false);
                    sq.TestingShowShootPathLesserThan(false);
                    sq.TestingShowShootPathNopoints(false);
                }
            }

            shootPathSq.Clear();
            Squar center = _battleGridController.GetSquareFromGrid(_activeUnit.CurrentPos.XPosition, _activeUnit.CurrentPos.YPosition);
            Vector3[] startRectCornrs = new Vector3[4];
            Vector3[] endRectCornrs = new Vector3[4];

            center.GetComponent<RectTransform>().GetWorldCorners(startRectCornrs);
            targetSquare.GetComponent<RectTransform>().GetWorldCorners(endRectCornrs);

            Vector2 startSqCenter = new Vector2((startRectCornrs[0].x + startRectCornrs[3].x) / 2, (startRectCornrs[0].y + startRectCornrs[1].y) / 2);
            Vector2 endSqCenter = new Vector2((endRectCornrs[0].x + endRectCornrs[3].x) / 2, (endRectCornrs[0].y + endRectCornrs[1].y) / 2);

            List<Vector2> points = _battleGridController.GetPointsBetweenActiveUnitAndTargetSquare(startSqCenter, endSqCenter);
            shootPathSq.AddRange(RaycastPointsOnGrid(points));

            // Todo mohl bych udelat check jestli prvni Sq je muj SQ na kterem stojím a poslední je SQ do kterého strilim
            if (shootPathSq.Count > 1)
            {
                shootPathSq.RemoveAt(shootPathSq.Count - 1);
                shootPathSq.RemoveAt(0);
            }

            Vector3[] squareCorners = new Vector3[4];

            foreach (Squar sq in shootPathSq)
            {
                List<Vector2?> intersectPoints = new List<Vector2?>();
                sq.GetComponent<RectTransform>().GetWorldCorners(squareCorners);

                intersectPoints.Add(_battleGridController.SegmentIntersect(startSqCenter, endSqCenter, squareCorners[0], squareCorners[1]));
                intersectPoints.Add(_battleGridController.SegmentIntersect(startSqCenter, endSqCenter, squareCorners[1], squareCorners[2]));
                intersectPoints.Add(_battleGridController.SegmentIntersect(startSqCenter, endSqCenter, squareCorners[2], squareCorners[3]));
                intersectPoints.Add(_battleGridController.SegmentIntersect(startSqCenter, endSqCenter, squareCorners[3], squareCorners[0]));

                bool? result = _battleGridController.DoesIntersectionPointsOnAdjacentSides(intersectPoints);

                //Body tvoří nepravidelný čtyřuhelník
                if (result == true)
                {
                    //Debug.Log("Je nepravidelny čtyřuhelnik -> " + sq.xCoordinate + "  " + sq.yCoordinate);
                    if (!sq.CanShootThrough)
                    {
                        tryAim = false;
                    }
                }
                else if (result == false) // pokud sousedí trjhelnik
                {
                    if (!sq.CanShootThrough)
                    {
                        Vector2[] pointsOfTriangle = _battleGridController.GetPointsOfTriangle(squareCorners, intersectPoints);
                        float triangleArea = _battleGridController.CalculateTriangeArea(pointsOfTriangle[0], pointsOfTriangle[1], pointsOfTriangle[2]);
                        float squareArea = _battleGridController.CalculateSquareArea(squareCorners[0], squareCorners[1]);

                        if (triangleArea < (squareArea * 0.33))
                        {
                            List<Squar> adjectedSquares = _battleGridController.GetTheAdjacentSquare(sq);
                            foreach (var item in adjectedSquares)
                            {
                                if (shootPathSq.Contains(item) && !sq.CanShootThrough)
                                {
                                    tryAim = false;
                                }
                            }

                            //var direction = GetAimDirection(startSqCenter, endSqCenter);
                            // Debug.Log("Je mensi triangle než 33% -> " + sq.xCoordinate + "  " + sq.yCoordinate);
                            if (debug)
                                sq.TestingShowShootPathLesserThan(true);
                        }
                        else
                        {
                            // Debug.Log("Je mensi triangle -> " + sq.xCoordinate + "  " + sq.yCoordinate);
                        }
                    }


                }
                //else
                //{
                //   // Debug.LogError($"Error! Calculation of intersect Points failed on square: {sq.xCoordinate} {sq.yCoordinate}");

                //    //Todo tento druh čtverce zatím neřeším
                if (debug)
                    sq.TestingShowShootPathNopoints(true);
                //}
            }

            if (debug)
            {
                foreach (var sq in shootPathSq)
                {
                    sq.TestingShowShootPath(true);
                }
            }

        }

        if (debug)
            Debug.Log("Attack status range can shoot ? :  " + tryAim);
        return tryAim;
    }

    // Performing Battle Actions

    public async Task Move(Squar targetSquare)
    {
        IsPerformingAction = true;

        Squar startingPosition = GetBattleGridController.GetSquareFromGrid(GetActiveUnit.CurrentPos.XPosition, GetActiveUnit.CurrentPos.YPosition);
        Squar endPosition = GetBattleGridController.GetSquareFromGrid(targetSquare.GetCoordinates());
        List<Squar> finalPath = GetBattkePathFinder.FindPath(startingPosition, endPosition);

        await Task.WhenAll(GetBattleGridController.MoveToPathAsync(GetActiveUnit, finalPath, GetDelayWalk, GetSquaresInMoveRange, GetSquaresInAttackRange, this));
    }

    // Melee and Range is same for now .. Maybe will be who knows
    private void MelleAttack(Unit targetUnit)
    {
        AttackInfo attackInfo = null;
        attackInfo = AttackToUnit(_activeUnit, targetUnit);

        if (attackInfo.unitDied)
        {
            _battleGridController.DestroyUnitFromBattleField(targetUnit);
            _battleInfoPanel.DeleteUnitFromOrder(targetUnit);
            _battleLog.AddBattleLog($"{targetUnit._name} is dead");
        }

        _battleLog.AddAttackBattleLog(attackInfo, _activeUnit, targetUnit);
        _battleInfoPanel.UpdateUnitData(targetUnit);
        
    }

    public AttackInfo AttackToUnit(Unit attackingUnit, Unit defendingUnit)
    {
        AttackInfo attackInfo = new AttackInfo();

        int dices = BattleSystem.CalculateAmountDices(attackingUnit);
        int success = BattleSystem.CalculateAmountSuccess(dices, attackingUnit, defendingUnit, out attackInfo.dicesValueRoll);

        defendingUnit.ReceivedDamage(success);
        attackInfo.unitDied = defendingUnit.CheckIfUnitIsNotDead();

        // for info
        attackInfo.dices = dices;
        attackInfo.success = success;

        return attackInfo;
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
}
