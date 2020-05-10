using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    public float CameraSpeed = 9f;
    public float MinY = 10f;
    public float MaxY = 40f;

    private float xAxis, yAxis, zAxis;
    private Vector3 movement;
    private static bool scrollEnabled = true;
    private static bool movementEnabled = true;
    // Start is called before the first frame update
    void Start()
    {
        movement = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        xAxis = 0;
        zAxis = 0;
        var speed = CameraSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = CameraSpeed * 6f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            xAxis = -speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            xAxis = speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.W))
        {
            zAxis = speed * Time.deltaTime;
        } 
        else if (Input.GetKey(KeyCode.S)) {
            zAxis = -speed * Time.deltaTime;
        }
        movement.x = xAxis;
        movement.z = zAxis;
        if (scrollEnabled)
        {
            movement.y = Input.mouseScrollDelta.y * Time.deltaTime * speed * -3;
        } else
        {
            movement.y = 0;
        }

        if (!movementEnabled)
        {
            movement.x = 0;
            movement.z = 0;
        }

        var height = transform.parent.transform.position.y;
        if (height > MaxY || height < MinY)
        {
            movement.y = 0;
        }
        
        transform.parent.transform.Translate(movement);
        //fix for fast scrolling on slow computer
        var v = transform.parent.transform.position;

        if (height < MinY)
        {
            transform.parent.transform.position = new Vector3(v.x, MinY + 0.01f, v.z);
        } else if (height > MaxY)
        {
            transform.parent.transform.position = new Vector3(v.x, MaxY - 0.01f, v.z);
        }
    }
    public static void ZoomByScrollEnabled(bool enabled)
    {
        scrollEnabled = enabled;
    }

    public static void MovementByArrowsEnabled(bool enabled)
    {
        movementEnabled = enabled;
    }

    public static void MovementAllEnable(bool enabled)
    {
        scrollEnabled = enabled;
        movementEnabled = enabled;
    }
}
