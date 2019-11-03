using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingOnUse : MonoBehaviour
{
    private Button button; // tohle mozna nepotrebuji. nemusim cachovat

    private Building building; // reference a scriptAble Object odpovidajicí konkretni budove

    private void Awake()
    {
       building = GetComponent<BuildingBuilder>().GetBuildingReferenc();
       SetButtonEvent();
    }


    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetButtonEvent ()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => ButtonOnClick());
    }

    public void ButtonOnClick ()
    {
        Debug.Log("Button Executed !!!!  "  + this.name );
    }

}
