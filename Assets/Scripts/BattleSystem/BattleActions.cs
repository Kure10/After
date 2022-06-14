using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public partial class BattleController
{
    // Selecting Battle Actions
    private BattleAction OnClickIntoGrid(Squar actionSquare)
    {
        IClickAble clickAbleObject = actionSquare.GetObjectFromSquareGeneric<IClickAble>();
        if (clickAbleObject == null)
        {
           return ChoiseUtilityAction(actionSquare);
        }
           
        if(clickAbleObject is Unit targetUnit)
        {
            return ChoiseUnitAction(actionSquare, targetUnit);
        }
        else if (clickAbleObject is Obstacle obstacle)
        {
            return ChoiseObstacleAction(actionSquare, obstacle);
        }

        return BattleAction.None;
    }

    private BattleAction ChoiseUtilityAction(Squar actionSquare)
    {
        BattleAction action = BattleAction.None;

        if (!actionSquare.IsSquearBlocked && _squaresInUnitMoveRange.Contains(actionSquare))
        {
            action = BattleAction.Move;
        }

        return action;
    }

    private BattleAction ChoiseObstacleAction(Squar targetSquare, Obstacle obstacle)
    {
        BattleAction action = BattleAction.None;
        if (obstacle is IDamageable damageAble)
        {
            bool isRange = false;
            if (_activeUnit.ActiveWeapon != null)
                isRange = !_activeUnit.ActiveWeapon.IsMelleWeapon;

            if (_squaresInUnitAttackRange.Contains(targetSquare))
            {
                if (isRange && TryGetAim(targetSquare))
                    return action = BattleAction.RangeAttack;

                return action = BattleAction.Attack;
            }
        }
        else
        {
            action = BattleAction.None;
        }

        return action;
    }

    private BattleAction ChoiseUnitAction(Squar actionSquare, Unit targetUnit)
    {
        BattleAction action = BattleAction.None;
        bool isRange = false;
        bool isFriendlyUnit = targetUnit._team == _activeUnit._team;

        if (_activeUnit.ActiveWeapon != null)
            isRange = !_activeUnit.ActiveWeapon.IsMelleWeapon;

        if (!isFriendlyUnit && _squaresInUnitAttackRange.Contains(actionSquare))
        {
            if (isRange && TryGetAim(actionSquare))
                return action = BattleAction.RangeAttack;

            return action = BattleAction.Attack;
        }

        if (isFriendlyUnit && _squaresInUnitAttackRange.Contains(actionSquare))
        {
           return action = BattleAction.Heal;
        }

        return action;
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
            Squar center = _battleGridController.GetSquareFromGrid(_activeUnit.GetXPosition, _activeUnit.GetYPosition);
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

        Squar startingPosition = GetBattleGridController.GetSquareFromGrid(GetActiveUnit.GetXPosition, GetActiveUnit.GetYPosition);
        Squar endPosition = GetBattleGridController.GetSquareFromGrid(targetSquare.GetCoordinates());
        List<Squar> finalPath = GetBattkePathFinder.FindPath(startingPosition, endPosition);

        await Task.WhenAll(GetBattleGridController.MoveToPathAsync(GetActiveUnit, finalPath, GetDelayWalk, GetSquaresInMoveRange, GetSquaresInAttackRange, this));
    }

    // Melee and Range is same for now .. Maybe will be who knows
    private void MelleAttack(IClickAble targetObject)
    {
        AttackInfo attackInfo = null;

        if(targetObject is Unit targetUnit)
        {
            attackInfo = AttackToUnit(_activeUnit, targetUnit);

            if (attackInfo.wasTargetDestroyed)
            {
                _battleGridController.DestroyUnitFromBattleField(targetUnit);
                _battleInfoPanel.DeleteUnitFromOrder(targetUnit);
                _battleLog.AddBattleLog($"{targetUnit.GetName} is dead");
            }

            _battleLog.AddAttackBattleLog(attackInfo, _activeUnit, targetUnit);
            _battleInfoPanel.UpdateUnitData(targetUnit);
        }

        if(targetObject is Obstacle obstacle)
        {
            if(obstacle is IDamageable damagable)
            {
                attackInfo = AttackToObstacle(_activeUnit, damagable);
            }

            if (attackInfo.wasTargetDestroyed)
            {
                if(obstacle is DestroyAbleObstacle destroAble && destroAble.IsExplosive)
                {
                    ExplosionAction(destroAble);
                }

                _battleGridController.DestroyObstacleFromBattleField(obstacle);
                _battleLog.AddBattleLog($"{obstacle.GetName} was destroyed");
            }

            _battleLog.AddAttackBattleLog(attackInfo, _activeUnit, obstacle);
        }
    }

    public AttackInfo AttackToUnit(Unit attackingUnit, Unit defendingUnit)
    {
        AttackInfo attackInfo = new AttackInfo();

        int dices = BattleSystem.CalculateAmountDices(attackingUnit);
        int success = BattleSystem.CalculateAmountSuccess(dices, attackingUnit, defendingUnit, out attackInfo.dicesValueRoll);

        defendingUnit.ReceivedDamage(success);
        attackInfo.wasTargetDestroyed = defendingUnit.IsBattleObjectDead();

        // for info
        attackInfo.dices = dices;
        attackInfo.success = success;

        return attackInfo;
    }

    public AttackInfo AttackToObstacle(Unit attackingUnit, IDamageable obstacle)
    {
        AttackInfo attackInfo = new AttackInfo();
        int dices = BattleSystem.CalculateAmountDices(attackingUnit);
        int success = BattleSystem.CalculateAmountSuccess(dices, attackingUnit, obstacle, out attackInfo.dicesValueRoll);

        obstacle.ReceivedDamage(success);
        attackInfo.wasTargetDestroyed = obstacle.IsBattleObjectDead();

        // for info
        attackInfo.dices = dices;
        attackInfo.success = success;

        return attackInfo;
    }

    private void ExplosionAction(DestroyAbleObstacle obstacle)
    {
        Squar centerSquare = _battleGridController.GetSquareFromGrid(obstacle.GetXPosition,obstacle.GetYPosition);
        List<Squar> adjectiveSq = _battleGridController.GetTheAdjacentSquare(centerSquare);

        List<Squar> squaresInExplodeRange = new List<Squar>();
        squaresInExplodeRange.AddRange(adjectiveSq);

        for (int i = 1; i < obstacle.GetExplosiveRange; i++)
        {
            List<Squar> adjectiveQquaresInOneCycle = new List<Squar>();

            foreach (Squar sq in adjectiveSq)
            {
                List<Squar> adjectiveForOneSq = _battleGridController.GetTheAdjacentSquare(sq);

                foreach (Squar square in adjectiveForOneSq)
                {
                    if (!squaresInExplodeRange.Contains(square) && square != centerSquare)
                    {
                        squaresInExplodeRange.Add(square);
                        adjectiveQquaresInOneCycle.Add(square);
                    }
                }
            }

            adjectiveSq.Clear();
            adjectiveSq.AddRange(adjectiveQquaresInOneCycle);
        }

        // PerforExplosion on choisen squares
        foreach (Squar sqInExplosiveRange in squaresInExplodeRange)
        {
            IDamageable damagable = sqInExplosiveRange.GetObjectFromSquareGeneric<IDamageable>();
            if(damagable != null)
                damagable.ReceivedDamage(obstacle.GetExplosiveDamage);
        }
    }

   

    #region HelperClass

    public class AttackInfo
    {
        public int dices = 0;
        public int success = 0;

        public bool wasTargetDestroyed = false;

        public List<int> dicesValueRoll = new List<int>();
    }

    #endregion
}
