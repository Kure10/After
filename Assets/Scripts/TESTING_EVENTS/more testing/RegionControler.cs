using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionControler : MonoBehaviour
{
    [SerializeField] Region region;

    [SerializeField] Image image;

    public float fogValue = 0.25f;
    public Color color;

    private bool isInitialized = false;

    private void Awake()
    {
       image = GetComponent<Image>();
       InicializationRegion();
    }
    // Start is called before the first frame update
    void Start()
    {
        SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.region.isFarAway)
        {
            image.color = color;
        }
        else if (!this.region.isExplored)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, fogValue);
        }
        else
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
        }
    }

    public void InicializationRegion ()
    {

        if(!this.isInitialized)
        {
            this.region.isExplored = false;
            this.region.isFarAway = false;
        }

        this.isInitialized = true;
    }

    public void UpdateFarAwayRegions()
    {
        foreach (var item in this.region.neighborhoodRegions)
        {
            if(item.isFarAway == true)
            {
                item.isFarAway = false;
            }
        }
    }

    public void SetUp()
    {
        if (this.region.neighborhoodRegions.Count == 0)
        {
            Debug.Log("Region has no neighborhood Regions -> Error");
            return;
        }
        else
        {
            if (this.region.isStartingRegion)
            {
                this.name = "_" + this.region.regionName;

                foreach (var item in this.region.neighborhoodRegions)
                {
                    item.isFarAway = false;
                }
                this.region.isExplored = true; // možna nebude. První misse bude explore muj region nevím...
            }
            else
            {
                this.name = this.region.regionName;
            }
        }
    }

}
