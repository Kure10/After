using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bestionary : MonoBehaviour
{

    [Header("Monsters in game")]
    [SerializeField]
    private List<Monster> allMonsters = new List<Monster>();

    // Start is called before the first frame update
    void Start()
    {
        MonsterXmlLoader xmlLoader = new MonsterXmlLoader();
        this.allMonsters = xmlLoader.GetMonstersFromXML();
    }

    public Monster GetMonsterByID (long id)
    {
        var result = allMonsters.Find(x => x.ID == id);

        return result;
    }

}
