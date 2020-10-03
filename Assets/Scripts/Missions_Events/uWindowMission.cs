using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;


public class uWindowMission : MonoBehaviour
{

    #region Fields

    [Header("References")]

    [SerializeField] private Text _missionName;

    [SerializeField] private Image image;

    [SerializeField] private Text labelType;

    [SerializeField] private Text labelLevel;

    [SerializeField] private Text labelEnviroment;

    [SerializeField] private Text labelTime;

    [SerializeField] private Text labelDistance;

    [SerializeField] private Text textDescription;

    [SerializeField] private Text buttonActiveText;

    [SerializeField] private Text specMinMax;

    [Header("Button")]

    [SerializeField] private Button buttonStartMission;

    [SerializeField] private Button buttonCloseMission;

    [Header("Specialist Add")]

    [SerializeField] private GameObject prefab;

    [SerializeField] private GameObject specContent;

    [Header("Info")]

    [SerializeField] private LevelOfDangerous missionLevel;

   // [SerializeField] private Terrain missionEnviroment;

    [SerializeField] private List<Terrain> emergingTerrains = new List<Terrain>();

    [SerializeField] private MissionTime missionTime;

    #endregion

    private int MaxSpecOnMission = 0;
    private int MinSpecOnMission = 0;


    #region Properties

    public string MissionName
    {
        get { return _missionName.text; }
        set { _missionName.text = value; }
    }

    public Sprite Sprite
    {
        // get { return image.; }
        set { image.sprite = value; }
    }

    public string MissionType
    {
        get { return labelType.text; }
        set { labelType.text = value; }
    }

    public float MissionDistance
    {
        set { labelDistance.text = value.ToString(); }
    }

    public string DesriptionText { set { textDescription.text = value; } }

    public int GetMaxSpecOnMission { get { return this.MaxSpecOnMission; } }

    public string ButtonStartText { set { buttonActiveText.text = value; } }

    public LevelOfDangerous MissionLevel
    {
        get { return missionLevel; }
        set { missionLevel = value; }
    }

    public List<Terrain> MissionTerrainList
    {
        get { return emergingTerrains; }
        set { emergingTerrains = value; }
    }

    public MissionTime MissionTime
    {
        get { return missionTime; }
        set { missionTime = value; }
    }

    public bool SetActivityMissionPanel
    {
        set { this.gameObject.SetActive(value); }
    }

    public Button GetStartMissionButton
    {
        get { return buttonStartMission; }
    }

    public Button GetCloseMissionButton
    {
        get { return buttonCloseMission; }
    }

    public GameObject SpecContent
    {
        get { return this.specContent; }
        set { this.specContent = value; }
    }

    public GameObject PlusPrefab
    {
        get { return this.prefab; }
        set { this.prefab = value; }
    }

    public void SetSpecMinMaxText(int min , int max)
    {
        this.MaxSpecOnMission = max;
        this.MinSpecOnMission = min;

        this.specMinMax.text = $"{MinSpecOnMission} / {MaxSpecOnMission}";
    }

    #endregion

    #region Methods

    public void AddTerrain(Terrain terrain)
    {
        this.emergingTerrains.Add(terrain);
    }

    public void RefreshDangerousLevel()
    {
        switch (missionLevel)
        {
            case LevelOfDangerous.jedna:
                labelLevel.text = "jedna";
                break;
            case LevelOfDangerous.dva:
                labelLevel.text = "dva";
                break;
            case LevelOfDangerous.tri:
                labelLevel.text = "tri";
                break;
        }
    }

    public void RefreshMissionEnviroment()
    {
        // vypis je TMP -> konečný format se pořesí v průběhu..
        foreach (var item in emergingTerrains)
        {
            switch (item)
            {
                case Terrain.pole:
                    labelEnviroment.text += "pole ";
                    break;
                case Terrain.poust:
                    labelEnviroment.text += "poust ";
                    break;
                case Terrain.dzungle:
                    labelEnviroment.text += "dzungle ";
                    break;
                case Terrain.les:
                    labelEnviroment.text += "les ";
                    break;
            }
        }


    }

    public void RefreshMissionTime()
    {
        switch (missionTime)
        {
            case MissionTime.malo:
                labelTime.text = "malo";
                break;
            case MissionTime.akorat:
                labelTime.text = "akorat";
                break;
            case MissionTime.stredne:
                labelTime.text = "stredne";
                break;
            case MissionTime.hodne:
                labelTime.text = "hodne";
                break;
        }    
    }


    public void Init()
    {
        RefreshDangerousLevel();
        RefreshMissionEnviroment();
        RefreshMissionTime();
    }


    #endregion
}
