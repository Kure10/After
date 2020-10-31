using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResolveMachine;

public class EventController : MonoBehaviour
{
    static public bool isEventRunning = false;

    [SerializeField] public EventManager eventManager;
    [SerializeField] public EventCreater eventCreater;

    [SerializeField] EventPanel eventPanel;

    private ResolveSlave slave;

    private ResourceSpriteLoader spriteLoader;

    private void Awake()
    {
        spriteLoader = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceSpriteLoader>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }

    public void EventTrigered(Mission mission)
    {
        // choise Random Event..
        StatsClass _event = eventManager.ChoiseRandomEvent(mission.DifficultyMin, mission.DifficultyMax, mission.GetEmergingTerrains);

        // nastavit obrazek..
        var text = _event.GetStrStat("EventPicture");
        Sprite sprite = spriteLoader.LoadEventSprite(text);

        eventPanel.SetupEventInfo(_event.GetStrStat("Name"), _event.GetStrStat("Description"), sprite);


        // Work with data..
        slave = eventCreater.resolveMaster.AddDataSlave("Events", _event.Title);
        slave.StartResolve();
        Dictionary<string, List<StatsClass>> output = slave.Resolve();

        this.eventPanel.gameObject.SetActive(true);

        LoadEventSteps(output, mission);

    }



    private void LoadEventSteps(Dictionary<string, List<StatsClass>> output, Mission mission)
    {
        foreach (StatsClass item in output["Result"])
        {
            SetNextStepEvent(item,mission);
        }
    }

    private Mission SetNextStepEvent(StatsClass item, Mission mission)
    {
        var title = item.Title;
        var number = item.GetIntStat("$T");

        switch (number)
        {
            case 1:
                var buttonText = item.GetStrStat("OptionType");
                eventPanel.CreateButon(() => PressEventButton(int.Parse(title), mission), buttonText);
                return mission;
            case 2:
                eventPanel.CreateButon(() => PressEventButton(int.Parse(title), mission), "Won Battle.." + title);
                this.eventPanel.TestingFight(item.GetStrStat("BattleType"), item.GetIntStat("BattleDiff"), item.GetIntStat("MinEnemyNumber"), item.GetIntStat("MonsterDiffMax"));
                return mission;
            case 3:
                this.eventPanel.TestingMonster(item.GetStrStat("Monster"), item.GetIntStat("BeastNumber"));
                return mission;
            case 4:

                return mission;
            case 5:

                if (item.GetBoolStat("AddTimeChange"))
                {
                    string isDelayed = item.GetStrStat("TimeChange");
                    int timeDelay = item.GetIntStat("TimeDelay");

                    if(isDelayed == "Delay")
                        mission.Distance += timeDelay;
                    else
                        mission.Distance -= timeDelay;

                }
                else if (item.GetBoolStat("AddEventEnd"))
                {
                    this.eventPanel.gameObject.SetActive(false);
                    TimeControl.IsTimeBlocked = false;
                }
                else if (item.GetBoolStat("AddTextWindow"))
                {
                    this.eventPanel.TitleField.text = item.GetStrStat("TitleTextWindow");
                    this.eventPanel.DescriptionTextField.text = item.GetStrStat("TextWindow");
                }
                else if (item.GetBoolStat("AddPerk"))
                {

                }
                else if (item.GetBoolStat("AddSecAtr"))
                {

                }
                else if (item.GetBoolStat("ChangeSource"))
                {

                }
                else if (item.GetBoolStat("RngChange"))
                {

                }
                else if (item.GetBoolStat("SpecDefSelect"))
                {

                }
                else if (item.GetBoolStat("AddContainer"))
                {

                }

                return mission;
            case 6:

                return mission;
            default:
                Debug.LogWarning("Warning event was created with error: " + number + " : " + title);
                return mission;


        }
    }

    private void PressEventButton(int i, Mission mission)
    {
        eventPanel.DestroyAllButtons();

        slave.StartResolve(i);
        var output = slave.Resolve();

        LoadEventSteps(output, mission);
    }


   }

