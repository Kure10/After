using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionControler : MonoBehaviour
{
    [SerializeField] Region thisRegion;

    [SerializeField] Image image;

    public float fogValue = 0.25f;

    private void Awake()
    {
       image = GetComponent<Image>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!thisRegion.isExplored)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, fogValue);
        }
        else
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
        }
    }
}
