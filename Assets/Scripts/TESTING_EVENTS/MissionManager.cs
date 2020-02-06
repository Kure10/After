using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    [SerializeField]
    GameObject missionPanel;

    public List<Mission> missions = new List<Mission>();

    public GameObject EnableMissionPanel{ set { missionPanel.SetActive(value); } }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    



}
