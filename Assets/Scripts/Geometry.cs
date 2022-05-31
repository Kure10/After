using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Geometry
{
    static public Vector3 PointFromGrid(Vector2Int gridPoint, bool isWall = false)
    {
        float y = isWall == true ? 0.5f : 0f;
        float x = -0.5f + 1.0f * gridPoint.x;
        float z = -0.5f + 1.0f * gridPoint.y;
        return new Vector3(x, y, z);
    }

    static public Vector2Int GridPoint(int col, int row)
    {
        return new Vector2Int(col, row);
    }

    static public Vector2Int GridFromPoint(Vector3 point)
    {
        int col = Mathf.RoundToInt(0.5f + point.x);
        int row = Mathf.RoundToInt(0.5f + point.z);
        return new Vector2Int(col, row);
    }
}
