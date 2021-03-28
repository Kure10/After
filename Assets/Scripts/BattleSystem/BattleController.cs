using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    public List<GameObject> rows = new List<GameObject>();

    public List<Squar> squars = new List<Squar>();

    [Space]
    public int collumCount = 16;

    public GameObject squarTemplate;


    private void Start()
    {
        CreateBattleField();
    }


    public void CreateBattleField()
    {
        int j = 0;
        foreach (GameObject row in rows)
        {
            for (int i = 0; i < collumCount; i++)
            {
                GameObject squarGameObject = Instantiate(squarTemplate, row.transform);
                Squar squar = squarGameObject.GetComponent<Squar>();
                squar.SetCoordinates(i,j);

                squars.Add(squar);
            }
            j++;
        }

    }

    public void InitBattle(BattleStartData data)
    {
        
    }

}
