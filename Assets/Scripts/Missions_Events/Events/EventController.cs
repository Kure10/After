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

    List<Character> CharactersSelectedForTesting = new List<Character>();


    private bool finalTestResult = false;
    private TestCase tCase;

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
                eventPanel.CreateButon(() => SelectionButton(int.Parse(title), mission), buttonText, buttonDescription);
                return mission;
            case 2:
               
                eventPanel.CreateButon(() => SelectionButton(int.Parse(title), mission), "Won Battle.." + title,"Tady nic neni proste jsi vyhral..");
                this.eventPanel.TestingFight(item.GetStrStat("BattleType"), item.GetIntStat("BattleDiff"), item.GetIntStat("MinEnemyNumber"), item.GetIntStat("MonsterDiffMax"));
                return mission;
            case 3:
                this.eventPanel.TestingMonster(item.GetStrStat("Monster"), item.GetIntStat("BeastNumber"));
                return mission;
            case 4:

                tCase = null; // ToDo nevim co to udela s new tak radci tu davam null musim se zeptat..

                eventPanel.SetState = EventPanel.PanelStates.Test;

                foreach (Character character in mission.GetCharactersOnMission)
                {
                    GameObject go = Instantiate(eventPanel.GetCharacterButtonPrefab, eventPanel.GetCharacterTransformContent.transform);
                    uWindowSpecialist uWindow = go.GetComponent<uWindowSpecialist>();
                    uWindow.SetAll(character);
                    eventPanel.AddCharacterToSelectionContent(go);
                    uWindow.GetMainButton.onClick.RemoveAllListeners();
                    uWindow.GetMainButton.onClick.AddListener(delegate () { SelectCharacterForTest(character, uWindow); });
                }

                 tCase = new TestCase(item.GetStrStat("TestTarget"), item.GetStrStat("TestName"), item.GetStrStat("KindTest"), item.GetStrStat("TestType"),
                    item.GetStrStat("TestAtribute"), item.GetIntStat("TestDiff"), item.GetIntStat("TestRateMod"), item.GetIntStat("KarmaInfluence"),
                    item.GetStrStat("KarmaDescription"), item.GetIntStat("SpecTestNumMin"), item.GetIntStat("SpecTestNumMax"));

                eventPanel.SetupTestingState(tCase);

                eventPanel.GetContinueButton.onClick.RemoveAllListeners();
                eventPanel.GetContinueButton.onClick.AddListener( ()=> ContinueButton());


                // cerna magie vratit se k tomuto
                eventManager.resolveMaster.ResolveCondition += OnAction;


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
                    eventPanel.SetState = EventPanel.PanelStates.Selection;

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

    private void SelectionButton(int numberOfBrachToResolve, Mission mission)
    {
        eventPanel.DestroyAllButtons();

        slave.StartResolve(numberOfBrachToResolve);
        var output = slave.Resolve();

        LoadEventSteps(output, mission);
    }

    private void ContinueButton()
    {
        int selectedCharacters = CharactersSelectedForTesting.Count;

        if (selectedCharacters >= tCase.GetMinCharParticipation && selectedCharacters <= tCase.GetMaxCharParticipation)
        {
            finalTestResult = StartTest();
        }
        else
        {
            // ToDo
            // vyber spravny pocet charakteru
        }

        Debug.Log("result of Test " +  finalTestResult);
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
        if (CharactersSelectedForTesting.Contains(character))
        {
            CharactersSelectedForTesting.Remove(character);
            uWindow.DeactivateCoverPanel();
        }
        else
        {
            CharactersSelectedForTesting.Add(character);
            uWindow.ActivateCoverPanel("Specialista je vybran");
        }
    }

    #endregion

    public class TestCase
    {
        #region Fields
        private string testName; 

        /*  Testing atributes */
        private bool isTestingLevel;
        private bool isTestingSocial;
        private bool isTestingTechnical;
        private bool isTestingScience;
        private bool isTestingMilitary;

        private int testDiff;
        private int testRateMod;

        private bool karmaInfluence; // ToDO Karma is not finish yet

        //public string karmaDescription;

        private int minimalCharacterParticipation;
        private int maximalCharacterParticipation;
        TestType testType;
        ClassTest classTest;
        TestingSubjects subject;

        #endregion
        // karmaDescription , Todo
        #region Constructor 

        public TestCase(string _testTarget, string _testName, string _kindTest, string _testType,
            string _testAtribute, int _testDiff, int _testRate, int _karmaInfluence, string _karmaDescription, int _min, int _max)
        {
            testType = (TestType)Enum.Parse(typeof(TestType), _testType, true);
            minimalCharacterParticipation = _min;
            maximalCharacterParticipation = _max;
            karmaInfluence = Convert.ToBoolean(_karmaInfluence);
            testRateMod = _testRate;
            testDiff = _testDiff;
            classTest = (ClassTest)Enum.Parse(typeof(ClassTest), _kindTest, true);
            subject = (TestingSubjects)Enum.Parse(typeof(TestingSubjects), _testTarget, true);
            ChooseTestingSkills(_testAtribute);
            testName = _testName;
        }

        #endregion
        #region Properities
        public int GetMaxCharParticipation { get { return this.maximalCharacterParticipation; } }
        public int GetMinCharParticipation { get { return this.minimalCharacterParticipation; } }
        public int GetRateMod { get { return this.testRateMod; } }
        public int GetDifficulty { get { return this.testDiff; } }
        public bool GetKarmaInfluence { get { return this.karmaInfluence; } }
        public TestType GetType { get { return this.testType; } }
        public ClassTest GetClass { get { return this.classTest; } }
        public TestingSubjects GetTestingSubjects { get { return this.subject; } }
        public string GetName { get { return this.testName; } }
        public bool IsTestingLevel { get { return this.isTestingLevel; } }
        public bool IsTestingMilitary { get { return this.isTestingMilitary; } }
        public bool IsTestingSocial { get { return this.isTestingSocial; } }
        public bool IsTestingScience { get { return this.isTestingScience; } }
        public bool IsTestingTechnical { get { return this.isTestingTechnical; } }

        public void ChooseTestingSkills (string _testAtribute)
        {
            isTestingLevel =  _testAtribute.Contains("LvL");
            isTestingMilitary = _testAtribute.Contains("MiL");
            isTestingScience = _testAtribute.Contains("ScL");
            isTestingSocial = _testAtribute.Contains("SoL");
            isTestingTechnical = _testAtribute.Contains("TeL");
        }
        #endregion
    }

    #region Enums
    public enum TestType
    {
        Battle,
        Comunication,
        DiggBuild,
        Gathering,
        Hunting,
        Leverage,
        LockPicking,
        Repair,
        Research,
        Scavenging,
        Scouting,
        SelectOnly,
        Sneaking,
    }

    public enum ClassTest
    {
        Together,
        Separately
    }

    public enum TestingSubjects
    {
        RangeNum,
        SpecActive,
        RangeNumRng,
        DirectNum,
        DirectNumRng,
        AllGroup
    }

    #endregion 



    public bool StartTest()
    {
        bool finalResult;

        if (tCase.GetClass == ClassTest.Separately)
        {
            foreach (Character character in CharactersSelectedForTesting)
            {
                int dices = CalculateAmountDices(character);
               character.PassedTheTest = CalculateSuccess(dices);
            }
        }
        else if (tCase.GetClass == ClassTest.Together)
        {
            int dices = 0;
            bool colectiveResult;

            foreach (Character character in CharactersSelectedForTesting)
                dices += CalculateAmountDices(character);

            colectiveResult = CalculateSuccess(dices);

            foreach (Character character in CharactersSelectedForTesting)
                character.PassedTheTest = colectiveResult;
        }

        // ToDo - podminka  Nastaveni priority je dulezitesi prohra || vyhra

        if (true) // pokud alespon jeden prosel, prosli vsichni
        {
            finalResult = false;

            foreach (Character character in CharactersSelectedForTesting)
            {
                if (character.PassedTheTest == true)
                    finalResult = true;
            }
        }
        else // pokud alespon jeden selhal , selhali vsichni..
        {
            finalResult = true;

            foreach (Character character in CharactersSelectedForTesting)
            {
                if (character.PassedTheTest == false)
                    finalResult = false;
            }
        }

        return finalResult;
    }

    private int CalculateAmountDices(Character character)
    {
        int amountDices = 0;

        if (tCase.IsTestingLevel)
            amountDices += character.Stats.level;
        if (tCase.IsTestingMilitary)
            amountDices += character.Stats.military;
        if (tCase.IsTestingTechnical)
            amountDices += character.Stats.tech;
        if (tCase.IsTestingSocial)
            amountDices += character.Stats.social;
        if (tCase.IsTestingScience)
            amountDices += character.Stats.science;

        return amountDices;
    }

    private bool CalculateSuccess(int amountDices)
    {
        bool result;
        int numberOfIndividualSuccesses = 0;
        int resultOfOneThrow = 0;

        for (int j = 0; j < amountDices; j++)
        {
            resultOfOneThrow += UnityEngine.Random.Range(1, 7);

            if (resultOfOneThrow % 6 == 0)
            {
                j--;
                continue;
            }

            if (resultOfOneThrow >= tCase.GetDifficulty)
                numberOfIndividualSuccesses++;

            resultOfOneThrow = 0;
        }

        int totalNumberOfSuccesses = numberOfIndividualSuccesses / tCase.GetRateMod;

        if (totalNumberOfSuccesses > 0)
            return true;
        else
            return false;

    }

    public bool OnAction(string dataNameFile, StatsClass element)
    {
        return finalTestResult;
    }
}




