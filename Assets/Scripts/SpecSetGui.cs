using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecSetGui : MonoBehaviour
{
    /*  old */
    Transform firstPanel;

    Transform panelImage;

    Transform panelStats;

    /*  New */

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

    [Header("Buttons")]
    [SerializeField] Button Control1;
    [SerializeField] Button Control2;
    [SerializeField] Button Control3;

    /*
     chyby inventar a buttony co delaji by meli byt asi dynamycke. Možna se jejich funkčnost bude jmenit.

        -- ovladani buttonu atd. Jeste nevíme co budou delat. A ani co bude presne zobrazovat probíhajicí cinnost

        ToDo  dodelat property

    */

    // Start is called before the first frame update
    void Start()
    {

    }

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

        // ToDo Jeste nevim jak to udelam a za jakych podminek se meni stav.

       // karmaValue.text = "Todo Nezapomen dodelat :D";
    }

    public void SetAll(Specialists spec)
    {
        SetSpecialistImage(spec);
        CalcHealtandStamina(spec);
        SetStatsPanel(spec);
    }

}
