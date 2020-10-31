using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EventPanel : MonoBehaviour
{
    [Header("Main Setting")]

    [SerializeField] private Text titleField; 

    [SerializeField] private Text descriptionTextField;

    [SerializeField] private Image sprite;

    [Header("Designer Settings")]

    [SerializeField] private string eventName;

    [SerializeField] private int nameFontSize = 22;

    [SerializeField] private int fontSize = 25;

    [TextArea(5, 10)]
    [SerializeField] private string description; 


    [Header("Buttons")]

    [SerializeField] GameObject buttonPrefab;
    
    [SerializeField] GameObject buttonHolder;

    [Header("Testing_")]

    [SerializeField] Text battleType;

    [SerializeField] Text BattleDif;

    [SerializeField] Text MinEnemyNumber;

    [SerializeField] Text MonsterDifMax;

    [SerializeField] Text MonsterID;

    [SerializeField] Text BeastNumber;


    #region Properities

    /* prop are for Editor..  */
    public string EventName { get { return eventName; } }
    public string Description { get { return description; } }
    public Text DescriptionTextField { get { return descriptionTextField; } }
    public Text TitleField { get { return this.titleField; } }
    public int FontSize { get { return fontSize; } set { fontSize = value; } }
    public int NameFontSize { get { return nameFontSize; } set { nameFontSize = value; } }

    #endregion

    public void SetupEventInfo (string _name, string _description , Sprite _sprite)
    {
        this.eventName = _name;
        this.description = _description;
        this.sprite.sprite = _sprite;
    }


    public void CreateButon(UnityAction evt, string text)
    {
        var gameObjectButton = Instantiate(this.buttonPrefab, this.transform.position, Quaternion.identity);
        gameObjectButton.transform.SetParent(buttonHolder.transform);

        var eventButton = gameObjectButton.GetComponent<EventButton>();
        eventButton.Text.text = text;
        eventButton.ButtonControler.onClick.RemoveAllListeners();
        eventButton.ButtonControler.onClick.AddListener(evt);
    }

    public void DestroyAllButtons()
    {
        foreach (Transform child in buttonHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void TestingFight(string _battleType, int _BattleDif, int _MinEnemyNumber, int _MonsterDifMax)
    {
        this.battleType.text = "battle type: " + _battleType;
        this.BattleDif.text = "battleDif: " + _BattleDif;
        this.MinEnemyNumber.text = "min enemy number: " +  _MinEnemyNumber;
        this.MonsterDifMax.text ="monsterDif max: "  + _MonsterDifMax;

    }

    public void TestingMonster (string _MonsterID, int _BeastNumber)
    {
        this.MonsterID.text = "monster ID: " + _MonsterID;
        this.BeastNumber.text = "beast number: " + _BeastNumber;
    }


}

