using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class RegionButControler : MonoBehaviour , IPointerClickHandler
{
    State currentState;
    [SerializeField] Region region;

    // Start is called before the first frame update
    void Start()
    {

        currentState = new Dark(region);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        currentState = currentState.Process(eventData);
    }
}
