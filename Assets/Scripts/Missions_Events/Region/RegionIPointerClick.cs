using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class RegionIPointerClick : MonoBehaviour , IPointerClickHandler
{
    private RegionOperator regionOperator;

    public void Awake()
    {
        this.regionOperator = GetComponent<RegionOperator>();
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Region regionToExplore = this.regionOperator.GetRegion();
            if (regionToExplore.AmountToUnlockedNeighborhodRegions <= 0 || regionToExplore.IsStartingRegion)
            {
                regionOperator.OpenExplorePanel();
            }
            else
            {
                Debug.Log("Region " + regionToExplore.RegionName +
                          "  nelze prozkoumat, neni dokonceno spravne mnozstvi q v okolnim regionu: " +
                          regionToExplore.AmountToUnlockedNeighborhodRegions);
            }
                

            // leftClick.Invoke();
           // Debug.Log("Left Clicked");
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
          //  middleClick.Invoke();
          //  Debug.Log("Middle Clicked");
        } 
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // rightClick.Invoke();
          //  regionOperator.CloseExplorePanel();
          //  Debug.Log("Right Clicked");
        }  
    }
}
