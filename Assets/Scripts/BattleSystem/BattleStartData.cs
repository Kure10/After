using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    public (int statClassNumber, Mission mission) WinEvaluation = (0, null);

    private List<Character> charactersInBattleFromMission = new List<Character>();


    public List<Character> GetCharacterFromBattle { get { return charactersInBattleFromMission; } }
    public void AddCharacterFromMission(List<Character> charactersFromMission)
    {
        charactersInBattleFromMission.AddRange(charactersFromMission);
    }

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

    // update inventory a taky zivoty.
    // 
    public void UpdateMainPlayerData(List<Unit> battlePlayerUnit)
    {
        foreach (Unit unit in battlePlayerUnit)
        {
            if(unit._team == Unit.Team.Human)
            {
                foreach (Character character in charactersInBattleFromMission)
                {
                    if (character.GetBlueprint().Id == unit.Id)
                    {
                        character.ModifyLife(unit.CurrentHealth);
                    }
                }
            }
        }
    }

    public void RestartDataForNewCombat()
    {
        Collumn = 0;
        Rows = 0;

        battleType = BattleType.BattleBasic;
        isRandomEnemyPosition = true;

        playerData.ClearData();
        enemyData.ClearData();

        charactersInBattleFromMission.Clear();

        WinEvaluation = (0, null);
    }

}

public class BattlePlayerStartData
{
    public List<DataUnit> playerUnits = new List<DataUnit>();

    public void ClearData()
    {
        playerUnits.Clear();
    }
}

public class BattleAIStartData
{
   public List<DataUnit> enemieUnits = new List<DataUnit>();

    public void ClearData()
    {
        enemieUnits.Clear();
    }
}
