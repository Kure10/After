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

    [Header("Obstacle")]
    [SerializeField] private GameObject stoneObstacle;

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


    public bool isInMoveRange = false;
    public bool isInAttackReach = false;

    public CursorColorEvent CursorEvent;

    [Header("Color for Curzor")]
    public Color isInMoveRangeColor;
    public Color isOutOfMoveRangeColor;
    public Color canAttackColor;
    public Color heal;

    public Action action;

    private bool _isSquarBlocked = false;

    private Image actionSprite = null;

    private BattlePathFinding.AAlgoritmStats _pathStats;

    public GameObject GetContainer { get { return container; } }
    public bool IsSquearBlocked
    {
        get { return _isSquarBlocked; }
    }

    public Unit UnitInSquar { get { return _unitInSquar; } set { _unitInSquar = value; } }

    public Obstacle GetObstacle { get { return _obstacle; } }

    public T GetObjectFromSquareGeneric<T>() where T : class
    {
        return container.GetComponentInChildren<T>();
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

    public void SetObstacle(GameObject gameObject, int posX , int posY)
    {
        Obstacle obstacle = Instantiate(gameObject).GetComponent<Obstacle>();
        if(obstacle != null)
        {
            _obstacle = obstacle;
            obstacle.gameObject.transform.SetParent(container.transform);
            _isSquarBlocked = true;
            obstacle.Init(posX, posY);

        }
        else
        {
            Debug.Log("Error in Pobsticle PRefab");
        }
    }


    private void Awake()
    {
        actionSprite = actionMark.GetComponent<Image>();
        _pathStats = new BattlePathFinding.AAlgoritmStats();
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

    public void DestrpyObstacleInSquare()
    {
        if(_obstacle != null)
        {
            Destroy(_obstacle.gameObject, 0.5f);
            _obstacle = null;
            _isSquarBlocked = false;
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





