using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] GameObject cameraHolder;
    [Space]

    public float CameraSpeed = 9f;
    public float CameraMouseSpeed = 3f;
    public float MinY = 10f;
    public float MaxY = 40f;

    public static float cameraPositionY;
    private float xAxis, yAxis, zAxis;
    private Vector3 movement;
    private static bool scrollEnabled = true;
    private static bool movementEnabled = true;
    // Start is called before the first frame update

    Vector3 mouseDelta = new Vector3(0, 0, 0);
    Vector3 MouseStart = new Vector3(0, 0, 0);
    private bool bDragging = false;
    float dist = 5f;

    void Start()
    {
        movement = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        bool mouseState = Input.GetKey(KeyCode.Mouse0);

        if (Input.GetMouseButtonDown(0))
        {
            MouseStart = Input.mousePosition;

        }

        if (Input.GetMouseButton(0))
        {
            Vector3 mouseDelta = Input.mousePosition - MouseStart;
            float dx = (mouseDelta.x) * (0.0001f * CameraMouseSpeed);  
            float dy = (mouseDelta.y) * (0.0001f * CameraMouseSpeed); 

            if (dx != 0 | dy != 0)
            {
                movement = new Vector3(-dx, 0, -dy);
                cameraHolder.transform.Translate(movement);
            }
        }

        if(!mouseState)
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
            else if (Input.GetKey(KeyCode.S))
            {
                zAxis = -speed * Time.deltaTime;
            }
            movement.x = xAxis;
            movement.z = zAxis;

            if (scrollEnabled)
            {
                movement.y = Input.mouseScrollDelta.y * Time.deltaTime * speed * -3;
            }
            else
            {
                movement.y = 0;
            }

            if (!movementEnabled)
            {
                movement.x = 0;
                movement.z = 0;
            }

            Vector3 vec = cameraHolder.transform.position;

            if (vec.y > MaxY || vec.y < MinY)
            {
                movement.y = 0;
            }

            bool isMaxZoom = false;
            bool isMinZoom = false;

            if (vec.y <= MinY)
            {
                vec.y = MinY;
                cameraHolder.transform.position = new Vector3(vec.x, 10 /*MinY + 0.01f*/, vec.z);
                isMinZoom = true;
            }
            else if (vec.y >= MaxY)
            {
                vec.y = MaxY;
                cameraHolder.transform.position = new Vector3(vec.x, 40 /*MaxY + 0.01f*/, vec.z);
                isMaxZoom = true;
            }

            cameraPositionY = cameraHolder.transform.position.y; // properity for healthBar

            if ((movement.y <= 0 && !isMinZoom) || (movement.y >= 0 && !isMaxZoom))
                cameraHolder.transform.Translate(movement);
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
