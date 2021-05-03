using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleType
{
    Testing,
    BattleBasic,
    Ambush,
    HordeAttack,
}


public class BattleStartData
{

    // Players In battle

    public BattlePlayerStartData playerData = new BattlePlayerStartData();

    public BattleAIStartData enemyData = new BattleAIStartData();

    // BattleField data

    public int Collumn;

    public int Rows;

    public BattleType battleType = BattleType.BattleBasic;

    public bool isRandomEnemyPosition = true;

    public void AddMonsterBattleData(Monster monster)
    {
        DataUnit dataUnit = new DataUnit(monster);

        enemyData.enemieUnits.Add(dataUnit);
    }

    public void AddPlayerBattleData(Character character)
    {
        DataUnit dataUnit = new DataUnit(character);

        playerData.playerUnits.Add(dataUnit);
    }
}

public class BattlePlayerStartData
{

    public List<DataUnit> playerUnits = new List<DataUnit>();
}

public class BattleAIStartData
{
   public List<DataUnit> enemieUnits = new List<DataUnit>();


}
