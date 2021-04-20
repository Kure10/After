using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLogPanel : MonoBehaviour
{
    public List<BattleLog> battleLogs = new List<BattleLog>();

    private int index = 0;
    public void AddLog(string logMessage)
    {
        battleLogs[index].transform.SetAsFirstSibling();
        battleLogs[index].SetLog(logMessage);

        index++;

        if(index >= battleLogs.Count)
            index = 0;
    }

}
