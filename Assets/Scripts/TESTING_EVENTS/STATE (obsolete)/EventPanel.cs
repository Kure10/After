using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventPanel : MonoBehaviour
{
    [Header("Main Setting")]

    //[SerializeField]
    //private Sprite sprite;

    [SerializeField]
    private Text nameTextField;

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
    private string description;

    [Header("Buttons")]

    
    public int numberOfOptions; // prozatím public

    [SerializeField]
    private Button origin;

    [SerializeField]
    private GameObject parrent;

    /* ?? Tady mozna bude neco jineho   ,, jednoduse musím vytvořit buttonu podle počtu odpovedí a naležite vyplnit.. DYnamycky probug..*/

    #region Properities

    public string EventName { get { return eventName; } }
    public string Description { get { return description; } }
    public Text DescriptionTextField { get { return descriptionTextField; } }

    public Text NameTextField { get { return nameTextField; } }
    public int FontSize { get { return fontSize; } set { fontSize = value; } }

    public int NameFontSize { get { return nameFontSize; } set { nameFontSize = value; } }

    #endregion


    // Start is called before the first frame update
    void Start()
    {
        descriptionTextField.fontSize = fontSize;
        CreateOptions();
    }

    public void CreateOptions()
    {
        if (numberOfOptions == 1)
        {
            SetupOrigin();
        }
        else if (numberOfOptions > 1)
        {
            SetupOrigin();
            AddNewOptions();
        }
        else
        {
            Debug.LogError("number of options is less than 1. Must be 1 and more !");
        }
    }

    public void AddNewOptions()
    {
        for (int i = 1; i < numberOfOptions; i++)
        {
            SetupOption();
        }
    }

    public void SetupOrigin()
    {
        // Tady se nastavi tlacitko jeho text a barva.. 
        // A taky udalost která se má vyvolat po zmačknutí tlačítka..
        // možna i vice.
    }

    public void SetupOption()
    {
        Button newButton;
        newButton = Instantiate(origin, this.transform.position, Quaternion.identity);
        newButton.transform.SetParent(parrent.transform);

    }

}
