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

    private Mission _currentMission = null;

    List<Character> CharactersSelectedForTesting = new List<Character>();

    private bool finalTestResult = false;
    private EventTestCase _tCase;

    private BattleStartData _battleStartData = new BattleStartData();

    public EventPanel GetEventPanel { get { return this.eventPanel; } }

    public void Awake()
    {
        eventPanel.GetMinimizeButton.onClick.RemoveAllListeners();
        eventPanel.GetMinimizeButton.onClick.AddListener(Minimaze);
    }

    #region Public Methods

    public void EventTrigered(Mission mission, bool isFinalEvent = false)
    {
        _currentMission = mission;
        StatsClass _event = null;
        if (isFinalEvent)
        {
            _event = eventManager.ChoiseFinalEvent(_currentMission.FinalEventID);
        }
        else
        {
             _event = eventManager.ChoiseRandomEvent(_currentMission.DifficultyMin, _currentMission.DifficultyMax, _currentMission.GetEmergingTerrains);
        }
        // choise Random Event..

        // PreWarm Pictureteamevent
        Sprite sprite = spriteLoader.LoadEventSprite(_event.GetStrStat("EventPicture"));
        eventPanel.SetImage(sprite);

        // Work with data..
        slave = eventManager.resolveMaster.AddDataSlave("Events", _event.Title);
        slave.StartResolve();
        Dictionary<string, List<StatsClass>> output = slave.Resolve();

        AddCharactersPrefabFromMissionToEvent();
        eventPanel.SetAmountTestedCharacters(_tCase, CharactersSelectedForTesting.Count);

        eventBlocker.SetActive(true);
        this.eventPanel.gameObject.SetActive(true);

        LoadEventSteps(output);

    }

    #endregion

    #region Private Methods

    private void LoadEventSteps(Dictionary<string, List<StatsClass>> output)
    {
        // windowSpec nastavit na default..
        foreach (KeyValuePair<GameObject, Character> characterInEvent in eventPanel.GetCharactersOnEvent)
        {
            uWindowSpecialist uWindow = characterInEvent.Key.GetComponent<uWindowSpecialist>();
            Character character = characterInEvent.Value;

            uWindow.RefreshCharacterInfo(false);
            uWindow.TurnOffResult();
        }

        // Jedu pres vsechny stavy
        foreach (StatsClass statsClass in output["Result"])
        {
            SetNextStepEvent(statsClass);
        }
    }


    private Mission SetNextStepEvent(StatsClass statClass)
    {
        var title = statClass.Title;
        var number = statClass.GetIntStat("$T");

        switch (number)
        {
            case 1:
                ProcessOptions(statClass, title);
                return _currentMission;
            case 2:
                ProcessFight(statClass, title);
                return _currentMission;
            case 3:
                ProcessMonster( statClass, title);
                return _currentMission;
            case 4:
                ProcessTest( statClass, title);
                return _currentMission;
            case 5:
                ProcessChange( statClass, title);
                return _currentMission;
            case 6:
                ProcessEvaluation(statClass, title);
                return _currentMission;
            default:
                Debug.LogWarning("Warning event was created with error: " + number + " : " + title);
                return _currentMission;
        }
    }

    private void ProcessOptions(StatsClass item, string title)
    {
        var buttonText = item.GetStrStat("OptionType");
        var buttonDescription = item.GetStrStat("Option");
        eventPanel.CreateButon(() => SelectionButton(int.Parse(title)), buttonText, buttonDescription);
    }

    private void ProcessFight(StatsClass statClass, string title)
    {
        //
        eventPanel.CreateButon( () => SelectionButton(int.Parse(title)), "Won Battle.." + title, "Tady nic neni proste jsi vyhral..");

        BattleController.OnBattleLost -= BattleEvaluation;
        BattleController.OnBattleLost += BattleEvaluation;

        BattleController.OnBattleEnd -= RefreshCharacterSlots;
        BattleController.OnBattleEnd += RefreshCharacterSlots;

        BattleController.OnBattleEnd -= RestartBattleStartData;
        BattleController.OnBattleEnd += RestartBattleStartData;


        foreach (Character character in _currentMission.GetCharactersOnMission)
        {
            _battleStartData.AddPlayerBattleData(character);
        }

        _battleStartData.AddCharacterFromMission(_currentMission.GetCharactersOnMission);

        BattleType battleType = BattleType.Testing;
        string type = statClass.GetStrStat("BattleType");
        bool checkParse = Enum.TryParse(type, out battleType);
        _battleStartData.battleType = battleType;

        _battleStartData.WinEvaluation.statClassNumber = int.Parse(title);
        _battleStartData.WinEvaluation.mission = _currentMission;

        battleController.StartBattle(_battleStartData);
    }

    private void ProcessMonster(StatsClass statClass, string title)
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

    private void ProcessTest(StatsClass statClass, string title)
    {

        _tCase = null; // ToDo nevim co to udela s new tak radci tu davam null musim se zeptat..

        eventPanel.SetState = EventPanel.PanelStates.Test;

        _tCase = new EventTestCase(statClass);

        eventPanel.SetupTestingState(_tCase);

        eventPanel.SetAmountTestedCharacters(_tCase, CharactersSelectedForTesting.Count);

        foreach (KeyValuePair<GameObject, Character> characterInEvent in eventPanel.GetCharactersOnEvent)
        {
            uWindowSpecialist uWindow = characterInEvent.Key.GetComponent<uWindowSpecialist>();
            Character character = characterInEvent.Value;
            uWindow.GetMainButton.onClick.RemoveAllListeners();

            if (_tCase.GetClass == EventTestCase.ClassTest.Together)
                SelectCharacterForTest(character, uWindow);
            else
                uWindow.GetMainButton.onClick.AddListener(delegate () { SelectCharacterForTest(character, uWindow); });
        }

        eventPanel.GetContinueButton.onClick.RemoveAllListeners();
        eventPanel.GetContinueButton.onClick.AddListener(() => ContinueButton());

        eventPanel.buttonForTMPProceed.onClick.RemoveAllListeners();
        eventPanel.buttonForTMPProceed.onClick.AddListener(() => TMP_ProceedButton(int.Parse(title), _currentMission));

        // cerna magie vratit se k tomuto
        eventManager.resolveMaster.ResolveCondition += OncheckTest;

        StringBuilder sb = new StringBuilder();
        sb.Clear();

        _tCase.GetType.ToString();

        sb.Append("Info Testu:");
        sb.AppendLine();
        sb.Append("Typ testu : " + _tCase.GetType.ToString() + " Naročnost : " + _tCase.GetDifficulty);
        sb.AppendLine();
        sb.Append("Zahrnuta karma : " + _tCase.GetKarmaInfluence);
        sb.AppendLine();
        sb.Append("Modifikator Miry uspechu : " + _tCase.GetRateMod);
        sb.AppendLine();
        sb.Append("Testing Atribute : " + _tCase.ReturnTestingAtribute());
        sb.AppendLine();
        sb.Append("Druh testu : " + _tCase.GetClass.ToString());
        sb.AppendLine();
        sb.Append("Prioritizace : " + _tCase.GetPriority);
        sb.AppendLine();
        sb.Append("Testovany subject : " + _tCase.GetTestingSubjects.ToString()) ;
        sb.AppendLine();
        sb.Append("Min subjektu : " + _tCase.GetMinCharParticipation + " Max subjektu : " + _tCase.GetMaxCharParticipation);

        eventPanel.testingTMPinfo2.text = sb.ToString();

    }

    private void ProcessChange(StatsClass item, string title)
    {
        if (item.GetBoolStat("AddTimeChange"))
        {
            string isDelayed = item.GetStrStat("TimeChange");
            int timeDelay = item.GetIntStat("TimeDelay");

            if (isDelayed == "Delay")
                _currentMission.Distance += timeDelay;
            else
                _currentMission.Distance -= timeDelay;
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
            List<Character> afectedCharacters = SelectTarget(item.GetStrStat("SecAtrChangeTarg"), _currentMission);
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
            OnEventEnd.Invoke(_currentMission);
            eventBlocker.SetActive(false);
            isEventRunning = false;

        }
        else if (item.GetBoolStat("BattleLost"))
        {
            finalTestResult = false;
            CharactersSelectedForTesting.Clear();
            RemoveCharactersGameObjectFromEvent();
            this.eventPanel.gameObject.SetActive(false);
            TimeControl.IsTimeBlocked = false;
            OnEventEnd.Invoke(_currentMission);
            eventBlocker.SetActive(false);
            isEventRunning = false;

            _currentMission.Distance = 0;
        }
    }

    private void ProcessEvaluation( StatsClass item, string title)
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



    private void AddCharactersPrefabFromMissionToEvent()
    {
        eventPanel.GetCharactersOnEvent.Clear();
        eventPanel.DisableCharacterContent();

        for (int i = 0; i < _currentMission.GetCharactersOnMission.Count; i++)
        {
            Character character = _currentMission.GetCharactersOnMission[i];
            GameObject go = Instantiate(eventPanel.GetCharacterButtonPrefab);
            go.transform.localScale = new Vector3(1, 1, 1);

            uWindowSpecialist uWindow = go.GetComponent<uWindowSpecialist>();
            uWindow.SetAll(character);
            //uWindow.CharacterInWindow = character;
            eventPanel.AddCharacterToSelectionContent(go, character);
            uWindow.PopulateItemSlots(character, false);
            uWindow.PopulateBackpackItemSlots(character, false);

            uWindow.AddActionsOnItemClicked(delegate { eventPanel.ReCalculatePositions();});
            uWindow.GetMainButton.onClick.RemoveAllListeners();

            foreach (SpecInventorySlot slot in uWindow.GetCharacterSlots())
            {
                slot.OnItemChangeCallBack += character.OnItemChange;
                //DragAndDropManager.Instantion.OnItemResponseReaction += OnItemDragResponce
            }

            foreach (SpecInventorySlot backpackSlot in uWindow.GetCharacterBackpackSlots())
            {
                backpackSlot.OnItemChangeCallBack += character.OnItemChange;
            }
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

    private void RefreshCharacterSlots ()
    {
        foreach (KeyValuePair<GameObject, Character> characterInEvent in eventPanel.GetCharactersOnEvent)
        {
            uWindowSpecialist uWindow = characterInEvent.Key.GetComponent<uWindowSpecialist>();
            Character character = characterInEvent.Value;

            uWindow.CleanAllItemSlots();
            uWindow.RefreshCharacterInfo(false);
            uWindow.PopulateItemSlots(character,false);
            uWindow.PopulateBackpackItemSlots(character,false);
        }
    }

    private void RestartBattleStartData()
    {
        _battleStartData.RestartDataForNewCombat();
    }

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

    public void BattleEvaluation(StatsClass statClass)
    {
        var objectResult = statClass.GetStat("BattleResult");
        bool isVictory = (bool)objectResult;
        object unknowMission = statClass.GetStat("Mission");
        Mission mission = unknowMission as Mission;
 
        int tittle = statClass.GetIntStat("$T");
    
        if (isVictory)
        {
            // victory
            SelectionButton(tittle);
        }
        else
        {
            // lost
            eventPanel.DestroyAllButtons();
            SetNextStepEvent(statClass);
        }

    }

    #endregion

    #region Buttons Methods

    private void TMP_ProceedButton(int numberOfBrachToResolve, Mission mission)
    {
        slave.StartResolve(numberOfBrachToResolve);
        var output = slave.Resolve();

        LoadEventSteps(output);
    }

    private void SelectionButton(int numberOfBrachToResolve)
    {
        eventPanel.DestroyAllButtons();

        slave.StartResolve(numberOfBrachToResolve);
        var output = slave.Resolve();

        LoadEventSteps(output);
    }

    private void ContinueButton()
    {
        int selectedCharacters = CharactersSelectedForTesting.Count;

        if(_tCase.GetClass == EventTestCase.ClassTest.Together)
        {
            this.finalTestResult = StartThrowTest();
        }
        else
        {
            if (selectedCharacters >= _tCase.GetMinCharParticipation && selectedCharacters <= _tCase.GetMaxCharParticipation)
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
        }

       

        // ToDo jenom vypis pro testovani
        StringBuilder sb = new StringBuilder();
        sb.Clear();

        foreach (Character item in CharactersSelectedForTesting)
        {
            int failure = item.AmountDicesInLastTest - item.AmountSuccessDicesInLastTest;
            sb.Append(item.GetName() + " " + "měl kostek: " + item.AmountDicesInLastTest + " " +
                "Z toho uspel: " + item.AmountSuccessDicesInLastTest + " neuspel: " + failure + "." +
                "---->  uspechy s modifikatore. " + item.AmountSuccessDicesInLastTest / _tCase.GetRateMod);
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

        eventPanel.SetAmountTestedCharacters(_tCase, CharactersSelectedForTesting.Count);
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

    private bool StartThrowTest()
    {
        bool finalResult;
        bool colectiveResult = false;
        int successDices = 0;

        if (_tCase.GetClass == EventTestCase.ClassTest.Separately)
        {
            foreach (Character character in CharactersSelectedForTesting)
            {
                
                int dices = CalculateAmountDices(character);
                character.PassedTheTest = CalculateSuccess(dices, out successDices);
                character.AmountDicesInLastTest = dices;
                character.AmountSuccessDicesInLastTest = successDices;
            }
        }
        else if (_tCase.GetClass == EventTestCase.ClassTest.Together)
        {
            int dices = 0;

            foreach (Character character in CharactersSelectedForTesting)
            {
                dices += CalculateAmountDices(character);
            }


            colectiveResult = CalculateSuccess(dices, out successDices);

            // vsem se nastaví stejny vysledek
            foreach (Character character in CharactersSelectedForTesting)
            {
                character.PassedTheTest = colectiveResult;
                character.AmountDicesInLastTest = dices;
                character.AmountSuccessDicesInLastTest = successDices;
            }
                
        }

        if (_tCase.GetClass == EventTestCase.ClassTest.Separately)
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
            if(_tCase.GetPriority == "Success") // Alespon jeden uspel. (Staci jeden character aby uspel na celkovy uspech)
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

        if (_tCase.IsTestingLevel)
            amountDices += character.Stats.level;
        if (_tCase.IsTestingMilitary)
            amountDices += character.Stats.military;
        if (_tCase.IsTestingTechnical)
            amountDices += character.Stats.tech;
        if (_tCase.IsTestingSocial)
            amountDices += character.Stats.social;
        if (_tCase.IsTestingScience)
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

            if (resultOfOneThrow >= _tCase.GetDifficulty)
                numberOfIndividualSuccesses++;

            resultOfOneThrow = 0;
        }

        if(_tCase.GetRateMod == 0)
        {
            Debug.LogError("Vole delis nulou ....");
        }

        totalNumberOfSuccesses = numberOfIndividualSuccesses / _tCase.GetRateMod;


        if (totalNumberOfSuccesses > 0)
            return true;
        else
            return false;

    }

    public bool OncheckTest(string dataNameFile, string dialogID,StatsClass element)
    {
        var tmp = element.GetStrStat("Result");

        return true;

        if(tmp == "Success" && this.finalTestResult)
            return true;
        else if  (tmp == "Failure" && !this.finalTestResult)
            return true;
        else
            return false;

    }
}




