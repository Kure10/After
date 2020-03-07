using System.Collections;
using System.Collections.Generic;
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

public class Move : Command
{
    public List<Vector2Int> Path;
    private float accumulatedTime = 0f;
    private TimeControl tc;
    private TileFactory tf;
    private float speed = 10f;
    private Vector3 startingPoint;
    private bool init;
    
    public Move(GameObject target, List<Vector2Int> path)
    {
        Target = target;
        tc = GameObject.FindGameObjectWithTag("TimeController").GetComponent<TimeControl>();
        tf = GameObject.FindGameObjectWithTag("TileFactory").GetComponent<TileFactory>();
        Path = path;
        init = true;
    }
    //returns true if completed and can move to another command
    public override Result Execute()
    {
        if (init)
        {
            init = false;
            startingPoint = Target.transform.position;
            accumulatedTime = 0;
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
        return Result.Success;
    }
    void Step(Vector3 to)
    {
        Target.transform.position = Vector3.Lerp(startingPoint, to, accumulatedTime / speed);
    }

}
public class PickUp : Command
{
    public PickUp(GameObject target)
    {
        Target = target;
    }
    public override Result Execute()
    {
        var resourceManager = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceManager>();
        var resource = resourceManager.PickUp(Geometry.GridFromPoint(Target.transform.position));
        if (resource == null) return Result.Failure;
        var character = Target.GetComponent<Character>();
        resource.Owner = character;
        return Result.Success;
    }
}
public class Drop : Command
{
    public Drop(GameObject target)
    {
        Target = target;
    }
    public override Result Execute()
    {
        var resourceManager = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceManager>();
        var resource = resourceManager.GetResource(Target.GetComponent<Character>());
        if (resource == null) return Result.Failure;
        var tf = GameObject.FindGameObjectWithTag("TileFactory").GetComponent<TileFactory>();
        var dropPoint = tf.getTile(Geometry.GridFromPoint(Target.transform.position));
        if (!(dropPoint is Tile t)) return Result.Failure;
        if (t.building != null)
        {
            resource.Owner = t.building;
        }
        else
        {
            resource.Owner = t;
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
        var ret = (accumulatedTime / 10f + techLevel) * 10f;
        Debug.Log(accumulatedTime + "Accumulated time");
        accumulatedTime = 0;
        return ret;
    }
}