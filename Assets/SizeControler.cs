using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SizeControler : MonoBehaviour
{
    private Image[] images;

    [Header("Size of Building")]
    public bool[] myArray = new bool[] { true, true, false, true, false, true };
    [Header("Name of Building")]
    [SerializeField] string title = "Default";
    [Header("Resources")]
    [SerializeField] int civilniMaterial = 0;
    [SerializeField] int technickyMaterial = 0;
    [SerializeField] int vojenskyMaterial = 0;
    [Header("Images")]
    [SerializeField] Sprite backgroundImage;
    [SerializeField] Sprite ilustrationImage;
    [Header("Info")]
    [SerializeField] string infoText = "Lazy GameDesign";


    // Start is called before the first frame update
    void Start()
    {
        
       SetButtonName();
       SetCost();
       UpdateImage();
       SetBuildingSize();
       SetInfoText();

    }

    private void SetButtonName()
    {
        Transform go = this.gameObject.transform.GetChild(0);
        Text text = go.GetComponent<Text>();
        text.text = title;
    }

    private void SetCost()
    {
        // set civil material
        Transform go = this.gameObject.transform.GetChild(1);
        go = go.GetChild(0);
        Text text = go.GetComponent<Text>();
        text.text = civilniMaterial.ToString();
        // set technicky material
        go = this.gameObject.transform.GetChild(2);
        go = go.GetChild(0);
        text = go.GetComponent<Text>();
        text.text = technickyMaterial.ToString();
        // set vojencky material
        go = this.gameObject.transform.GetChild(3);
        go = go.GetChild(0);
        text = go.GetComponent<Text>();
        text.text = vojenskyMaterial.ToString();
    }

    private void SetBuildingSize()
    {
        Transform go = this.gameObject.transform.GetChild(5);
        if (go != null)
        {
            images = go.GetComponentsInChildren<Image>();
            UpdateSize();
        }
    }

    private void UpdateSize()
    {
        for (int i = 0; i < myArray.Length; i++)
        {
            if (myArray[i] == true)
                images[i].color = Color.black;
        }
    }

    private void UpdateImage()
    {
        Image image;
        if (backgroundImage != null)
        {
            image = gameObject.GetComponent<Image>();
            image.sprite = backgroundImage;
        }

        if (ilustrationImage != null)
        {
            image = gameObject.transform.GetChild(4).GetComponent<Image>();
            image.sprite = ilustrationImage;
        }
    }

    private void SetInfoText ()
    {
        Transform go = gameObject.transform.GetChild(6);
        go = go.transform.GetChild(0);
        Text text = go.GetComponent<Text>();
        text.text = infoText;
    }

}
