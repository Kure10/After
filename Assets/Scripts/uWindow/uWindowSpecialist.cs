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
    [SerializeField] Text currentActivity;

    [Header("Button")]
    [SerializeField] Button mainButton;

    [Header("State")]
    [SerializeField] GameObject selectedPanel;
    [SerializeField] Text infoText;


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

    #endregion

    #region Private Methods
    private void SetImage(Specialists spec)
    {
        this.image.sprite = spec.Sprite;
        if (this.backgroundImage != null)
            this.backgroundImage.color = spec.SpecialistColor;
    }

    private void CalcHealtandStamina(Specialists spec)
    {
        this.percentHealth = spec.PercentHealth;
        this.percentStamina = spec.PercentStamina;
        healthBar.transform.localScale = new Vector3(spec.PercentHealth / 100, 1f, 1f);
        staminaBar.transform.localScale = new Vector3(spec.PercentStamina / 100, 1f, 1f);
    }

    private void SetStatsPanel(Specialists spec)
    {
        characterName.text = spec.FullName.ToString();
        characterLevel.text = spec.Level.ToString();
        militaryValue.text = spec.Mil.ToString();
        scientistValue.text = spec.Scl.ToString();
        technicianValue.text = spec.Tel.ToString();
        socialValue.text = spec.Sol.ToString();
        karmaValue.text = spec.Kar.ToString();
    }

    #endregion

    #region Public Methods
    public void SetAll(Character character)
    {
        var spec = character.GetBlueprint(); // Todo Je to spatne. musi se brat hodnoty z charakteru

        SetImage(spec);
        CalcHealtandStamina(spec);
        SetStatsPanel(spec);
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

