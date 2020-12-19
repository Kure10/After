using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uWindowSpecialist : MonoBehaviour
{
    #region Fields

    [Header("Main Header")]
    [SerializeField] Image image;
    [SerializeField] Text characterName;
    [SerializeField] Text characterLevel;
    [SerializeField] Image backgroundImage;


    [Header("Stats")]
    [SerializeField] Transform healthBar;
    [SerializeField] Transform staminaBar;
    [Space]
    [SerializeField] Text militaryValue;
    [SerializeField] Text socialValue;
    [SerializeField] Text technicianValue;
    [SerializeField] Text scientistValue;
    [SerializeField] Text karmaValue;

    [Header("Activity Stats")]
    [SerializeField] List<SpecInventorySlot> characterSlots = new List<SpecInventorySlot>();

    [Header("Activity Stats")]
    [SerializeField] Text currentActivity;

    [Header("Button")]
    [SerializeField] Button mainButton;

    [Header("State")]
    [SerializeField] GameObject selectedPanel;
    [SerializeField] Text infoText;

    [Header("Results")]
    [SerializeField] GameObject success;
    [SerializeField] GameObject failure;

    /* For now buttons are not used.. waiting for functionality and implementation.....*/
    [Header("Buttons")]
    [SerializeField] Button Control1;
    [SerializeField] Button Control2;
    [SerializeField] Button Control3;

    private float percentHealth = 0;
    private float percentStamina = 0;


    private bool isActiveBuilding;

    // inventory is missiong ... Todo. (if designer will want to.)

    #endregion

    #region Properity
    public float GetPercentHelth { get => this.percentHealth; }
    public string GetName { get => this.characterName.text.ToString(); }
    public int GetKarma { get => int.Parse(this.karmaValue.text); }

    public int GetLevel { get => int.Parse(this.characterLevel.text); }

    public float GetHealth { get => int.Parse(this.characterLevel.text); }

    public Text SetCurrentActivity { set { currentActivity.text = value.text; } }

    public Button GetMainButton { get { return this.mainButton; } }

    public List<SpecInventorySlot> GetCharacterSlots()
    {
        return this.characterSlots;
    }
    public bool IsResultSuccess 
    { 
        set
        { 
            this.success.SetActive(value);
            this.failure.SetActive(!value);
        } 
    }

    public void TurnOffResult()
    {
        this.success.SetActive(false);
        this.failure.SetActive(false);
    }

    #endregion

    #region Private Methods
    private void SetImage(Specialists spec)
    {
        this.image.sprite = spec.Sprite;
        if (this.backgroundImage != null)
            this.backgroundImage.color = spec.SpecialistColor;
    }

    private void CalcHealtandStamina(Character character)
    {
        this.percentHealth = character.LifeEnergy.PercentHealth;
        this.percentStamina = character.LifeEnergy.PercentStamina;
        healthBar.transform.localScale = new Vector3(this.percentHealth / 100, 1f, 1f);
        staminaBar.transform.localScale = new Vector3(this.percentStamina / 100, 1f, 1f);
    }

    private void SetStatsPanel(Character character)
    {
        characterName.text = character.GetName();
        characterLevel.text = character.Stats.level.ToString();
        militaryValue.text = character.Stats.military.ToString();
        scientistValue.text = character.Stats.science.ToString();
        technicianValue.text = character.Stats.tech.ToString();
        socialValue.text = character.Stats.social.ToString();
        karmaValue.text = character.Stats.karma.ToString();
    }

    #endregion

    #region Public Methods
    public void SetAll(Character character)
    {
        SetImage(character.GetBlueprint());
        CalcHealtandStamina(character);
        SetStatsPanel(character);
    }

    public void ActivateCoverPanel(string text = " ")
    {
        this.selectedPanel.SetActive(true);
        this.infoText.text = text;
    }

    public void DeactivateCoverPanel(string text = " ")
    {
        this.selectedPanel.SetActive(false);
        this.infoText.text = text;
    }

    public void SetActiveSuperimposePanel()
    {
        bool state = this.selectedPanel.activeSelf;

        if (state == true)
            DeactivateCoverPanel();
        else
            ActivateCoverPanel("Specialista byl je vybran");
    }



    #endregion
}

