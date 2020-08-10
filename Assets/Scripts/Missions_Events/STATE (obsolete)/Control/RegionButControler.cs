using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class RegionButControler : MonoBehaviour , IPointerClickHandler
{
    State currentState;
    [SerializeField] Region region;
    private Image regionImage;
    private GameObject explorePanel;

    void Awake()
    {
        explorePanel = this.transform.GetChild(0).GetChild(0).gameObject;
        regionImage = this.GetComponent<Image>();
    }
    // Start is called before the first frame update
    void Start()
    {
        currentState = new Dark(region);
        currentState = currentState.Process(regionImage);
    }

    // Update is called once per frame
    void Update()
    {
        if(this.currentState == null)
        {
            return;
        }

      // if(region.isExplored == true)
       //     currentState = currentState.Process

                // takze budu cekovat Region v jakem je stavu

        // dale pokud (v jakem je stavu) tak,,, 

        // bych mu měl zmenit stav

        // udelam current state process 

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(this.currentState == null)
            currentState = new Dark(region);

        currentState = currentState.Process(regionImage, this.explorePanel, eventData);
    }

}
