using System;
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
    [SerializeField] Text workingSpecOnBuilding;

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

    // inventory is missiong ... Todo. (if designer will want to.)

    #endregion

    #region Properity

    void IsBuildingSelected()
    {
            if (building != null)
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

    private Building building;
    private Character character;
    #endregion

    #region Private Methods
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

    private void CalcHealtandStamina(Character character)
    {
        this.percentHealth = character.LifeEnergy.PercentHealth;
        this.percentStamina = character.LifeEnergy.PercentStamina;
        healthBar.transform.localScale = new Vector3(character.LifeEnergy.PercentHealth / 100, 1f, 1f);
        staminaBar.transform.localScale = new Vector3(character.LifeEnergy.PercentStamina / 100, 1f, 1f);
       
    }

    private void CalcProgressAndState(Building b)
    {
        int result = 100 - (int)((b.TimeToBuildRemaining/ b.blueprint.TimeToBuild) * 100);
        this.progressBarText.text = result.ToString();
        this.stateBarText.text = SetBuildingState((int)b.State);
        this.image.sprite = building.blueprint.Sprite;
        backgroundImage.color = building.blueprint.BackgroundColor;
        
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

    private void SetStatsPanel(BuildingBlueprint build)
    {
        buildingName.text = build.Name;
    }

    private void SetSpecialist(List<Character> specOnBuilding)
    {
        foreach (GameObject item in specInfoPlaceHolders.ToArray())
        {
            Destroy(item);
        }

        specInfoPlaceHolders.Clear();

        foreach (Character character in specOnBuilding)
        {
            GameObject specView = Instantiate(specialistPreafab, specialistHolder.transform);
            var window = specView.GetComponent<uWindowSpecialist>();
            if (window != null)
                window.SetAll(character);

            specInfoPlaceHolders.Add(specView);
        }
    }

    private void SetWorkingCharactersOnBuilding(int workingCharactersOnBuilding, int maxCharactersWorking)
    {
        this.workingSpecOnBuilding.text = workingCharactersOnBuilding + "/" + maxCharactersWorking;
    }

    #endregion

    #region Public Methods
    public void SetAll(Character spec)
    {
        this.building = null;
        character = spec;
        IsBuildingSelected();
    }

    public void SetAll(Building building)
    {
        this.building = building;
        this.character = null;
        SetImage(building.blueprint);
        SetStatsPanel(building.blueprint);
        // set building Projekt in future
        // set Spec working in building in future.
        IsBuildingSelected();
        var workers = building.getWorkers();
        var charsInBuilding = new List<Character>();
        foreach (Worker character in workers)
        {
            charsInBuilding.Add(character.character);
        }
        SetSpecialist(charsInBuilding);
        SetWorkingCharactersOnBuilding(8,9);
    }

    public void Update()
    {
        if (building != null)
        {
            CalcProgressAndState(building);
        }

        if (character != null)
        {
            SetImage(character.GetBlueprint());
            CalcHealtandStamina(character);
            SetStatsPanel(character);
            currentActivity.text = character.State;
        }
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
}
