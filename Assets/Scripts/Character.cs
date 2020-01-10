using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    // Start is called before the first frame update
    List<Vector2Int> target;
    private TimeControl tc;
    private float speed = 10f;
    private float accumulatedTime;
    private Vector3 startingPoint;
    private TileFactory tf;
    private ResourceManager resourceManager;
    void Start()
    {
        startingPoint = transform.position;
        target = new List<Vector2Int>();
        tc = GameObject.FindGameObjectWithTag("TimeController").GetComponent<TimeControl>();
        tf = GameObject.FindGameObjectWithTag("TileFactory").GetComponent<TileFactory>();
        resourceManager = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        accumulatedTime += Time.deltaTime * tc.TimePointMultiplier();
        //in case of more then one time point from last frame, skip rendering of previous actions
        while (accumulatedTime > speed)
        {
            Execute();
        }

        if (target.Count > 0)
        {
            var nextVector3 = Geometry.PointFromGrid(target[0]);
            Step(nextVector3);
        }
    }

    public void Move(List<Vector2Int> path)
    {
        //finish this move and then get new path
        if (target.Count > 0)
        {
            var newList = new List<Vector2Int>();
            newList.Add(target[0]);
            newList.AddRange(path);
            target = newList;
            return;
        }
        accumulatedTime = 0;
        target = path;
    }
    void Step(Vector3 to)
    {
        transform.position = Vector3.Lerp(startingPoint, to, accumulatedTime / speed);
    }

    private void Execute()
    {
        if (target.Count > 0)
        {
            startingPoint = Geometry.PointFromGrid(target[0]);
            var lastPosition = target[0];
            transform.position = startingPoint;
            target.RemoveAt(0);
            if (target.Count == 0)
            {
                // add action for destination point
                //if there is an resource box, pick it up TODO: do this universally
                if (tf.getTile(lastPosition) is Tile tile)
                {
                    var res = resourceManager.PickUp(lastPosition);
                    if (res != null) res.Owner = this;
                }
            }
        }

        accumulatedTime -= speed;
        if (accumulatedTime < 0)
        {
            //float numbers overflow check
            accumulatedTime = 0f;
        }
    }
}
