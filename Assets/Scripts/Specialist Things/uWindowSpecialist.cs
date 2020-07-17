using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uWindowSpecialist : MonoBehaviour
{
    #region Fields

    [Header("Main Header")]
    [SerializeField] Image specialistImage;
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
    [Header("Stats")]
    [SerializeField] Text currentActivity;

    /* For now buttons are not used.. waiting for functionality and implementation.....*/
    [Header("Buttons")]
    [SerializeField] Button Control1;
    [SerializeField] Button Control2;
    [SerializeField] Button Control3;

    private float percentHealth = 0;
    private float percentStamina = 0;

    // Parts
    [Header("Parts")]
    [SerializeField] GameObject statsBuilding;
    [SerializeField] GameObject barsBuilding;
    [SerializeField] GameObject statsPerson;
    [SerializeField] GameObject barsPerson;

    private bool isActiveBuilding; // ToDo Need Rename... Asi by mel by byt enum a podle toho co je za typ se aktivuje cast prefabu..

    // inventory is missiong ... Todo. (if designer will want to.)

    #endregion

    #region Properity
    public float GetPercentHelth { get => this.percentHealth; }
    public string GetName { get => this.characterName.text.ToString(); }
    //  public int MilitaryValue { get => int.Parse(this.militaryValue.text); }
    public int GetKarma { get => int.Parse(this.karmaValue.text); }

    public int GetLevel { get => int.Parse(this.characterLevel.text); }

    public float GetHealth { get => int.Parse(this.characterLevel.text); }

    public Text SetCurrentActivity { set { currentActivity.text = value.text; } }

    public bool IsBuildingInView
    { 
        set 
        {
            if (isActiveBuilding == value)
                return;

            if(value == true)
            {
                statsBuilding.SetActive(true);
                barsBuilding.SetActive(true);
                statsPerson.SetActive(false);
                statsPerson.SetActive(false);
            }
            else
            {
                statsBuilding.SetActive(false);
                barsBuilding.SetActive(false);
                statsPerson.SetActive(true);
                statsPerson.SetActive(true);
            }
        } 
    }

    #endregion

    #region Private Methods
    private void SetSpecialistImage(Specialists spec)
    {
        this.specialistImage.sprite = spec.GetSprite();
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
    public void SetAll(Specialists spec)
    {
        SetSpecialistImage(spec);
        CalcHealtandStamina(spec);
        SetStatsPanel(spec);
    }
    #endregion
}
