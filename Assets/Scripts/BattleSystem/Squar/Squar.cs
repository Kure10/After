using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;


[System.Serializable]
public class CursorColorEvent : UnityEvent<Squar>
{
    //public bool canMove = false;
    //public bool canAttack = false;
    //public bool canHeal = false;
    public BattleController.BattleAction action = BattleController.BattleAction.Attack;
}

public class Squar : MonoBehaviour
{
    public int xCoordinate = 0;
    public int yCoordinate = 0;

    [Header("Holder For Occupied Objects")]
    [SerializeField] private GameObject container;
    [SerializeField] private GameObject waContainer;
    [SerializeField] private GameObject terrainContainer;

    [Header("Range Marks")]
    [SerializeField] public GameObject inRangeBackground;
    [SerializeField] private GameObject actionMark;

    [Header("Attack Range Borders")]
    [SerializeField] public GameObject leftBorder;
    [SerializeField] public GameObject rightBorder;
    [SerializeField] public GameObject upBorder;
    [SerializeField] public GameObject downBorder;

    [Header("Attack Move Costs")]
    [SerializeField] private Text gCost;
    [SerializeField] private Text hCost;
    [SerializeField] private Text fCost;

    [Header("testing")]
    [SerializeField] private GameObject shootPath;
    [SerializeField] private GameObject shootPathLesserThan33;
    [SerializeField] private GameObject shootPathNoPoints;

    [Space]

    private Unit _unitInSquar;
    private Obstacle _obstacle;
    private WalkableObject _walkAbleObject;
    private WalkableObject _walkAbleTerrain;


    public bool isInMoveRange = false;
    public bool isInAttackReach = false;

    public CursorColorEvent CursorEvent;

    [Header("Color for Curzor")]
    public Color isInMoveRangeColor;
    public Color isOutOfMoveRangeColor;
    public Color canAttackColor;
    public Color heal;

    public Action action;

    private Image actionSprite = null;

    private BattlePathFinding.AAlgoritmStats _pathStats;

    public GameObject GetContainer { get { return container; } }
    public GameObject GetWalkAbleContainer { get { return waContainer; } }
    public GameObject GetTerrainContainer { get { return terrainContainer; } }
    public bool IsSquearBlocked
    {
        get
        {
            return _unitInSquar != null || _obstacle != null || _walkAbleObject != null;
        }
    }

    public bool IsSquareWalkAble
    {
        get
        {
            return _unitInSquar == null && _obstacle == null;
        }
    }

    public Unit UnitInSquar { get { return _unitInSquar; } set { _unitInSquar = value; } }

    public Obstacle GetObstacle { get { return _obstacle; } }

    public T GetObjectFromSquareGeneric<T>() where T : class
    {
        var obj = container.GetComponentInChildren<T>();

        if (obj != null)
            return obj;

        obj = waContainer.GetComponentInChildren<T>();

        if (obj != null)
            return obj;

        return terrainContainer.GetComponentInChildren<T>();
    }

    public bool CanShootThrough 
    { 
        get 
        {
            if (_obstacle != null)
            {
                return _obstacle.CanShootThrough;
            }

            return true;
        }
    }

    public BattlePathFinding.AAlgoritmStats PathStats { get { return _pathStats; } }

    private void Awake()
    {
        actionSprite = actionMark.GetComponent<Image>();
        _pathStats = new BattlePathFinding.AAlgoritmStats();
    }

    public void SetObject(GameObject gameObject, int posX, int posY, Sprite sprite = null)
    {
        IBattleObject battleObject = null;
        bool obs = Instantiate(gameObject).TryGetComponent(out battleObject);

        if(!obs)
        {
            Debug.LogError("You are trying instantiate non battle GameObject");
            return;
        }

        if (battleObject is Obstacle obstacle)
        {
            _obstacle = obstacle;
            obstacle.gameObject.transform.SetParent(container.transform);
            obstacle.Init(posX, posY);
        }

        if (battleObject is WalkableObject walkAble)
        {
            if(walkAble.IsTerrain)
            {
                _walkAbleTerrain = walkAble;
                walkAble.gameObject.transform.SetParent(terrainContainer.transform);
                walkAble.Init(posX, posY);
            }
            else
            {
                _walkAbleObject = walkAble;
                walkAble.gameObject.transform.SetParent(waContainer.transform);
                walkAble.Init(posX, posY);
            }

        }
    }

    public WalkableObject CheckTrigerObjectInSquare()
    {
        if (_walkAbleObject != null && _walkAbleObject.IsWalkAbleTriger)
        {
            return _walkAbleObject;
        }
     
        if (_walkAbleTerrain != null && _walkAbleTerrain.IsWalkAbleTriger)
        {
            return _walkAbleTerrain;
        }

        return null;
    }

    public Vector2Int GetCoordinates()
    {
        return new Vector2Int(xCoordinate, yCoordinate);
    }

    public void SetCoordinates(int x, int y)
    {
        xCoordinate = x;
        yCoordinate = y;

        this.name = $"Squar  {x}:{y}";
    }

    // Events actions
    public void InitEvent(UnityAction<Squar> call)
    {
        if (CursorEvent == null)
            CursorEvent = new CursorColorEvent();

        CursorEvent.AddListener(call);
    }

    public void DisableAttackBorders()
    {
        leftBorder.SetActive(false);
        rightBorder.SetActive(false);
        upBorder.SetActive(false);
        downBorder.SetActive(false);
    }

    public void DestroyObstacleInSquare()
    {
        if(_obstacle != null)
        {
            Destroy(_obstacle.gameObject, 0.5f);
            _obstacle = null;
        }
    }

    // Event Trigger from Unity
    public void OnPointerEnter()
    {
        CursorEvent.Invoke(this);

        if (actionSprite == null)
            actionSprite = actionMark.GetComponent<Image>();

        if (actionSprite == null)
            return;

        if (CursorEvent.action == BattleController.BattleAction.Attack)
        {
            actionSprite.color = canAttackColor;
        }
        else if (CursorEvent.action == BattleController.BattleAction.Heal)
        {
            actionSprite.color = canAttackColor;
        }
        else
        {
            if (CursorEvent.action == BattleController.BattleAction.Move)
            {
                actionSprite.color = isInMoveRangeColor;
            }
            else
            {
                actionSprite.color = isOutOfMoveRangeColor;
            }
        }

        actionMark.SetActive(true);
    }

    public void SetActionMark(Sprite sprite)
    {
        actionSprite.sprite = sprite;
    }

    // Event Trigger from Unity
    public void OnPointerExit()
    {
        actionMark.SetActive(false);
    }

    // Testing AA Algoritm
    public void ShowCosts(BattlePathFinding.AAlgoritmStats stats)
    {
        gCost.text = stats.GCost.ToString();
        hCost.text = stats.HCost.ToString();
        fCost.text = stats.FCost.ToString();
    }

    public void TestingShowShootPath(bool show)
    {
        shootPath.SetActive(show);
    }

    public void TestingShowShootPathLesserThan(bool show)
    {
        shootPathLesserThan33.SetActive(show);
    }

    public void TestingShowShootPathNopoints(bool show)
    {
        shootPathNoPoints.SetActive(show);
    }
}