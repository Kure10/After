using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.ObjectModel;
using System.ComponentModel;

public class uWindowSelecctedObject : MonoBehaviour
{
    #region Fields

    [Header("Main Header")]
    [SerializeField] Image image;
    [SerializeField] Text characterName;
    [SerializeField] Text characterLevel;
    [SerializeField] Image backgroundImage;
    [SerializeField] Text buildingName;


    [Header("Stats")]
    [SerializeField] Transform healthBar;
    [SerializeField] Transform staminaBar;
    [SerializeField] Transform progressBar;
    [Space]
    [SerializeField] Text militaryValue;
    [SerializeField] Text socialValue;
    [SerializeField] Text technicianValue;
    [SerializeField] Text scientistValue;
    [SerializeField] Text karmaValue;
    [SerializeField] Text progressBarText;
    [SerializeField] Text stateBarText;
    [Header("Stats")]
    [SerializeField] Text currentActivity;

    [Header("Building")]
    [SerializeField] GameObject specialistHolder;
    [SerializeField] GameObject specialistPreafab;

    [SerializeField] List<GameObject> specInfoPlaceHolders = new List<GameObject>();
    /* For now buttons are not used.. waiting for functionality and implementation.....*/
    [Header("Buttons")]
    [SerializeField] Button Control1;
    [SerializeField] Button Control2;
    [SerializeField] Button Control3;

    // Parts
    [Header("Parts")]
    [SerializeField] GameObject statsBuilding;
    [SerializeField] GameObject barsBuilding;
    [SerializeField] GameObject statsPerson;
    [SerializeField] GameObject barsPerson;
    [SerializeField] GameObject buildersView;

    private float percentHealth = 0;
    private float percentStamina = 0;

    private bool isActiveBuilding;

   // private BindingList<Specialists> specWorkingOnBuilding = new BindingList<Specialists>();




 

    // inventory is missiong ... Todo. (if designer will want to.)

    #endregion

    #region Properity

    public Text SetCurrentActivity { set { currentActivity.text = value.text; } }

    public bool IsBuildingSelected
    {
        set
        {
            if (value == true)
            {
                statsBuilding.SetActive(true);
                barsBuilding.SetActive(true);
                statsPerson.SetActive(false);
                barsPerson.SetActive(false);
                buildersView.SetActive(true);
            }
            else
            {
                statsBuilding.SetActive(false);
                barsBuilding.SetActive(false);
                statsPerson.SetActive(true);
                barsPerson.SetActive(true);
                buildersView.SetActive(false);

            }
        }
    }

    private Building building;
    #endregion

    #region Private Methods

    private void Awake()
    {
        //specWorkingOnBuilding.ListChanged += delegate (object sender, ListChangedEventArgs args)
        //{
        //    nedasco();
        //};
    }

    private void SetImage(Specialists spec)
    {
        this.image.sprite = spec.Sprite;
        if (this.backgroundImage != null)
            this.backgroundImage.color = spec.SpecialistColor;
    }

    private void SetImage(BuildingBlueprint build)
    {
        this.image.sprite = build.Sprite;
    }

    private void CalcHealtandStamina(Specialists spec)
    {
        this.percentHealth = spec.PercentHealth;
        this.percentStamina = spec.PercentStamina;
        healthBar.transform.localScale = new Vector3(spec.PercentHealth / 100, 1f, 1f);
        staminaBar.transform.localScale = new Vector3(spec.PercentStamina / 100, 1f, 1f);
    }

    private void CalcProgressAndState(float remainingTime, float fullTimeToBuild, int state)
    {
        int result = 100 - (int)((remainingTime / fullTimeToBuild) * 100);
        this.progressBarText.text = result.ToString();
        this.stateBarText.text = SetBuildingState(state);
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

    private void SetStatsPanel(BuildingBlueprint build)
    {
        buildingName.text = build.Name;
    }

    private string SetBuildingState(int state)
    {
        switch (state)
        {
            case 0:
                return "Init";
            case 1:
                return "Designed";
            case 2:
                return "UnderConstruction";
            case 3:
                return "Build";

            default:
                return "Empty";
        }
    }



    #endregion

    #region Public Methods
    public void SetAll(Specialists spec)
    {
        this.IsBuildingSelected = false;
        this.building = null;
        SetImage(spec);
        CalcHealtandStamina(spec);
        SetStatsPanel(spec);
    }

    public void SetAll(Building building) // doplnit list..
    {
        this.IsBuildingSelected = true;
        this.building = building;
        SetImage(building.blueprint);
        SetStatsPanel(building.blueprint);
        // set building Projekt in future
        // SetSpecialist("List");
    }

    public void SetSpecialist(List<Specialists> specOnBuilding)
    {
        //foreach (var item in specInfoPlaceHolders)
        //{
        //    Destroy(item);
        //}

        specInfoPlaceHolders.Clear();
        
        foreach (Specialists spec in specOnBuilding)
        {
            GameObject specView = Instantiate(specialistPreafab, specialistHolder.transform);
            var window = specView.GetComponent<uWindowSpecialist>();
            if (window != null)
                window.SetAll(spec);

            specInfoPlaceHolders.Add(specView);
        }


    }

    public void Update()
    {
        if (building != null)
        {
            CalcProgressAndState(building.TimeToBuildRemaining, building.blueprint.TimeToBuild, (int) building.State);
        }
    }

    #endregion
}
