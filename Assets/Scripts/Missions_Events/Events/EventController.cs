using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResolveMachine;
using System;

public class EventController : MonoBehaviour
{
    static public bool isEventRunning = false;

    [SerializeField] public EventManager eventManager;

    [SerializeField] EventPanel eventPanel;

    private ResolveSlave slave;

    private ResourceSpriteLoader spriteLoader;

    List<Character> TestingCharacters = new List<Character>();

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
        slave = eventManager.resolveMaster.AddDataSlave("Events", _event.Title);
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
                var buttonDescription = item.GetStrStat("Option");
                eventPanel.CreateButon(() => PressEventButton(int.Parse(title), mission), buttonText, buttonDescription);
                return mission;
            case 2:
                this.eventPanel.IsBattleOnline = true;
                eventPanel.CreateButon(() => PressEventButton(int.Parse(title), mission), "Won Battle.." + title,"Tady nic neni proste jsi vyhral..");
                this.eventPanel.TestingFight(item.GetStrStat("BattleType"), item.GetIntStat("BattleDiff"), item.GetIntStat("MinEnemyNumber"), item.GetIntStat("MonsterDiffMax"));
                return mission;
            case 3:
                this.eventPanel.TestingMonster(item.GetStrStat("Monster"), item.GetIntStat("BeastNumber"));
                return mission;
            case 4:



                this.eventPanel.testingTest.SetActive(false);
                this.eventPanel.IsBattleOnline = false;

                eventPanel.CreateButon(() => PressEventButton(int.Parse(title), mission), "Start Test.." + title, "Zahajit test.. ..");

                eventPanel.EnableCharacterContent();

                foreach (Character character in mission.GetCharactersOnMission)
                {
                    GameObject go = Instantiate(eventPanel.GetCharacterButtonPrefab, eventPanel.GetCharacterTransformContent.transform);
                    uWindowSpecialist uWindow = go.GetComponent<uWindowSpecialist>();
                    uWindow.SetAll(character);
                    eventPanel.AddCharacterToSelectionContent(go);
                    uWindow.GetMainButton.onClick.RemoveAllListeners();
                    uWindow.GetMainButton.onClick.AddListener(delegate () { SelectCharacterForTest(character, uWindow); });
                }


                Test test = new Test(item.GetStrStat("TestTarget"), item.GetStrStat("TestName"), item.GetStrStat("KindTest"), item.GetStrStat("TestType"),
                    item.GetStrStat("TestAtribute"), item.GetIntStat("TestDiff"), item.GetIntStat("TestRateMod"), item.GetIntStat("KarmaInfluence"),
                    item.GetStrStat("KarmaDescription"), item.GetIntStat("SpecTestNumMin"), item.GetIntStat("SpecTestNumMax"));
                

                item.GetStrStat("TestTarget");

                item.GetStrStat("TestName");

                item.GetStrStat("KindTest");

                item.GetStrStat("TestType");

                item.GetStrStat("TestAtribute");





                item.GetIntStat("TestDiff");

                item.GetIntStat("TestRateMod");

                item.GetIntStat("KarmaInfluence");

                item.GetStrStat("KarmaDescription");

                this.eventPanel.TestingTest();

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
                    // todo when type is "vyber rozsahem" bude chyba poresit..

                    List<Character> afectedCharacters = SelectTarget(item.GetStrStat("SecAtrChangeTarg"), mission);

                    ModifyAtribute(item.GetStrStat("SecAtrChangeType"), afectedCharacters, item.GetStrStat("SecAtr"), item.GetIntStat("SecAtrChange"));

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

    #region HelpingMethods

    private void ModifyAtribute (string type , List<Character> targets , string atribute , int value)
    {
        // Todo

        if(type == "ChangeTypeDirect")
        {
            foreach (Character character in targets)
            {
                character.ModifiCharacterAtribute(atribute,value);
            }
        }
        else
        {

        }

    }

    private List<Character> SelectTarget(string target, Mission mission)
    {
        //Todo

        if (target == "AllActiveSpec")
        {
            return mission.GetCharactersOnMission;
        }
        else if(target == "AllSpec")
        {
            return mission.GetCharactersOnMission;
        }
        else if (target == "sad")
        {
            return mission.GetCharactersOnMission;
        }
        else if (target == "neewewco")
        {
            return mission.GetCharactersOnMission;
        }
        else if (target == "rrrr")
        {
            return mission.GetCharactersOnMission;
        }


        Debug.LogError("Error selectTarget Method in script: " + this.name);
        return null;
    }

    private void SelectCharacterForTest(Character character, uWindowSpecialist uWindow)
    {
        if (TestingCharacters.Contains(character))
        {
            TestingCharacters.Remove(character);
            uWindow.DeactivateCoverPanel();
        }
        else
        {
            TestingCharacters.Add(character);
            uWindow.ActivateCoverPanel("Specialista je vybran");
        }
    }

    #endregion

    public class Test
    {
        // zeptat se nefa jak se vyhodnocuje uspech atd.. 
        // jak ma vypadat gui


        public string testName; // Co to jako je ?? pro hrace ? jestli jo do CZ musi mit preklad.. // musi byt v EventCz


        public string testedAtribute;  // muže byt i vic..

        // -------------

        private int testDiff;

        private int testRateMod;

        private bool karmaInfluence;

        //public string karmaDescription;

        private int minimalCharacterParticipation;

        private int maximalCharacterParticipation;

        TestType testType; // Potrebuji vycet vsech moznosti anglicky co me chodi  v XML

        ClassTest classTest; // to same..

        TestingSubjects subject; // to same..

        public Test(string _testTarget , string _testName , string _kindTest, string _testType ,
            string _testAtribute, int _testDiff, int _testRate, int _karmaInfluence, string _karmaDescription, int _min , int _max)
        {
            testType = (TestType) Enum.Parse(typeof(TestType), _testType, true);
            minimalCharacterParticipation = _min;
            maximalCharacterParticipation = _max;
            karmaInfluence = Convert.ToBoolean(_karmaInfluence);
            testRateMod = _testRate;
            testDiff = _testDiff;
            classTest = (ClassTest)Enum.Parse(typeof(ClassTest), _kindTest, true);
            subject = (TestingSubjects)Enum.Parse(typeof(TestingSubjects), _testTarget, true);
        }

        public int GetMaxCharParticipation { get { return this.maximalCharacterParticipation; } }
        public int GetMinCharParticipation { get { return this.minimalCharacterParticipation; } }
        public int GetRateMod { get { return this.testRateMod; } }
        public int GetDifficulty { get { return this.testDiff; } }
        public bool GetKarmaInfluence { get { return this.karmaInfluence; } }
        public TestType GetType { get { return this.testType; } }
        public ClassTest GetClass { get { return this.classTest; } }
        public TestingSubjects GetTestingSubjects { get { return this.subject; } }

    }


    public enum TestType
    {
        Angry,
        Sneaking,
        Sad
    }

    public enum ClassTest
    {
        Angry,
        Separately
    }

    public enum TestingSubjects
    {
        Angry,
        Separately
    }

}




