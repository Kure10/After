using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionInfoController : MonoBehaviour  
{

    [SerializeField]
    GameObject prefabToSpawn;

    [SerializeField]
    GameObject ParentHolder;

    private GameObject myRow;

    private List<uMissionInfoRow> rows = new List<uMissionInfoRow>();

    private void SpawnInfoRow ()
    {
        if(ParentHolder == null)
        {
            myRow = Instantiate(prefabToSpawn,this.transform.position, Quaternion.identity);
            myRow.transform.SetParent(this.gameObject.transform);
        }
        else
        {
            myRow = Instantiate(prefabToSpawn, this.transform.position, Quaternion.identity);
            myRow.transform.SetParent(ParentHolder.transform);
        }
    }

    public void InfoRowCreate (Mission mis)
    {
        SpawnInfoRow();
        uMissionInfoRow tmp = myRow.GetComponent<uMissionInfoRow>();

        tmp.misID = mis.Id;
        tmp.TextString.text = UpdateDescription(mis);
        rows.Add(tmp);
    }

    public void UpdateInfoRows (List<Mission> missionInProgress)
    {
        for (int i = 0; i <= missionInProgress.Count -1; i++)
        {
            for (int j = 0; j <= rows.Count -1; j++)
            {
                if(rows[j].misID == missionInProgress[i].Id)
                {
                    rows[j].TextString.text = UpdateDescription(missionInProgress[i]);
                }
            }
        }
    }

    private string UpdateDescription(Mission miss, bool missionComplete = false)
    {
        string description = null;

        if (missionComplete)
        {
            description = miss.Name + " Mission Complete! ";
            return description;
        }

        description = miss.Name +  " Time Remains: " + miss.Distance.ToString("N0");
        return description;
    }

    public void DeleteFromInfoRow(Mission mission)
    {
        uMissionInfoRow rowToDelete = GetInfoRow(mission);
        rowToDelete.TextString.text = UpdateDescription(mission, true);

        rows.Remove(rowToDelete);
        Destroy(rowToDelete.gameObject, 2f);
    }

    private uMissionInfoRow GetInfoRow (Mission miss)
    {
        for (int i = 0; i <= rows.Count -1; i++)
        {
            if (miss.Id == rows[i].misID)
                return rows[i];
        }

        return null;
    }


}
