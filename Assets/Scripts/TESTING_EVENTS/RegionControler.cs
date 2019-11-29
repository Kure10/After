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
    private Color normalColor;

    private bool isInitialized = false;


    public Region GetRegion
    {
        get { return this.region; }
    }

    private void Awake()
    {
       normalColor = this.GetComponent<Image>().color;
       image = GetComponent<Image>();
       InicializationRegion();
    }
    // Start is called before the first frame update
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        if (this.region.IsOutOfReach)
        {
            image.color = color;
        }
        else if (!this.region.IsExplored)
        {
            image.color = new Color(normalColor.r, normalColor.g, normalColor.b, fogValue);
        }
        else
        {
            image.color = new Color(normalColor.r, normalColor.g, normalColor.b, 1f);
        }
    }

    public void InicializationRegion ()
    {
        this.name = this.region.regionName;
        if (this.region.IsStartingRegion)
        {
            this.region.IsExplored = false;
            this.region.IsOutOfReach = false;
        }
        else if (!this.isInitialized)
        {
            this.region.IsExplored = false;
            this.region.IsOutOfReach = true;
        }

        this.isInitialized = true;
    }

    public void UpdateFarAwayRegions()
    {
        foreach (var item in this.region.neighborhoodRegions)
        {
            if(item.IsOutOfReach == true)
            {
                item.IsOutOfReach = false;
                item.IsExplored = false;
            }
        }
    }

    public void ExploreRegion()
    {
        if(!this.region.IsOutOfReach) 
        {
            if (!this.region.IsExplored)
                this.region.IsExplored = true;

            foreach (var nearRegion in region.neighborhoodRegions)
            {
                if (nearRegion.IsOutOfReach)
                    nearRegion.IsOutOfReach = false;
            }
        }

    }



}
