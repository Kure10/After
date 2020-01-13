using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionControler : MonoBehaviour
{
    [SerializeField] Region region;

    private Image image;

    public float fogValue = 0.25f;
    private Color black = new Color(0,0,0,1f);
    private Color idleColor = new Color(1,1,1,1);

    public Region GetRegion
    {
        get { return this.region; }
    }

    private void Awake()
    {
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
        // tohle tady nesmí byt
        if (this.region.IsInDarkness)
        {
            image.color = black;
        }
        else if (!this.region.IsExplored)
        {
            image.color = new Color(idleColor.r, idleColor.g, idleColor.b, fogValue);
        }
        else
        {
            image.color = new Color(idleColor.r, idleColor.g, idleColor.b, 1f);
        }
    }

    public void InicializationRegion ()
    {
        this.name = this.region.regionName;
        if (this.region.IsStartingRegion)
        {
            this.region.IsInShadow = true;
        }
        else
        {
            this.region.IsInDarkness = true;
        }
    }

    public void ExploreRegion()
    {
        if (this.region.IsInShadow)
        {
            this.region.IsExplored = true;
            this.region.RevealNeighbors();
        }
    }



}
