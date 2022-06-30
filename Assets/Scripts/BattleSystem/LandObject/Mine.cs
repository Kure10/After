using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : WalkableObject, IDamageable
{
    [SerializeField] int _maxHealth;
    [SerializeField] int _currentHealth = 0;
    [SerializeField] int _threat = 0;
    [SerializeField] int _mineDamage = 6;
    public int GetThreat { get { return _threat; } }

    public int GetCurrentHealth { get { return _currentHealth; } }

    public int GetMaxHealth { get { return _maxHealth; } }

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

    public void ReceivedDamage(int amountDamage)
    {
        _currentHealth = _currentHealth - Mathf.Abs(amountDamage);

        if (_currentHealth < 0)
            _currentHealth = 0;
        if (_currentHealth > _maxHealth)
            _currentHealth = _maxHealth;

        _windowWalkAble.UpdateHealthBar(_currentHealth, _maxHealth);
    }

    public override void ImmediatelyDestroy()
    {
        base.ImmediatelyDestroy();
    }

    public override void PerformeAction(Unit unit, out bool stopper)
    {
        unit.ReceivedDamage(_mineDamage);
        stopper = true;
        unit.DecreaseMovementPoints(100);
        Destroy(this.gameObject, 2f);
    }
}
