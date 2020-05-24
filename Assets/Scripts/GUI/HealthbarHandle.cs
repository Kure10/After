using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarHandle : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Canvas")]
    [Header("Image")]
    [SerializeField] private Image healthbarImage;
    [SerializeField] private RectTransform canvas;

    private float initialHeight;
    private float initialWidth;

    private int percentScaleForOneMeter = 2; // dve procenta narust ci pokles
    private int initialDistance = 20;  // Když je kamera vzdalena 20m. tak ma healthBar 100% scale. když 10m tak 80%


    void Start()
    {
        initialWidth = canvas.rect.width;
        initialHeight = canvas.rect.height;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateSizeOfPanel();
    }

    private void CalculateSizeOfPanel()
    {
        float cameraPositionY = CameraMovement.cameraPositionY;

        float tmp = initialDistance - cameraPositionY;
        tmp = tmp * percentScaleForOneMeter;
        tmp = 100 - tmp; // procento narustu ci poklesu

        float newWidth = initialWidth * (tmp / 100);
        float newHeight = initialHeight * (tmp / 100);

        Vector2 vec = new Vector2((int)newWidth, (int)newHeight);
        canvas.sizeDelta = vec;
    }

    // Toto jsem prekopiroval od tebe. 
    // Ale spravne si myslim že by to mělo byt udělane podle Scalu..
    //  131 kamene treba mužes vytežit   100 % je 131
    // a trojclenkou dopocitat procento Scalu..
    // nevim kde se bere kolik ma kamen bodu.. Tak to zatim necham tak.

    public void SetHPValue(float value)
    {
        if (healthbarImage is null)
        {
        }
        healthbarImage.fillAmount = value;
        if (healthbarImage.fillAmount < 0.3f)
        {
            healthbarImage.color = Color.red;
        }
        else if (healthbarImage.fillAmount < 0.6f)
        {
            healthbarImage.color = Color.yellow;
        }
        else
        {
            healthbarImage.color = Color.green;
        }
    }
}
