using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheatButton : MonoBehaviour
{

    [SerializeField] GameObject panel;
    public Button button;
    private Text text;
    // Start is called before the first frame update
    void Start()
    {
        button = gameObject.GetComponent<Button>();
        Transform t;
        t = transform.Find("Text");
        text = t.GetComponent<Text>();
    }

    public void Coloring()
    {
        ColorBlock colors = button.colors;

        if (panel.activeSelf == true)
        {
            colors.normalColor = Color.red;
            button.colors = colors;
            text.text = "Cheats ON";
        }
        else
        {
            colors.normalColor = Color.white;
            button.colors = colors;
            text.text = "Cheats OFF";
        }
    }


}
