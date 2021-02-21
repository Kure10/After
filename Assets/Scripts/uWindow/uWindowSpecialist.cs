﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


// TOdo jsou asi tri prefaby na tento script.  No chce to vymyslet lepe..
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

    [Header("Inventory Stats")]
    [SerializeField] List<SpecInventorySlot> characterSlots = new List<SpecInventorySlot>();

    [SerializeField] List<SpecInventorySlot> backPackSlots = new List<SpecInventorySlot>();
    [Space]
    [SerializeField] GameObject backPackGameObject;

    [SerializeField] List<GameObject> backPackCollums = new List<GameObject>();

    [Space]
    [SerializeField] GameObject BackPack;

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

    [Header("Item Prefab")]
    [SerializeField] GameObject itemPrefab;

    private float percentHealth = 0;
    private float percentStamina = 0;

    private bool isOpenBackPack = false;

    private Character _character;

    #endregion

    #region Properity
    public Character CharacterInWindow { get { return this._character; } set { _character = value; } }
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

    public List<SpecInventorySlot> GetCharacterBackpackSlots()
    {
        return this.backPackSlots;
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

    // Todo
    public void RefreshCharacterInfo()
    {
        if (_character == null)
            return;

        CalcHealtandStamina(_character);
    }

    public void PopulateItemSlots(Character character)
    {
        ItemCreater itemCreator = new ItemCreater();
        foreach (Item item in character.GetInventory)
        {
            if (item == null)
                continue;

            GameObject newGameObject = Instantiate(itemPrefab);
            Item newColection = itemCreator.CreateObjectForInventory(item, newGameObject);

            DragAndDropHandler dragHandler = newGameObject.GetComponent<DragAndDropHandler>();
            dragHandler._disableDrag = true; // disable drag..

            for (int i = 0; i < characterSlots.Count; i++)
            {
                if (characterSlots[i].GetFirstSlotType == newColection.Type || characterSlots[i].GetSecondSlotType == newColection.Type)
                {
                    if (characterSlots[i].IsEmpty)
                    {
                        characterSlots[i].SetSlot(newGameObject, newColection);
                        newColection.MySlot = characterSlots[i];
                        break;
                    }
                }
            }
        }
    }

    public void AddActionsOnItemClicked( UnityAction action)
    {
        foreach (SpecInventorySlot item in characterSlots)
        {
            item.AddAction(OpenAndCloseBackpack, action);
        }
    }

    // Open and Close.. backpack
    public void OpenAndCloseBackpack(int backPackCapacity)
    {
        if (backPackCapacity <= 0)
            return;

        if (backPackGameObject != null)
            backPackGameObject.SetActive(!backPackGameObject.activeSelf);

        var count = backPackCapacity / 2;

        if (isOpenBackPack)
        {
            for (int i = 0; i < count; i++)
            {
                backPackCollums[i].SetActive(false);
            }
            isOpenBackPack = false;
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                backPackCollums[i].SetActive(true);
            }
            isOpenBackPack = true;
        }

        var tmp = this.transform.GetComponentInParent<Transform>();

        tmp.SetAsFirstSibling();
        // reorder.. 
    }

    public void OpenBackpackInventory(int backPackCapacity)
    {
        // todo special case
        if (backPackCapacity <= 0)
            return;

        if (backPackGameObject != null)
            backPackGameObject.SetActive(true);

        int i = 0;
        foreach (GameObject collum in backPackCollums)
        {
            if(i < backPackCapacity)
                collum.SetActive(true);
            else
                collum.SetActive(false);

            i++;
        }

        var specController = this.gameObject.GetComponent<uWindowSpecController>();
        if(specController != null)
        {
            specController.RefreshGrid();
        }
    }

    public void CloseBackpackInventory()
    {
        backPackGameObject.SetActive(false);

        foreach (GameObject collum in backPackCollums)
        {
            collum.SetActive(false);
        }

        var specController = this.gameObject.GetComponent<uWindowSpecController>();
        if (specController != null)
        {
            specController.RefreshGrid();
        }
    }

    #endregion
}

