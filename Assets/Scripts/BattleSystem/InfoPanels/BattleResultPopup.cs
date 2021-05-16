using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultPopup : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] private ItemCreater itemCreator;

    [Header("Prefabs")]
    [SerializeField] private GameObject _panelSpecialist;
    [SerializeField] private GameObject _itemSlot;
    [SerializeField] public GameObject _itemPrefab;

    [Header("Panels")]
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] public GameObject _lootPanel;

    [Header("Holder")]
    [SerializeField] private RectTransform _specHolder;
    [SerializeField] private Transform _slotHolder;

    [SerializeField] private List<Transform> _playerUnitHolder = new List<Transform>();
    [SerializeField] private List<Transform> _enemyUnitHolder = new List<Transform>();

    [Header("Holder")]
    [SerializeField] private Button _lootButton;
    [SerializeField] private Button _exitButton;

    private void Awake()
    {
        _lootButton.onClick.AddListener(OnPressLoot);
        _exitButton.onClick.AddListener(OnPressExit);
    }

    public void OnPressLoot()
    {
        _resultPanel.SetActive(false);
        _lootPanel.SetActive(true);
    }

    public void OnPressExit ()
    {
        // Battle Is over :. 
        // refresh battle some shit what ever
    }

    public void ShowBattleResult()
    {
        this.gameObject.SetActive(true);

    }

    public void InitPlayerUnits(List<Unit> battleUnits , GameObject unitPrefab)
    {
        int i = 0;
        int j = 0;
        int k = 0;

        foreach (Unit unit in battleUnits)
        {
            int row = 0;
            unit.gameObject.SetActive(true);

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


    public void InicializedStartInventory(List<ItemBlueprint> blueprits)
    {
        int size = 30;
        int count = blueprits.Count;

        // init Slots
        for (int i = 0; i < size; i++)
        {
            // create and set slot
            GameObject slot = Instantiate(_itemSlot);
            slot.transform.SetParent(_specHolder);
            slot.transform.localScale = new Vector3(1f, 1f, 1f);
            ItemSlot itemSlot = slot.GetComponent<ItemSlot>();

            // create and set item
            if (i < count)
            {
                ItemBlueprint blueprint = blueprits[i];

                GameObject game = itemCreator.CreateItemByType(blueprint, _itemPrefab);
                var item = game.GetComponent<Item>();

                if (item == null) // Todo Res and None type.. 
                {
                    Debug.LogWarning("somewhere is mistake Error in Inventory");
                    item = game.AddComponent<Item>();
                    item.SetupItem(blueprint.name, blueprint.Type, blueprint.Sprite);
                }

                item.MySlot = itemSlot;

                game.GetComponent<DragAndDropHandler>().InitDragHandler();

                itemSlot.SetSlot(i, game, item);
            }
            else
                itemSlot.SetSlot(i, null, null);

        }
    }

    public void InicializedCharacter(Character character)
    {
        GameObject ga = Instantiate(_panelSpecialist);
        ga.transform.SetParent(_specHolder);
        ga.transform.localScale = new Vector3(1f, 1f, 1f);
        uWindowSpecialist uWindowSpec = ga.GetComponent<uWindowSpecialist>();
        uWindowSpec.CharacterInWindow = character;
        uWindowSpec.SetAll(character);

        // specInGame.Add(uWindowSpec);

        List<SpecInventorySlot> slots = uWindowSpec.GetCharacterSlots();
        //character.SetCharacterSlots = slots;

        List<SpecInventorySlot> backpackSlots = uWindowSpec.GetCharacterBackpackSlots();
        //character.SetCharacterBackPackSlots = backpackSlots;

        // todo onitem change  pro backpack

        foreach (SpecInventorySlot slot in slots)
        {
            slot.OnItemChangeCallBack += character.OnItemChange;
            //DragAndDropManager.Instantion.OnItemResponseReaction += OnItemDragResponce;

            // Todo..
            if (slot.GetFirstSlotType == ItemBlueprint.ItemType.BagSpec || slot.GetSecondSlotType == ItemBlueprint.ItemType.BagSpec)
            {
                slot.OnOpenBackPack += uWindowSpec.OpenBackpackInventory;
              //  slot.OnOpenBackPack += RebuildLayout;
                slot.OnCloseBackPack += uWindowSpec.CloseBackpackInventory;
              //  slot.OnCloseBackPack += RebuildLayout;
            }

        }
    }
}
