using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    // Start is called before the first frame update
    float movementSpeed = 3f;
    List<Vector2Int> target;
    void Start()
    {
        target = new List<Vector2Int>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target.Count > 0)
        {
            var next = target[0];
            var nextVector3 = Geometry.PointFromGrid(next);
            Step(nextVector3);
            if (Mathf.Approximately(transform.position.x, nextVector3.x) && Mathf.Approximately(transform.position.z, nextVector3.z))
            {
                target.RemoveAt(0);
            }
        }
    }
    public void Move(List<Vector2Int> path) 
    {
        target = path;
    }
    void Step(Vector3 to)
    {
        transform.position = Vector3.MoveTowards(transform.position, to, movementSpeed * Time.deltaTime);
    }
}
