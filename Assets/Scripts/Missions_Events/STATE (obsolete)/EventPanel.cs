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

    [Header("States Panels")]

    [SerializeField] GameObject testInfoContent;

    [SerializeField] GameObject selectionCharacterContent;

    [SerializeField] GameObject selectionInfoContent;

    [SerializeField] GameObject testButtonContent;

    [Header("Active Buttons")]

    [SerializeField] Button continueButton;

    [Header("Event Slots")]

    [SerializeField] List<ItemSlot> eventSlots = new List<ItemSlot>();

    [SerializeField] Scrollbar scrollBar;

    [Header("Information For Player")]

    [SerializeField] Text eventSubTitle;

    [SerializeField] Text diff;

    [SerializeField] Text rateMod;

    [SerializeField] Text selectionInfoText;

    [SerializeField] Text amountCharacterSelectedText;

    [SerializeField] GameObject karma;

    [SerializeField] GameObject separatelyGo;

    [SerializeField] GameObject togetherGo;

    [SerializeField] GameObject[] testingCondition;

    [Header("Holders for Buttons")]

    [SerializeField] GameObject buttonHolder;

    [SerializeField] Transform charContent;

    [Header("Prefabs")]

    [SerializeField] GameObject caseButtonPrefab;

    [SerializeField] GameObject characterButtonPrefab;

    [Header("Special - Buttons / Panels")]

    [SerializeField] GameObject legendGo;

    [SerializeField] Button minimize;

    private Dictionary<GameObject, Character> charactersInContent = new Dictionary<GameObject, Character>();

    [Space]
    [Space]
    [Header("Testing_")]

    [SerializeField] public Button buttonForTMPProceed;

    [SerializeField] public Text testingTMPinfo;

    [SerializeField] public Text testingTMPinfo2;



    #region Properities

    public Text DescriptionTextField { get { return descriptionTextField; } }
    public Text TitleField { get { return this.titleField; } }
    public Text SelectionInfoText { get { return this.selectionInfoText; } }
    public Text AmountCharacterSelectedText { get { return this.amountCharacterSelectedText; } }
    public GameObject GetCharacterButtonPrefab { get { return this.characterButtonPrefab; } }
    public Button GetContinueButton { get { return this.continueButton; } }
    public Button GetMinimizeButton { get { return this.minimize; } }
    public Dictionary<GameObject, Character> GetCharactersOnEvent { get { return this.charactersInContent; } }

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
                testInfoContent.SetActive(true);
                selectionCharacterContent.SetActive(true);
                selectionInfoContent.SetActive(false);
                testButtonContent.SetActive(true);
            }
            else
            {

                testInfoContent.SetActive(false);
                selectionCharacterContent.SetActive(true);
                selectionInfoContent.SetActive(true);
                testButtonContent.SetActive(false);
            }

            scrollBar.value = 1;
        }
    }

    public void SetAmountTestedCharacters(EventTestCase _tCase , int minCharacters)
    {
        if (_tCase != null)
        {
            if (_tCase.GetClass == EventTestCase.ClassTest.Together)
            {
                this.amountCharacterSelectedText.text = $" {minCharacters } / {minCharacters}";
            }
            else
            {
                this.amountCharacterSelectedText.text = $" {minCharacters } / {_tCase.GetMaxCharParticipation}";
            }
        }
        else
        {
            this.amountCharacterSelectedText.text = $" ";
        }
    }

    public void SetupTestingState(EventTestCase tCase)
    {
        IsInvolvedKarma(tCase.GetKarmaInfluence);
        IsTestedSeparately(tCase.GetClass);
        diff.text = tCase.GetDifficulty.ToString();
        rateMod.text = tCase.GetRateMod.ToString();
        SetupConditionAtributes(tCase);
        eventSubTitle.text = tCase.GetName;

        if (tCase.GetClass == EventTestCase.ClassTest.Together)
        {
            selectionInfoText.text = $" Test je pro vsechny specialisty";
        }
        else
        {
            selectionInfoText.text = $" Vyber {tCase.GetMinCharParticipation} charakter až {tCase.GetMaxCharParticipation}.";
        }
    }

    public void SetImage(Sprite _sprite)
    {
        this.sprite.sprite = _sprite;
    }

    public void CreateButon(UnityAction evt, string text, string buttonDescription)
    {
        var gameObjectButton = Instantiate(this.caseButtonPrefab, this.transform.position, Quaternion.identity);
        gameObjectButton.transform.SetParent(buttonHolder.transform);
        gameObjectButton.transform.localScale = new Vector3(1, 1, 1);

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

    public enum PanelStates
    {
        Battle,
        Test,
        Selection
    }

    #region Private Methods
    private void SetupConditionAtributes(EventTestCase tCase)
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
    private void IsTestedSeparately(EventTestCase.ClassTest classTest)
    {
        if (classTest == EventTestCase.ClassTest.Separately)
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

    public void ActivateCharacterContent()
    {
        foreach (KeyValuePair<GameObject, Character> item in charactersInContent)
        {
            item.Key.SetActive(true);
        }
    }

    public void DisableCharacterContent()
    {
        foreach (KeyValuePair<GameObject, Character> item in charactersInContent)
        {
            item.Key.SetActive(false);
        }
    }

    public void AddCharacterToSelectionContent(GameObject gameObject, Character character)
    {
        charactersInContent.Add(gameObject, character);
        gameObject.transform.SetParent(charContent);
    }

    public void TurnOnLegend()
    {
        legendGo.SetActive(true);
    }

    public void TurnOffLegend()
    {
        legendGo.SetActive(false);
    }

    public void ReCalculatePositions()
    {
        int count = charactersInContent.Count;
        Character[] arrayPos = new Character[count];

        int i = 0;
        foreach (KeyValuePair<GameObject, Character> item in charactersInContent)
        {
            arrayPos[i] = item.Value;
            i++;
        }

        var rect = charContent.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);

        for (int j = 0; j < arrayPos.Length - 1; j++)
        {
            foreach (KeyValuePair<GameObject, Character> item in charactersInContent)
            {
                if (arrayPos[j] == item.Value)
                {
                    item.Key.transform.SetAsFirstSibling();
                }
            }
        }


    }

    public ItemSlot FindEmptySlot()
    {
        ItemSlot emptySlot = new ItemSlot();

        foreach (ItemSlot item in eventSlots)
        {
            if (item.IsEmpty)
            {
                emptySlot = item;
                break;
            }
        }

        return emptySlot;
    }

    #endregion
}
