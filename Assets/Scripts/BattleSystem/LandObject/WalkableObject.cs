using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkableObject : MonoBehaviour , IClickAble , IBattleObject
{
    [SerializeField] string myName;

    [SerializeField] Sprite _sprite;

    [SerializeField] bool _walkAbleTriger = false;

    [SerializeField] bool _isTerrain = false;

    protected WindowWalkableObject _windowWalkAble;

    private BattleGridController.PositionSquar _currentPos;

    public string GetName { get { return this.myName; } }

    public int GetXPosition { get { return _currentPos.XPosition; } }

    public int GetYPosition { get { return _currentPos.YPosition; } }

    public bool IsWalkAbleTriger
    {
        get { return _walkAbleTriger; } 
    }

    public bool IsTerrain
    {
        get { return _isTerrain; }
    }

    protected virtual void Awake()
    {
        _windowWalkAble = GetComponent<WindowWalkableObject>();
    }

    public void Init(int posX, int posY)
    {
        myName = "my name";
        _currentPos.XPosition = posX;
        _currentPos.YPosition = posY;
    }

    public virtual void ImmediatelyDestroy()
    {
        // DO nothing
    }

    public virtual void PerformeAction(Unit unit,out bool stopper)
    {
        stopper = false;
    }


}
