using System;
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
    [SerializeField] Text healthValueText;
    [SerializeField] Text staminaValueText;
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

    private Character _character;

    #endregion

    #region Properity
    public Character CharacterInWindow { get { return this._character; } /*set { _character = value; }*/ }
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

        var currentLife = (int)character.LifeEnergy.CurrentLife;
        var currentStamina = (int)character.LifeEnergy.CurrentStamina;
        
        if(healthValueText != null )
            healthValueText.text = $"{currentLife} / {character.LifeEnergy.MaxLife}";

        if (staminaValueText != null)
            staminaValueText.text = $"{currentStamina} / {character.LifeEnergy.MaxStamina}";
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
        this._character = character;
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

    public void RefreshCharacterInfo(bool UpdatePanel = true)
    {
        if (_character == null)
            return;

        CalcHealtandStamina(_character);

        if(UpdatePanel)
        {
            if (_character.GetBlueprint().IsOnMission)
            {
                ActivateCoverPanel("Character is on Mission");
            }
            else
            {
                DeactivateCoverPanel();
            }
        }

    }

    public void PopulateItemSlots(Character character, bool disableDrag)
    {
        ItemCreater itemCreator = new ItemCreater();

        foreach (Item item in character.GetInventory)
        {
            if (item == null)
                continue;

            GameObject newGameObject = itemCreator.CreateItemFromItem(item , itemPrefab);
            Item newItem = newGameObject.GetComponent<Item>();
           
            DragAndDropHandler dragHandler = newGameObject.GetComponent<DragAndDropHandler>();
            dragHandler._disableDrag = disableDrag;

            for (int i = 0; i < characterSlots.Count; i++)
            {
                bool slotHasType = false;
                foreach (ItemBlueprint.ItemType slotType in characterSlots[i].GetSlotTypes)
                {
                    if(slotType == newItem.Type)
                    {
                        slotHasType = true;
                    }
                }

                if(slotHasType)
                {
                    if (characterSlots[i].IsEmpty)
                    {
                        characterSlots[i].SetSlot(newGameObject, newItem);
                        newItem.MySlot = characterSlots[i];
                        break;
                    }
                }

            }

            if (!disableDrag)
            {
                dragHandler.InitDragHandler();
            }
        }
    }

    public void PopulateBackpackItemSlots(Character character, bool disableDrag)
    {
        ItemCreater itemCreator = new ItemCreater();

        int j = 0;
        foreach (Item backpackItem in character.GetBackpackInventory)
        {
            if (backpackItem == null)
            {
                j++;
                continue;
            }

            GameObject newGameObject = itemCreator.CreateItemFromItem(backpackItem, itemPrefab);
            Item newItem = newGameObject.GetComponent<Item>();

            DragAndDropHandler dragHandler = newGameObject.GetComponent<DragAndDropHandler>();
            dragHandler._disableDrag = disableDrag;

            if (backPackSlots[j].IsEmpty)
            {
                backPackSlots[j].SetSlot(newGameObject, newItem);
                newItem.MySlot = backPackSlots[j];
            }

            if (!disableDrag)
            {
                dragHandler.InitDragHandler();
            }

            j++;
        }

        Backpack backPack = character.GetCharacterBackpack;
        if(backPack != null)
        {
            OpenBackpackInventory(backPack.Capacity);
        }
        else
        {
            CloseBackpackInventory();
        }
    }

    public void CleanAllItemSlots ()
    {
        foreach (SpecInventorySlot slot in characterSlots)
        {
            slot.CleanSlot();
        }

        foreach (SpecInventorySlot slot in backPackSlots)
        {
            slot.CleanSlot();
        }
    }

    public void AddActionsOnItemClicked( UnityAction action)
    {
        foreach (SpecInventorySlot slot in characterSlots)
        {
            if(!slot.IsEmpty)
            {
                if (slot.CurrentItem.item is Backpack back)
                {
                    slot.OnOpenBackPack += OpenBackpackInventory;
                    slot.OnGridSizeChange += action;
                    slot.OnCloseBackPack += CloseBackpackInventory;

                    
                    // slot.AddBackpackAction(OpenAndCloseBackpack, action, back.Capacity);
                }
                else if (slot.CurrentItem.item is Weapon weapon)
                {
                  // todo
                }
                else if (slot.CurrentItem.item is Armor armor)
                {
                    // todo
                }
                else if (slot.CurrentItem.item is ActiveItem activeItem)
                {
                    // todo
                }
                else
                {
                    // todo
                }
            }  
        }
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

        //TODO commented i dont know why it should be there
        //var specController = this.gameObject.GetComponent<uWindowSpecController>();
        //if (specController != null)
        //{
        //    specController.RefreshGrid();
        //}
    }

    public void CloseBackpackInventory()
    {
        backPackGameObject.SetActive(false);

        foreach (GameObject collum in backPackCollums)
        {
            collum.SetActive(false);
        }

        //TODO commented i dont know why it should be there
        //var specController = this.gameObject.GetComponent<uWindowSpecController>();
        //if (specController != null)
        //{
        //    specController.RefreshGrid();
        //}
    }

    #endregion
}

