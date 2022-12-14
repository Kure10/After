using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SizeControler : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    // IMPORTNT !!!
    /*
     Tohle se už nepoužíva mam to tady jenom pro případ že by jeste mohlo
    */


    public GameObject infoPanel;
    private Animator anim;
    private Image[] images;
    private bool mouseOver = false;

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
    [SerializeField] float timeForInfo = 2;
    [SerializeField] Color fadeColor = Color.gray;
    [TextArea(4, 10)]
    [SerializeField] string textForInfo = "Lazy GameDesign";
    



    // Start is called before the first frame update
    void Start()
    {
        
       SetButtonName();
       SetCost();
       UpdateImage();
       SetBuildingSize();
       SetInfoText();
    }

    void Update()
    {
        ShowInfo();
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
        infoPanel = gameObject.transform.GetChild(6).gameObject;
        anim = infoPanel.GetComponent<Animator>();
        Transform go = infoPanel.transform.GetChild(0);
        Text text = go.GetComponent<Text>();
        text.text = textForInfo;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
    }

    private void ShowInfo()
    {
        if(mouseOver)
        {
            Invoke("ShowInfoWithDelay", timeForInfo);
        }
        else
        {
            CancelInvoke();
            infoPanel.SetActive(false);
        }
    }

    private void ShowInfoWithDelay()
    {
        infoPanel.SetActive(true);
    }

}
