using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DestroyAbleObstacle : Obstacle , IDamageable 
{
    [SerializeField] int _maxHealth;
    private int _currentHealth = 0;

    public int GetCurrentHealth { get { return _currentHealth; } }

    public void ReceivedDamage(int amountDamage)
    {
        _currentHealth = _currentHealth - Mathf.Abs(amountDamage);

        if (_currentHealth < 0)
            _currentHealth = 0;
        if (_currentHealth > _maxHealth)
            _currentHealth = _maxHealth;


        _windowObstacle.UpdateHealthBar(_currentHealth, _maxHealth);
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Init()
    {
        _maxHealth = _currentHealth;
        base.Init();
    }
}
