using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AllowDragDrop : MonoBehaviour , IDragHandler
{

    [SerializeField]
    private bool dragParent = false;

    public Transform parent;

    public void OnDrag(PointerEventData eventData)
    {
        if(!dragParent)
        transform.position = eventData.position;

        if(dragParent)
        {
            parent.transform.position = eventData.position;
        }
    }

    private void Awake()
    {
        if (transform.parent.parent.parent != null)
        {
            parent = this.transform.parent.parent.parent;
            if(parent.name == "Canvas")
            {
                parent = this.transform.parent.parent;
            }
        }

        else if (transform.parent.parent != null)
            parent = this.transform.parent.parent;

        else if (transform.parent != null)
            parent = this.transform.parent;


    }

}
