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

    // inventory is missiong ... Todo. (if designer will want to.)

    #endregion



    #region Properity

    public Text SetCurrentActivity { set { currentActivity.text = value.text; } }

    #endregion

    #region Methods
    private void SetSpecialistImage(Specialists spec)
    {
        specialistImage.sprite = spec.GetSprite();
    }

    private void CalcHealtandStamina(Specialists spec)
    {
        healthBar.transform.localScale = new Vector3(spec.PercentHealt / 100, 1f, 1f);
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

    public void SetAll(Specialists spec)
    {
        SetSpecialistImage(spec);
        CalcHealtandStamina(spec);
        SetStatsPanel(spec);
    }
    #endregion
}
