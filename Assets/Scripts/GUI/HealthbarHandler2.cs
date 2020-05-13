using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarHandler2 : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Canvas")]
    [SerializeField] private RectTransform rect;
    [Header("Image")]
    [SerializeField] private Image healthbarImage;
    [SerializeField] private RectTransform imageRect;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
