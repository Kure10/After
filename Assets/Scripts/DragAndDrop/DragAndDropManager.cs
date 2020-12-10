using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDropManager : MonoBehaviour
{
    static private bool _isDraging = false;

    static public bool IsDraging { get { return _isDraging; } set { _isDraging = value; } }

    public Item dragingSlot = null;

    //public IInventory oldPosition;

    //public IInventory newPosition;


    static public (GameObject go, Item slot) dragingObject;

    //static public void SetDestination (IInventory slot) // nepotrebuji ho.. musim set destiny
    //{

    //}
    static public void InitDraging(GameObject go, Item slot)
    {
        dragingObject = (go, slot);
    }

    static public void StartDraging(GameObject go, Item slot)
    {
        dragingObject = (go, slot);


    }

    //static public void EndDraging(IInventory destination , IInventory source)
    //{
        


    //}

}
