using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class RegionIPointerClick : MonoBehaviour , IPointerClickHandler
{
    private RegionSettings regionSettings;

    public void Awake()
    {
        this.regionSettings = GetComponent<RegionSettings>();
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            regionSettings.OpenExplorePanel();
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
            regionSettings.CloseExplorePanel();
          //  Debug.Log("Right Clicked");
        }  
    }
}
