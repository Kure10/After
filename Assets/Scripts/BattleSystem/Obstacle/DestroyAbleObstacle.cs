using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DestroyAbleObstacle : Obstacle, IDamageable
{
    [SerializeField] int _maxHealth;
    [SerializeField] int _currentHealth = 0;

    [Header("Explosive stats")]
    [SerializeField] bool _isExplosive = false;
    [SerializeField] [Range(1, 10)] int _explosiveRange = 1;
    [SerializeField] [Range(1, 10)] int _explosiveDamage = 1;

    //Todo Obsticle Has some base threat -> Later according type of obstacle change threat
    [SerializeField] int _threat = 2;

    public bool IsExplosive { get { return _isExplosive; } }
    public int GetExplosiveRange { get { return _explosiveRange; } }
    public int GetExplosiveDamage { get { return _explosiveDamage; } }
    public int GetMaxHealth { get { return _maxHealth; } }
    public int GetCurrentHealth { get { return _currentHealth; } }
    public int GetThreat { get { return _threat; } }

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

    public override void Init(int posX, int posY)
    {
        _maxHealth = _currentHealth;
        base.Init(posX, posY);
    }

    public bool IsBattleObjectDead()
    {
        if (_currentHealth <= 0)
        {
            // animation
            // what ever call back///

            return true;
        }

        return false;
    }
}
