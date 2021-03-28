using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleType
{
    Testing,
    Standard,
    Ambush,
    HordeAttack
}


public class BattleStartData
{

    // Players In battle

    public BattlePlayerStartData playerData;

    public BattleAIStartData aiData;

    // BattleField data

    public BattleType battleType = BattleType.Standard;



}

public class BattlePlayerStartData
{

    public List<Unit> playerUnits = new List<Unit>();
}

public class BattleAIStartData
{
   public List<Unit> enemieUnits = new List<Unit>();


}
