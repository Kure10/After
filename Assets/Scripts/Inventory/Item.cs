using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Item : MonoBehaviour
{
    

    private ItemBlueprint blueprint;


    public ItemBlueprint Blueprint { get { return this.blueprint; } set { blueprint = value; } }

}
