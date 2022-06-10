using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using ItemCreating;

public class BattleResultPopup : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _panelSpecialist;
    [SerializeField] private GameObject _itemSlot;
    [SerializeField] public GameObject _itemPrefab;

    [Header("Panels")]
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] public GameObject _lootPanel;

    [Header("Holder")]
    [SerializeField] private RectTransform _specHolder;

    [SerializeField] private List<Transform> _playerUnitHolder = new List<Transform>();
    [SerializeField] private List<Transform> _enemyUnitHolder = new List<Transform>();

    [Header("Buttons")]
    [SerializeField] private Button _lootButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _backButton;

    [Header("ItemSlots")]
    [SerializeField] private List<ItemSlot> _itemSlots = new List<ItemSlot>();

    private int _sortCategory = 0;

    // Buttons
    public void OnPressLoot()
    {
        _resultPanel.SetActive(false);
        _lootPanel.SetActive(true);

        _sortCategory = -1;
        OnPressSort(1);
    }
    // Buttons
    public void OnPressExit ()
    {
        // clean this panel
        // reset it
        this.gameObject.SetActive(false);
    }
    // Buttons
    public void OnPressBack()
    {
        _resultPanel.SetActive(true);
        _lootPanel.SetActive(false);
    }
    // Buttons
    public void OnPressSort(int currentSortCategory)
    {
        List<uWindowSpecialist> specList = new List<uWindowSpecialist>();
        specList.AddRange(_specHolder.gameObject.GetComponentsInChildren<uWindowSpecialist>());

        if (_sortCategory == currentSortCategory)
        {
            foreach (var spec in specList)
            {
                spec.transform.SetAsFirstSibling();
            }
        }
        else
        {
            _sortCategory = currentSortCategory;

            switch (_sortCategory)
            {
                case 0:
                    specList = specList.OrderBy(x => x.GetKarma).ToList();
                    break;
                case 1:
                    specList = specList.OrderBy(x => x.GetName).ToList();
                    break;
                case 2:
                    specList = specList.OrderBy(x => x.GetLevel).ToList();
                    break;
                case 3:
                    specList = specList.OrderBy(x => x.GetPercentHelth).ToList();
                    break;
                default:
                    Debug.Log("Error in sorting wrong sortCategory");
                    break;
            }

            foreach (var spec in specList)
            {
                spec.transform.SetAsFirstSibling();
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_specHolder);
    }

    public void InicializedControlles(UnityAction action)
    {
        _lootButton.onClick.AddListener(OnPressLoot);
        _exitButton.onClick.AddListener(action);
        _backButton.onClick.AddListener(OnPressBack);
    }

    public void ShowBattleResult(List<Unit> battleUnits, GameObject unitPrefab , List<Character> characterInBattle)
    {
        InitUnits(battleUnits, unitPrefab);
        InicializedCharacter(characterInBattle);

        this.gameObject.SetActive(true);
    }

    public void InitBeforeBattleStart()
    {
        this.gameObject.SetActive(false);
        _resultPanel.SetActive(true);
        _lootPanel.SetActive(false);

        CleanLootPanel();
        CleanCharacterList();
    }

    public void InicializedStartInventory(List<ItemBlueprint> blueprits)
    {
        for (int i = 0; i < blueprits.Count; i++)
        {
            ItemBlueprint itemBlueprint = blueprits[i];

            GameObject game = ItemCreater.ItemCreator.CreateItemByType(itemBlueprint, _itemPrefab);
            var item = game.GetComponent<Item>();

            if (item == null) // Todo Res and None type.. 
            {
                Debug.LogWarning("somewhere is mistake Error in Inventory");
                item = game.AddComponent<Item>();
                item.SetupItem(itemBlueprint);
            }

            item.MySlot = _itemSlots[i];
            game.GetComponent<DragAndDropHandler>().InitDragHandler();
            _itemSlots[i].SetSlot(i, game, item);
        }
    }

    public void RebuildLayout(int i)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_specHolder);
    }

    public void RebuildLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_specHolder);
    }

    private void InitUnits(List<Unit> battleUnits, GameObject unitPrefab)
    {
        int i = 0;
        int j = 0;
        int k = 0;

        foreach (Unit unit in battleUnits)
        {
            int row = 0;
            unit.gameObject.SetActive(true);
            unit.UpdateData();

            if (unit._team == Unit.Team.Human)
            {
                row = i / 4;
                _playerUnitHolder[row].gameObject.SetActive(true);
                unit.gameObject.transform.SetParent(_playerUnitHolder[row]);
                i++;
            }

            if (unit._team == Unit.Team.Demon)
            {
                row = j / 5;
                _enemyUnitHolder[row].gameObject.SetActive(true);
                unit.gameObject.transform.SetParent(_enemyUnitHolder[row]);
                j++;
            }

            if (unit._team == Unit.Team.Neutral)
            {
                row = k / 4;
                _enemyUnitHolder[row].gameObject.SetActive(true);
                unit.gameObject.transform.SetParent(_playerUnitHolder[row]);
                k++;
            }
        }
    }

    private void InicializedCharacter(List<Character> characters)
    {
        foreach (Character character in characters)
        {
            GameObject gameObject = Instantiate(_panelSpecialist);
            gameObject.transform.SetParent(_specHolder);
            gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
            uWindowSpecialist window = gameObject.GetComponent<uWindowSpecialist>();
            window.SetAll(character);

            window.PopulateItemSlots(character,false);
            window.PopulateBackpackItemSlots(character,false);


            List<SpecInventorySlot> charSlots = window.GetCharacterSlots();
            //character.SetCharacterSlots = slots;
            List<SpecInventorySlot> backpackSlots = window.GetCharacterBackpackSlots();
            //character.SetCharacterBackPackSlots = backpackSlots;

            foreach (SpecInventorySlot slot in charSlots)
            {
                slot.OnItemChangeCallBack += character.OnItemChange;
                // DragAndDropManager.Instantion.OnItemResponseReaction += OnItemDragResponce;

                //Todo
                foreach (ItemBlueprint.ItemType slotType in slot.GetSlotTypes)
                {
                    if(slotType == ItemBlueprint.ItemType.BagSpec)
                    {
                        //slot.OnOpenBackPack += window.OpenBackpackInventory;
                        //slot.OnOpenBackPack += RebuildLayout;
                        //slot.OnCloseBackPack += window.CloseBackpackInventory;
                        //slot.OnCloseBackPack += RebuildLayout;
                    }
                }
            }

            foreach (SpecInventorySlot slot in backpackSlots)
            {
                slot.OnItemChangeCallBack += character.OnItemChange;
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_specHolder);
    }

    private void CleanLootPanel()
    {
        foreach (ItemSlot slot in _itemSlots)
        {
            slot.CleanSlot();
        }
    }

    private void CleanCharacterList()
    {
        foreach (Transform child in _specHolder.transform)
        {
            Destroy(child.gameObject);
        }
    }

}


