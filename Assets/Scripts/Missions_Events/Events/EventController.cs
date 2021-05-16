using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResolveMachine;
using System;
using System.Text;

public class EventController : MonoBehaviour
{
    static public bool isEventRunning = false;

    public static event Action<Mission> OnEventEnd = delegate { };

    [SerializeField] public EventManager eventManager;

    [SerializeField] EventPanel eventPanel;

    [SerializeField] ResourceSpriteLoader spriteLoader;

    [SerializeField] Bestionary bestionary;

    [SerializeField] BattleController battleController;

    [SerializeField] GameObject eventBlocker;

    private ResolveSlave slave;

    List<Character> CharactersSelectedForTesting = new List<Character>();

    private bool finalTestResult = false;
    private TestCase tCase;

    private BattleStartData _battleStartData = new BattleStartData();

    public GameObject GetEventPanel { get { return this.eventPanel.gameObject; } }

    public void Awake()
    {
        eventPanel.GetMinimizeButton.onClick.RemoveAllListeners();
        eventPanel.GetMinimizeButton.onClick.AddListener(Minimaze);
    }

    #region Public Methods

    public void EventTrigered(Mission mission)
    {
        // choise Random Event..
        StatsClass _event = eventManager.ChoiseRandomEvent(mission.DifficultyMin, mission.DifficultyMax, mission.GetEmergingTerrains);

        // PreWarm Pictureteamevent
        Sprite sprite = spriteLoader.LoadEventSprite(_event.GetStrStat("EventPicture"));
        eventPanel.SetImage(sprite);

        // Work with data..
        slave = eventManager.resolveMaster.AddDataSlave("Events", _event.Title);
        slave.StartResolve();
        Dictionary<string, List<StatsClass>> output = slave.Resolve();

        AddCharactersPrefabFromMissionToEvent(mission);
        eventPanel.AmountCharacterSelectedText.text = "";

        eventBlocker.SetActive(true);
        this.eventPanel.gameObject.SetActive(true);

        LoadEventSteps(output, mission);

    }

    #endregion

    #region Private Methods

    private void LoadEventSteps(Dictionary<string, List<StatsClass>> output, Mission mission)
    {
        // windowSpec nastavit na default..
        foreach (KeyValuePair<GameObject, Character> characterInEvent in eventPanel.GetCharactersOnEvent)
        {
            uWindowSpecialist uWindow = characterInEvent.Key.GetComponent<uWindowSpecialist>();
            Character character = characterInEvent.Value;

            uWindow.TurnOffResult();
        }

        // Jedu pres vsechny slavy
        foreach (StatsClass item in output["Result"])
        {
            SetNextStepEvent(item, mission);
        }
    }


    private Mission SetNextStepEvent(StatsClass statClass, Mission mission)
    {
        var title = statClass.Title;
        var number = statClass.GetIntStat("$T");

        switch (number)
        {
            case 1:
                ProcessOptions(mission, statClass, title);
                return mission;
            case 2:
                ProcessFight(mission, statClass, title);
                return mission;
            case 3:
                ProcessMonster(mission, statClass, title);
                return mission;
            case 4:
                ProcessTest(mission, statClass, title);
                return mission;
            case 5:
                ProcessChange(mission, statClass, title);
                return mission;
            case 6:
                ProcessEvaluation(mission, statClass, title);
                return mission;
            default:
                Debug.LogWarning("Warning event was created with error: " + number + " : " + title);
                return mission;
        }
    }

    private void ProcessOptions(Mission mission,StatsClass item, string title)
    {
        var buttonText = item.GetStrStat("OptionType");
        var buttonDescription = item.GetStrStat("Option");
        eventPanel.CreateButon(() => SelectionButton(int.Parse(title), mission), buttonText, buttonDescription);
    }

    private void ProcessFight(Mission mission, StatsClass statClass, string title)
    {
        // eventPanel.CreateButon( () => SelectionButton(int.Parse(title), mission), "Won Battle.." + title, "Tady nic neni proste jsi vyhral..");

        foreach (Character character in mission.GetCharactersOnMission)
        {
            _battleStartData.AddPlayerBattleData(character);
        }

        _battleStartData.AddCharacterFromMission(mission.GetCharactersOnMission);

        BattleType battleType = BattleType.Testing;
        string type = statClass.GetStrStat("BattleType");
        bool checkParse = Enum.TryParse(type, out battleType);
        _battleStartData.battleType = battleType;

        battleController.StartBattle(_battleStartData);
    }

    private void ProcessMonster(Mission mission, StatsClass statClass, string title)
    {
        string stringID = statClass.GetStrStat("Monster");
        int monsterCount = statClass.GetIntStat("BeastNumber");

        long.TryParse(stringID, out long idNumber);
        long monsterID = idNumber;

        Monster monster = bestionary.GetMonsterByID(monsterID);

        for (int i = 0; i < monsterCount; i++)
        {
            _battleStartData.AddMonsterBattleData(monster);
        }
    }

    private void ProcessTest(Mission mission, StatsClass item, string title)
    {

        tCase = null; // ToDo nevim co to udela s new tak radci tu davam null musim se zeptat..

        eventPanel.SetState = EventPanel.PanelStates.Test;

        foreach (KeyValuePair<GameObject, Character> characterInEvent in eventPanel.GetCharactersOnEvent)
        {
            uWindowSpecialist uWindow = characterInEvent.Key.GetComponent<uWindowSpecialist>();
            Character character = characterInEvent.Value;
            uWindow.GetMainButton.onClick.RemoveAllListeners();
            uWindow.GetMainButton.onClick.AddListener(delegate () { SelectCharacterForTest(character, uWindow); });
        }

        tCase = new TestCase(item.GetStrStat("TestTarget"), item.GetStrStat("TestName"), item.GetStrStat("KindTest"), item.GetStrStat("TestType"),
            item.GetStrStat("TestAtribute"), item.GetIntStat("TestDiff"), item.GetIntStat("TestRateMod"), item.GetIntStat("KarmaInfluence"),
            item.GetStrStat("KarmaDescription"), item.GetIntStat("SpecTestNumMin"), item.GetIntStat("SpecTestNumMax"), item.GetStrStat("ResultPriority"));

        eventPanel.SetupTestingState(tCase);
        eventPanel.AmountCharacterSelectedText.text = CharactersSelectedForTesting.Count + " / " + tCase.GetMaxCharParticipation;

        eventPanel.GetContinueButton.onClick.RemoveAllListeners();
        eventPanel.GetContinueButton.onClick.AddListener(() => ContinueButton());

        eventPanel.buttonForTMPProceed.onClick.RemoveAllListeners();
        eventPanel.buttonForTMPProceed.onClick.AddListener(() => TMP_ProceedButton(int.Parse(title), mission));

        // cerna magie vratit se k tomuto
        eventManager.resolveMaster.ResolveCondition += OncheckTest;
    }

    private void ProcessChange(Mission mission, StatsClass item, string title)
    {
        if (item.GetBoolStat("AddTimeChange"))
        {
            string isDelayed = item.GetStrStat("TimeChange");
            int timeDelay = item.GetIntStat("TimeDelay");

            if (isDelayed == "Delay")
                mission.Distance += timeDelay;
            else
                mission.Distance -= timeDelay;
        }
        else if (item.GetBoolStat("AddTextWindow"))
        {
            eventPanel.SetState = EventPanel.PanelStates.Selection;

            this.eventPanel.SelectionInfoText.text = $"Tvoji Speciaiste na misi";
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
        else if (item.GetBoolStat("AddEventEnd"))
        {
            finalTestResult = false;
            CharactersSelectedForTesting.Clear();
            RemoveCharactersGameObjectFromEvent();
            this.eventPanel.gameObject.SetActive(false);
            TimeControl.IsTimeBlocked = false;
            OnEventEnd.Invoke(mission);
            eventBlocker.SetActive(false);
            isEventRunning = false;
        }
    }

    private void ProcessEvaluation(Mission mission, StatsClass item, string title)
    {
        foreach (KeyValuePair<GameObject, Character> characterInEvent in eventPanel.GetCharactersOnEvent)
        {
            uWindowSpecialist uWindow = characterInEvent.Key.GetComponent<uWindowSpecialist>();
            Character character = characterInEvent.Value;
            CharactersSelectedForTesting.Remove(character);
            uWindow.GetMainButton.onClick.RemoveAllListeners();
            uWindow.DeactivateCoverPanel();

            if (character.PassedTheTest)
                uWindow.IsResultSuccess = true;
            else
                uWindow.IsResultSuccess = false;
        }
    }

    private void AddCharactersPrefabFromMissionToEvent(Mission mission)
    {
        eventPanel.GetCharactersOnEvent.Clear();
        eventPanel.DisableCharacterContent();

        for (int i = 0; i < mission.GetCharactersOnMission.Count; i++)
        {
            Character character = mission.GetCharactersOnMission[i];
            GameObject go = Instantiate(eventPanel.GetCharacterButtonPrefab);

            uWindowSpecialist uWindow = go.GetComponent<uWindowSpecialist>();
            uWindow.SetAll(character);
            eventPanel.AddCharacterToSelectionContent(go, character);
            uWindow.PopulateItemSlots(character);
            uWindow.AddActionsOnItemClicked(delegate { eventPanel.ReCalculatePositions();});
            uWindow.GetMainButton.onClick.RemoveAllListeners();

        }

        eventPanel.ActivateCharacterContent();
    }

    private void RemoveCharactersGameObjectFromEvent()
    {
        foreach (KeyValuePair<GameObject, Character> dic  in eventPanel.GetCharactersOnEvent)
        {
            Destroy(dic.Key);
        }

        eventPanel.GetCharactersOnEvent.Clear();
    }

    #endregion

    #region Helping Methods

    private void ModifyAtribute (string type , List<Character> targets , string atribute , int value)
    {
        // Todo  musim taky rozeznavat jestli charakter uspel nebo neuspel..
       
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
        //Todo Podle targetu vybere určity počet specialistu......

        if (target == "AllActiveSpec")
        {
            return mission.GetCharactersOnMission;
        }
        else if (target == "AllSpec")
        {
            return mission.GetCharactersOnMission;
        }
        else if (target == "OneRanActiveSpec")
        {
            return mission.GetCharactersOnMission;
        }
        else if (target == "RandomActiveSpec")
        {
            return mission.GetCharactersOnMission;
        }
        else if (target == "RangeID")
        {
            return mission.GetCharactersOnMission;
        }

        Debug.LogError("Error SelectTarget Atribute Failed. In script: " + this.name);
        return null;
    }

    #endregion

    #region Buttons Methods

    private void TMP_ProceedButton(int numberOfBrachToResolve, Mission mission)
    {
        slave.StartResolve(numberOfBrachToResolve);
        var output = slave.Resolve();

        LoadEventSteps(output, mission);
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
            this.finalTestResult = StartThrowTest();
        }
        else
        {
            // ToDo
            // vyber spravny pocet charakteru
            // nejake hlaska
            eventPanel.testingTMPinfo.text = "Vyber spravny pocet specialistu.... !!!!";
            return;
        }

        // ToDo jenom vypis pro testovani
        StringBuilder sb = new StringBuilder();
        sb.Clear();

        foreach (Character item in CharactersSelectedForTesting)
        {
            int failure = item.AmountDicesInLastTest - item.AmountSuccessDicesInLastTest;
            sb.Append(item.GetName() + " " + "měl kostek: " + item.AmountDicesInLastTest + " " +
                "Z toho uspel: " + item.AmountSuccessDicesInLastTest + " neuspel: " + failure + "." +
                "---->  uspechy s modifikatore. " + item.AmountSuccessDicesInLastTest / tCase.GetRateMod);
            sb.AppendLine();
        }

        sb.Append("uplny vysledek: " + this.finalTestResult.ToString());

        eventPanel.testingTMPinfo.text = sb.ToString();

        Debug.Log("result of Test " + this.finalTestResult);
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

        eventPanel.AmountCharacterSelectedText.text = CharactersSelectedForTesting.Count + " / " + tCase.GetMaxCharParticipation;
    }

    public void Minimaze()
    {
        eventBlocker.SetActive(false);
        eventPanel.gameObject.SetActive(false);
    }

    public void Maximaze()
    {
        this.eventBlocker.SetActive(true);
        this.eventPanel.gameObject.SetActive(true);
    }

    #endregion

    #region Helping Classes

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
        private string priority;

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
            string _testAtribute, int _testDiff, int _testRate, int _karmaInfluence, string _karmaDescription,
            int _min, int _max, string _resultPriority)
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
            priority = _resultPriority;
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
        public string GetPriority { get { return this.priority; } }

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

    #endregion
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

    private bool StartThrowTest()
    {
        bool finalResult;
        bool colectiveResult = false;
        int successDices = 0;

        if (tCase.GetClass == ClassTest.Separately)
        {
            foreach (Character character in CharactersSelectedForTesting)
            {
                
                int dices = CalculateAmountDices(character);
                character.PassedTheTest = CalculateSuccess(dices, out successDices);
                character.AmountDicesInLastTest = dices;
                character.AmountSuccessDicesInLastTest = successDices;
            }
        }
        else if (tCase.GetClass == ClassTest.Together)
        {
            int dices = 0;

            foreach (Character character in CharactersSelectedForTesting)
                dices += CalculateAmountDices(character);


            colectiveResult = CalculateSuccess(dices, out successDices);

            // vsem se nastaví stejny vysledek
            foreach (Character character in CharactersSelectedForTesting)
            {
                character.PassedTheTest = colectiveResult;
                character.AmountDicesInLastTest = dices;
                character.AmountSuccessDicesInLastTest = successDices;
            }
                
        }

        if (tCase.GetClass == ClassTest.Separately)
        {
            bool everyBodySuccess = true;
            bool everyBodyFailed = true;

            foreach (Character character in CharactersSelectedForTesting)
            {
                // Zkouska jestli vsichni maji uspesny vysledek nebo nikdo neuspel.
                if(character.PassedTheTest == true && everyBodySuccess)
                    everyBodySuccess = false;
               
                if (character.PassedTheTest == false && everyBodyFailed)
                    everyBodyFailed = false;
                
            }

            // vsichni uspeli nebo nikdo. Vsichni by meli mit stejny vysledek.
            if(everyBodySuccess || everyBodyFailed)
            {
                finalResult = CharactersSelectedForTesting[0].PassedTheTest;
                return finalResult;
            }

            // Pokud jsem tady. Skupina se sklada se clenu co uspeli a aji co neuspeli.
            if(tCase.GetPriority == "Success") // Alespon jeden uspel. (Staci jeden character aby uspel na celkovy uspech)
                finalResult = true;
            else // Alespon jeden neuspel. (Staci jeden character aby neuspel na celkovy neuspech)
                finalResult = false;
        }
        else
        {
            finalResult = colectiveResult;
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

    private bool CalculateSuccess(int amountDices , out int numberOfIndividualSuccesses)
    {
        int totalNumberOfSuccesses = 0;
        numberOfIndividualSuccesses = 0;
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

        if(tCase.GetRateMod == 0)
        {
            Debug.LogError("Vole delis nulou ....");
        }

        totalNumberOfSuccesses = numberOfIndividualSuccesses / tCase.GetRateMod;


        if (totalNumberOfSuccesses > 0)
            return true;
        else
            return false;

    }

    public bool OncheckTest(string dataNameFile, StatsClass element)
    {
        var tmp = element.GetStrStat("Result");

        if(tmp == "Success" && this.finalTestResult)
            return true;
        else if  (tmp == "Failure" && !this.finalTestResult)
            return true;
        else
            return false;

    }
}




