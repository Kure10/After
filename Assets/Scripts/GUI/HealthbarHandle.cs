using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarHandle : MonoBehaviour
{
    private Image healthbarImage;
    public GameObject parent;

    private Transform pivot;
    // Start is called before the first frame update
    void Awake()
    {
        healthbarImage = GetComponentInParent<Image>();

    }

    public void SetHPValue(float value)
    {
        if (healthbarImage is null)
        {
        }
        healthbarImage.fillAmount = value;
        if (healthbarImage.fillAmount < 0.3f)
        {
            healthbarImage.color = Color.red;
        } else if (healthbarImage.fillAmount < 0.6f)
        {
            healthbarImage.color = Color.yellow;
        }
        else
        {
            healthbarImage.color = Color.green;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (pivot == null)
        {
            pivot = parent.gameObject.transform.Find("pivot");
        }
        transform.position = Camera.main.WorldToScreenPoint(pivot.position);
    }
}
