using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    public float CameraSpeed = 9f;
    public float MinY = 10f;
    public float MaxY = 40f;

    public static float cameraPositionY;
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

        Vector3 vec = transform.parent.transform.position;

        if (vec.y > MaxY || vec.y < MinY)
        {
            movement.y = 0;
        }

        bool isMaxZoom = false;
        bool isMinZoom = false;

        if (vec.y <= MinY)
        {
            vec.y = MinY;
            transform.parent.transform.position = new Vector3(vec.x, 10 /*MinY + 0.01f*/, vec.z);
            isMinZoom = true;
        }
        else if (vec.y >= MaxY)
        {
            vec.y = MaxY;
            transform.parent.transform.position = new Vector3(vec.x, 40 /*MaxY + 0.01f*/, vec.z);
            isMaxZoom = true;
        }

        cameraPositionY = transform.parent.transform.position.y; // properity for healthBar

        if ((movement.y < 0 && !isMinZoom) || (movement.y > 0 && !isMaxZoom))
            transform.parent.transform.Translate(movement);

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
