using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    // Start is called before the first frame update
    List<Vector2Int> target;
    private TimeControl tc;
    private float speed = 1f;
    private float accumulatedTime;
    private Vector3 startingPoint;
    void Start()
    {
        startingPoint = transform.position;
        target = new List<Vector2Int>();
        tc = GameObject.FindGameObjectWithTag("TimeController").GetComponent<TimeControl>();
    }

    // Update is called once per frame
    void Update()
    {
        accumulatedTime += Time.deltaTime * tc.TimePointMultiplier();
        //in case of more then one time point from last frame, skip rendering of previous actions
        while (accumulatedTime > 1f)
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
        accumulatedTime = 0;
        target = path;
    }
    void Step(Vector3 to)
    {
        transform.position = Vector3.Lerp(startingPoint, to, accumulatedTime);
    }

    private void Execute()
    {
        if (target.Count > 0)
        {
            startingPoint = Geometry.PointFromGrid(target[0]);
            transform.position = startingPoint;
            target.RemoveAt(0);
            if (target.Count == 0)
            {
                //TODO add action for destination point
            }
        }

        accumulatedTime -= 1f;
        if (accumulatedTime < 0)
        {
            //float numbers overflow check
            accumulatedTime = 0f;
        }
    }
}
