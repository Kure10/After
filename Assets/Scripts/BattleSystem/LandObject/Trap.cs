using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : WalkableObject, IDamageable
{

    [SerializeField] int _maxHealth;
    [SerializeField] int _currentHealth = 0;
    [SerializeField] int _threat = 0;
    [SerializeField] int _trapDamage = 2;

    public int GetThreat { get { return _threat; } }

    public int GetCurrentHealth { get { return _currentHealth; } }

    public int GetMaxHealth { get { return _maxHealth; } }

    public bool IsBattleObjectDead()
    {
        throw new System.NotImplementedException();
    }

    public void ReceivedDamage(int amountDamage)
    {
        throw new System.NotImplementedException();
    }

    public override void PerformeAction(Unit unit, out bool stopper)
    {
        unit.ReceivedDamage(_trapDamage);
        stopper = true;
        unit.DecreaseMovementPoints(3);
        Destroy(this.gameObject, 2f);
    }
}
