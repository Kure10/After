using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;


[System.Serializable]
public class CursorColorEvent : UnityEvent<Squar>
{
    public bool canMove = false;
    public bool canAttack = false;
}

public class Squar : MonoBehaviour
{
    public int xCoordinate = 0;
    public int yCoordinate = 0;

    [Header("Holder For Occupied Objects")]
    [SerializeField] public GameObject container;

    [Header("Obstacle")]
    [SerializeField] public GameObject stoneObstacle;

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

    public Unit _unitInSquar;

    public bool isInMoveRange = false;
    public bool isInAttackReach = false;

    public CursorColorEvent CursorEvent;

    [Header("Color for Curzor")]
    public Color isInMoveRangeColor;
    public Color isOutOfMoveRangeColor;
    public Color canAttackColor;

    public Action action;

    private bool _isSquarBlocked = false;

    private BattlePathFinding.AAlgoritmStats _pathStats;

    public bool IsSquearBlocked
    {
        get { return _isSquarBlocked; }
        set
        {
            _isSquarBlocked = value;
            stoneObstacle.SetActive(value);
        }
    }

    public Unit UnitInSquar { get { return _unitInSquar; } set { _unitInSquar = value; } }

    public BattlePathFinding.AAlgoritmStats PathStats { get { return _pathStats; } }

    private void Awake()
    {
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

    // Event Trigger from Unity
    public void OnPointerEnter()
    {
        CursorEvent.Invoke(this);
        var cursorImg = actionMark.GetComponent<Image>();

        if (cursorImg == null)
            return;

        if (CursorEvent.canAttack)
        {
            cursorImg.color = canAttackColor;
        }
        else
        {
            if (CursorEvent.canMove)
            {
                cursorImg.color = isInMoveRangeColor;
            }
            else
            {
                cursorImg.color = isOutOfMoveRangeColor;
            }
        }

        actionMark.SetActive(true);
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



