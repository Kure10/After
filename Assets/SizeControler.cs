using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SizeControler : MonoBehaviour
{
    private Image[] images;

    [Header("Size of Building")]
    public bool[] myArray = new bool[] { true, true, false, true, false, true };

    // Start is called before the first frame update
    void Start()
    {
        images = GetComponentsInChildren<Image>();
        UpdateSize();
    }

    private void UpdateSize()
    {
        for (int i = 0; i < myArray.Length; i++)
        {
            if (myArray[i] == true)
                images[i].color = Color.black;
        }
    }

}
