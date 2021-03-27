using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squar : MonoBehaviour
{

    public int yCoordinate = 0;
    public int xCoordinate = 0;

    [SerializeField] GameObject container;

    public Unit unitInSquar = null;

    public void SetCoordinates(int x, int y)
    {
        xCoordinate = x;
        yCoordinate = y;
    }
}
