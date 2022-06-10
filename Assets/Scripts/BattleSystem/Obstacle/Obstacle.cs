using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WindowObstacle))]
public class Obstacle : MonoBehaviour
{
    [SerializeField] Sprite _sprite;
    [SerializeField] private bool _canShootThrough = false;

    protected WindowObstacle _windowObstacle;

    public bool CanShootThrough { get { return _canShootThrough; } }

    public Sprite GetSprite { get{ return _sprite; } }

    protected virtual void Awake()
    {
        _windowObstacle = GetComponent<WindowObstacle>();
    }

    public virtual void Init()
    {
        _windowObstacle.UpdateStats(this);
    }
}


