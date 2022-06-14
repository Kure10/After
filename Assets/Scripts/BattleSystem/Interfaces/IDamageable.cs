using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void ReceivedDamage(int amountDamage);

    int GetThreat { get; }

    int GetCurrentHealth { get; }

    int GetMaxHealth { get; }

    bool IsBattleObjectDead();
}
