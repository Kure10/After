using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WindowObstacle))]
public class Obstacle : MonoBehaviour , IClickAble
{
    [SerializeField] Sprite _sprite;
    [SerializeField] private bool _canShootThrough = false;

    protected WindowObstacle _windowObstacle;

    private BattleGridController.PositionSquar _currentPos;
    public bool CanShootThrough { get { return _canShootThrough; } }

    public Sprite GetSprite { get{ return _sprite; } }

    public string GetName { get { return this.name; } }

    public int GetXPosition { get { return _currentPos.XPosition; } }

    public int GetYPosition { get { return _currentPos.YPosition; } }

    protected virtual void Awake()
    {
        _windowObstacle = GetComponent<WindowObstacle>();
    }

    public virtual void Init(int posX, int posY)
    {
        _currentPos.XPosition = posX;
        _currentPos.YPosition = posY;
        _windowObstacle.UpdateStats(this);
    }

}


