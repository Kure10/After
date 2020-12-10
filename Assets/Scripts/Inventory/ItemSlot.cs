using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] Image image;

    [SerializeField] Transform itemContainer;


    public ItemBlueprint slot;

    public void AddItem(GameObject gameObject)
    {
        gameObject.transform.SetParent(itemContainer);
    }

    public void SetEmpty()
    {
        image.gameObject.SetActive(false);
    }
}
