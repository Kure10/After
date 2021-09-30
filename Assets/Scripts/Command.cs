using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Buildings;
using Resources;
using UnityEngine;

public abstract class Command
{
    internal GameObject Target;
    public abstract Result Execute();
}

public enum Result
{
    Success,
    Failure,
    Running
}

public class MoveOutside : Move
{
    
    public MoveOutside(GameObject target, List<Vector2Int> path, bool ignoreMark = true) : base(target, path, ignoreMark)
    {
        
    }
}

public class Move : Command
{
    public List<Vector2Int> Path;
    private float accumulatedTime = 0f;
    private TimeControl tc;
    private TileFactory tf;
    private float speed = 10f;
    private Vector3 startingPoint;
    private bool init;
    private bool ignoreMark;
    private static readonly int IsMoving = Animator.StringToHash("IsWalking");

    public Move(GameObject target, List<Vector2Int> path, bool ignoreMark = false)
    {
        Target = target;
        tc = GameObject.FindGameObjectWithTag("TimeController").GetComponent<TimeControl>();
        tf = GameObject.FindGameObjectWithTag("TileFactory").GetComponent<TileFactory>();
        Path = path;
        init = true;
        this.ignoreMark = ignoreMark;
    }
    //returns true if completed and can move to another command
    public override Result Execute()
    {
        if (init)
        {
            init = false;
            startingPoint = Target.transform.position;
            accumulatedTime = 0;
            tf.LeaveTile(Geometry.GridFromPoint(startingPoint));
            var animator = Target.GetComponent<Animator>();
            animator.SetBool(IsMoving, true);
        }
        accumulatedTime += Time.deltaTime * tc.TimePointMultiplier();
        //in case of more then one time point from last frame, skip rendering of previous actions
        while (accumulatedTime > speed)
        {
            if (Path.Count > 0)
            {
                startingPoint = Geometry.PointFromGrid(Path[0]);
                Target.transform.position = startingPoint;
                Path.RemoveAt(0);
            }

            accumulatedTime -= speed;
            if (accumulatedTime < 0)
            {
                //float numbers overflow check
                accumulatedTime = 0f;
            }
        }
        if (Path.Count > 0)
        {
            var nextVector3 = Geometry.PointFromGrid(Path[0]);
            Step(nextVector3);
            return Result.Running;
        }

        if (ignoreMark)
        {
            Target.GetComponent<Animator>().SetBool(IsMoving, false);
            return Result.Success;
        }
        Vector2Int mark = Geometry.GridFromPoint(Target.transform.position);
        if (tf.OccupyTile(mark))
        {
            Target.GetComponent<Animator>().SetBool(IsMoving, false);
            return Result.Success;
        }
        
        var forbiddenTiles = tf.GetOccupiedTiles();
            var newPlace = tf.FindFreeTile(mark, forbiddenTiles, true);
            if (newPlace.Count > 0)
            {
                Path = tf.FindPath(mark, newPlace.First());
                foreach (var cand in newPlace)
                {
                    var shorter = tf.FindPath(mark, cand);
                    if (shorter.Count < Path.Count)
                    {
                        Path = shorter;
                    }
                }

                return Result.Running;
            }

            return Result.Failure;
    }
    void Step(Vector3 to)
    {
        Target.transform.position = Vector3.Lerp(startingPoint, to, accumulatedTime / speed);
    }

}
public class PickUp : Command
{
    private ResourceManager.ResourceAmount howMany;
    public PickUp(GameObject target, ResourceManager.ResourceAmount howMany)
    {
        Target = target;
        this.howMany = howMany;

    }
    public override Result Execute()
    {
        var resourceManager = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceManager>();
        var tileFactory = GameObject.FindGameObjectWithTag("TileFactory").GetComponent<TileFactory>();
        var pickupPoint = tileFactory.getTile(Geometry.GridFromPoint(Target.transform.position));
        if (pickupPoint is IResourceHolder tile)
        {
            var character = Target.GetComponent<Character>() as IResourceHolder;
            var pickedUp = tile.Remove(howMany);
            if (pickedUp.Empty())
            {
                //maybe not empty tile, but warehouse
                if (tile is Tile t)
                {
                    if (t.building is Warehouse w)
                    {
                        pickedUp = w.Remove(howMany);
                        if (pickedUp.Empty()) return Result.Failure;
                        var s = character.Add(pickedUp);
                        w.Add(s);
                        return Result.Success;
                    }
                }
                else
                {
                    return Result.Failure;
                }
            }
            var surplus = character.Add(pickedUp);
            tile.Add(surplus);            
            return Result.Success;
        }
        return Result.Failure;
    }
}
public class Drop : Command
{
    private readonly ResourceManager resourceManager;
    private readonly TileFactory tileFactory;
    public Drop(GameObject target)
    {
        Target = target;
        tileFactory = GameObject.FindGameObjectWithTag("TileFactory").GetComponent<TileFactory>();
        resourceManager = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceManager>();
    }
    public override Result Execute()
    {
        Character owner = Target.GetComponent<Character>();
        if (owner == null) return Result.Failure;
        var dropPoint = tileFactory.getTile(Geometry.GridFromPoint(Target.transform.position));
        if (!(dropPoint is Tile t)) return Result.Failure;
        IResourceHolder target = null;
        if (t.building != null)
        {
            if(t.building is IResourceHolder rh)
            {
                target = rh;
            }
        }
        else
        {
            target = tileFactory.getTile(tileFactory.FindFreeTile(Geometry.GridFromPoint(Target.transform.position)).First()) as IResourceHolder;
        }

        if (target is null) return Result.Failure;
        var surplus = target.Add(owner.Amount);
        owner.Set(surplus);
        if (!surplus.Empty())
        {
            return Result.Failure;
        }
        return Result.Success;
    }
}

public class Build : Command
{
    private float accumulatedTime;
    private TimeControl tc;
    public Build()
    {
        accumulatedTime = 0f;
        tc = GameObject.FindGameObjectWithTag("TimeController").GetComponent<TimeControl>();
    }
    public override Result Execute()
    {
        accumulatedTime += Time.deltaTime * tc.TimePointMultiplier();
        
        return Result.Success;
    }

    public float GetBuildPoints(int techLevel)
    {
        var ret = accumulatedTime + techLevel;
        accumulatedTime = 0;
        return ret;
    }
}