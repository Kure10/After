using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float CameraSpeed = 3f;
    private float xAxis, yAxis, zAxis;
    private Vector3 movement;
    private static bool scrollEnabled = true;
    // Start is called before the first frame update
    void Start()
    {
        movement = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        xAxis = 0;
        yAxis = 0;
        if (Input.GetKey(KeyCode.A))
        {
            xAxis = -CameraSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            xAxis = CameraSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.W))
        {
            yAxis = CameraSpeed * Time.deltaTime;
        } 
        else if (Input.GetKey(KeyCode.S)) {
            yAxis = -CameraSpeed * Time.deltaTime;
        }
        movement.x = xAxis;
        movement.y = yAxis;
        if (scrollEnabled)
        {
            movement.z = Input.mouseScrollDelta.y * Time.deltaTime * 10;
        } else
        {
            movement.z = 0;
        }
        transform.Translate(movement);
    }
    public static void ZoomByScrollEnabled(bool enabled)
    {
        scrollEnabled = enabled;
    }
}
