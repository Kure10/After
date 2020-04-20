using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventPanel : MonoBehaviour
{
    [Header("Main Setting")]

    private Sprite sprite;  // Main Image of Event

    [SerializeField]
    private Text titleField; 

    [SerializeField]
    private Text descriptionTextField;

    [Header("Designer Settings")]

    [SerializeField]
    private string eventName;

    [SerializeField]
    private int nameFontSize = 22;

    [SerializeField]
    private int fontSize = 25;



    [TextArea(5, 10)]
    [SerializeField]
    private string description; // this is only for testing purposes (for Game Designer or Artist..)

    [Header("Buttons")]

    
    private int numberOfOptions; // prozatím public

    [SerializeField]
    private Button origin; // This is origin Button like a template

    [SerializeField]
    private GameObject optionParrent; // just a holder for options

    #region Properities

    public string EventName { get { return eventName; } }
    public string Description { get { return description; } }
    public Text DescriptionTextField { get { return descriptionTextField; } }

    public Sprite SetSprite
    {
        set { this.sprite = value; }
    }

    public Text TitleField { get { return this.titleField; } }
    public int FontSize { get { return fontSize; } set { fontSize = value; } }

    public int NameFontSize { get { return nameFontSize; } set { nameFontSize = value; } }

    #endregion

    public void CreateOptions(int numberOfOptions, string[] optionsTextField)
    {
        this.numberOfOptions = numberOfOptions;

        if (numberOfOptions == 1)
        {
            SetupOrigin(optionsTextField[0]);
        }
        else if (numberOfOptions > 1)
        {
            SetupOrigin(optionsTextField[0]);
            AddNewOptions(optionsTextField);
        }
        else
        {
            Debug.LogError("number of options is less than 1. Must be 1 and more !");
        }
    }

    private void AddNewOptions(string[] optionsTextField)
    {
        for (int i = 1; i < numberOfOptions; i++)
        {
            SetupOption(optionsTextField[i]);
        }
    }

    private void SetupOrigin(string textButton)
    {
        Button button = this.origin.GetComponent<Button>();
        Image image = this.origin.GetComponent<Image>();
        Text buttonText = this.origin.GetComponentInChildren<Text>();

        //  button.onClick.AddListener(); 
        //  image.color; 

        buttonText.text = textButton;


    }

    private void SetupOption(string text)
    {
        Button newButton;
        newButton = Instantiate(origin, this.transform.position, Quaternion.identity);
        newButton.transform.SetParent(this.optionParrent.transform);

        Button button = newButton.GetComponent<Button>();
        Image image = newButton.GetComponent<Image>();
        Text buttonTextField = newButton.GetComponentInChildren<Text>();

        //  button.onClick.AddListener(); 
        //  image.color; 

        buttonTextField.text = text;

    }

}
