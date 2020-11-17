using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static EventController;

public class EventPanel : MonoBehaviour
{
    [Header("Main Setting")]

    [SerializeField] private Text titleField;

    [SerializeField] private Text descriptionTextField;

    [SerializeField] private Image sprite;

    [Header("Sections Panels")]

    [SerializeField] GameObject testInfoContent;

    [SerializeField] GameObject selectionCharacterContent;

    [SerializeField] GameObject panelButtonNormal;

    [SerializeField] GameObject panelButtonTesting;

    [SerializeField] GameObject rateModandDiffGo;

    [Header("Active Buttons")]

    [SerializeField] Button continueButton;

    [Header("Information For Player")]

    [SerializeField] Text eventSubTitle;

    [SerializeField] Text diff;

    [SerializeField] Text rateMod;

    [SerializeField] GameObject karma;

    [SerializeField] GameObject separatelyGo;

    [SerializeField] GameObject togetherGo;

    [SerializeField] GameObject[] testingCondition;

    [Header("Holders for Buttons")]

    [SerializeField] GameObject buttonHolder;

    [SerializeField] GameObject characterContent;

    [Header("Prefabs")]

    [SerializeField] GameObject caseButtonPrefab;

    [SerializeField] GameObject characterButtonPrefab;

    [Header("Special - Buttons / Panels")]

    [SerializeField] GameObject legendGo;



    /* zatim nevim mozna z nebudu potrebovat*/
    List<GameObject> charactersInContent = new List<GameObject>();

    #region Hovna
    [Space]
    [Space]
    [Header("Testing_")]

    // battle.....

    [SerializeField] Text battleType;

    [SerializeField] Text BattleDif;

    [SerializeField] Text MinEnemyNumber;

    [SerializeField] Text MonsterDifMax;

    [SerializeField] Text MonsterID;

    [SerializeField] Text BeastNumber;

    // Test . . . . .. 

    [Space]
    [Space]
    [Space]
    [Space]

    [SerializeField] public GameObject testingBattle;

    [SerializeField] public GameObject testingTest;

    #endregion

    #region Properities

    public Text DescriptionTextField { get { return descriptionTextField; } }
    public Text TitleField { get { return this.titleField; } }
    public GameObject GetCharacterTransformContent { get { return this.characterContent; } }
    public GameObject GetCharacterButtonPrefab { get { return this.characterButtonPrefab; } }
    public Button GetContinueButton { get { return this.continueButton; } }

    #endregion

    public PanelStates SetState
    {
        set
        {
            if (PanelStates.Battle == value)
            {

                // todo
            }
            else if (PanelStates.Test == value)
            {
                panelButtonNormal.SetActive(false);
                testInfoContent.SetActive(true);
                selectionCharacterContent.SetActive(true);
                panelButtonTesting.SetActive(true);
                rateModandDiffGo.SetActive(true);
            }
            else
            {
                panelButtonNormal.SetActive(true);
                testInfoContent.SetActive(false);
                selectionCharacterContent.SetActive(false);
                panelButtonTesting.SetActive(false);
                rateModandDiffGo.SetActive(false);
            }
        }
    }

    public void SetupTestingState(TestCase tCase)
    {
        IsInvolvedKarma(tCase.GetKarmaInfluence);
        IsTestedSeparately(tCase.GetClass);
        diff.text = tCase.GetDifficulty.ToString();
        rateMod.text = tCase.GetRateMod.ToString();
        SetupConditionAtributes(tCase);
        eventSubTitle.text = tCase.GetName;
    }

   

    public void SetupEventInfo(string _name, string _description, Sprite _sprite)
    {
        //this.eventName = _name;
        //this.description = _description;
        this.sprite.sprite = _sprite;
    }


    public void CreateButon(UnityAction evt, string text, string buttonDescription)
    {
        var gameObjectButton = Instantiate(this.caseButtonPrefab, this.transform.position, Quaternion.identity);
        gameObjectButton.transform.SetParent(buttonHolder.transform);

        var eventButton = gameObjectButton.GetComponent<EventButton>();
        eventButton.Text.text = text;
        eventButton.ButtonDescription.text = buttonDescription;
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

    public void AddCharacterToSelectionContent(GameObject gameObject)
    {
        charactersInContent.Add(gameObject);
    }

    public void TestingFight(string _battleType, int _BattleDif, int _MinEnemyNumber, int _MonsterDifMax)
    {
        this.battleType.text = "battle type: " + _battleType;
        this.BattleDif.text = "battleDif: " + _BattleDif;
        this.MinEnemyNumber.text = "min enemy number: " + _MinEnemyNumber;
        this.MonsterDifMax.text = "monsterDif max: " + _MonsterDifMax;

    }

    public void TestingMonster(string _MonsterID, int _BeastNumber)
    {
        this.MonsterID.text = "monster ID: " + _MonsterID;
        this.BeastNumber.text = "beast number: " + _BeastNumber;
    }

    public enum PanelStates
    {
        Battle,
        Test,
        Selection
    }

    #region Private Methods
    private void SetupConditionAtributes(TestCase tCase)
    {
        testingCondition[0].SetActive(tCase.IsTestingLevel);
        testingCondition[1].SetActive(tCase.IsTestingMilitary);
        testingCondition[2].SetActive(tCase.IsTestingScience);
        testingCondition[3].SetActive(tCase.IsTestingSocial);
        testingCondition[4].SetActive(tCase.IsTestingTechnical);
    }
    private void IsInvolvedKarma(bool value)
    {
        if (value == true)
            karma.SetActive(true);
        else
            karma.SetActive(false);
    }
    private void IsTestedSeparately(ClassTest classTest)
    {
        if (classTest == ClassTest.Separately)
        {
            separatelyGo.SetActive(true);
            togetherGo.SetActive(false);
        }
        else
        {
            separatelyGo.SetActive(false);
            togetherGo.SetActive(true);
        }
    }

    #endregion

    #region Helping Methods

    public void TurnOnLegend()
    {
        legendGo.SetActive(true);
    }

    public void TurnOffLegend()
    {
        legendGo.SetActive(false);
    }

    #endregion
}

