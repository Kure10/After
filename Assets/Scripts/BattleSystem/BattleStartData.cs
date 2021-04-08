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

    public BattlePlayerStartData playerData = new BattlePlayerStartData();

    public BattleAIStartData aiData = new BattleAIStartData();

    // BattleField data

    public BattleType battleType = BattleType.Standard;



}

public class BattlePlayerStartData
{

    public List<DataUnit> playerUnits = new List<DataUnit>();
}

public class BattleAIStartData
{
   public List<DataUnit> enemieUnits = new List<DataUnit>();


}
