using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowMissionController : MonoBehaviour
{
    [SerializeField] private PanelTime time;

    [SerializeField] private SpecialistControler specialistController;

    [SerializeField] uWindowMission uWindowShowMission;

    [SerializeField] public uWindowMissionSpecSelection uWindowSpecSelection;

    [SerializeField] private GameObject blocker;

    private MissionPanelState state;

    private Mission currentMission;

    private List<Character> charactersPreSelectedToMission = new List<Character>();

    private List<Character> charactersReadyToMission = new List<Character>();

    private List<GameObject> SiblingsObject = new List<GameObject>();

    public MissionPanelState State { set { this.state = value; } }

    public int AmountReadyCharactersToMission { get { return charactersReadyToMission.Count; } }

    public void ActivateBlocker()
    {
        this.blocker.SetActive(true);
    }

    public void DisableBlocker()
    {
        this.blocker.SetActive(false);
    }

    private void Awake()
    {
        var closeButton = uWindowShowMission.GetCloseMissionButton;
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(DisableShowMissionPanel);
    }

    public void OnEnable()
    {
        switch (this.state)
        {
            case MissionPanelState.normal:
                uWindowShowMission.ButtonStartText = "Zacit misi";
                break;
            case MissionPanelState.inRepeatTime:
                if (currentMission != null)
                    CalcTimeForReactivationMission(currentMission.RepeatableTime);
                break;
            case MissionPanelState.inProgress:
                if (currentMission != null)
                    uWindowShowMission.ButtonStartText = "Zrusit misi";
                break;
            case MissionPanelState.Complete:
                if (currentMission != null)
                    uWindowShowMission.ButtonStartText = "Misse je splnena";
                break;

            default:
                break;
        }
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void CalcTimeForReactivationMission(float missionTime)
    {
        string missionAvailableIn = null;

        int seconds = (int)(missionTime % 60);
        int minutes = (int)(missionTime / 60) % 60;
        int hours = (int)(missionTime / 3600) % 24;
        int days = (int)(missionTime / 86400);


        string sec = seconds.ToString("D2");
        string min = minutes.ToString("D2");
        string hour = hours.ToString("D2");
        string dayS = days.ToString();


        missionAvailableIn = $"Misse dostupna za dnu:{dayS} hodin:{hour} min:{min} sec:{sec}";

        uWindowShowMission.ButtonStartText = missionAvailableIn;
    }

    private void OpenSelectionPanel()
    {
        this.PrepairSelectionWindow(this.specialistController.PassSpecToMissionSelection());
        uWindowSpecSelection.ActivateWindow();
        uWindowSpecSelection.ActivateBlocker();
    }

    private void PrepairSelectionWindow(List<Character> characterList)
    {
        var prefab = this.uWindowSpecSelection.SpecPrefab;
        var holder = this.uWindowSpecSelection.SpecHolder;

        this.uWindowSpecSelection.GetBackButton.onClick.RemoveAllListeners();
        this.uWindowSpecSelection.GetConfirmButton.onClick.RemoveAllListeners();

        this.uWindowSpecSelection.GetBackButton.onClick.AddListener(this.BackButton);
        this.uWindowSpecSelection.GetConfirmButton.onClick.AddListener(this.ConfirmPreSelectedSpecialist);

        this.charactersPreSelectedToMission.Clear();

        // Destroy all character prefabs (panels)
        foreach (Transform item in holder.transform)
        {
            Destroy(item.gameObject);
        }

        var currentAmountSelectedSpec = charactersReadyToMission.Count;
        var missionSelectedMaxSpec = currentMission.SpecMax;
        this.uWindowSpecSelection.setInfoText(currentAmountSelectedSpec, missionSelectedMaxSpec);

        // pres vsechny charactery kteri muzou na missi
        foreach (Character character in characterList)
        {
            Specialists spec = character.GetBlueprint();

            GameObject go = Instantiate(prefab) as GameObject;
            go.transform.SetParent(holder.transform);
            var uWindow = go.GetComponent<uWindowSpecialist>();
            uWindow.SetAll(character);
            uWindow.PopulateItemSlots(character, true);


            var but = go.GetComponent<Button>();

            if (but == null)
            {
                Debug.LogError("Error Nejaky buttonSpecialist prefab nemel button component");
                continue;
            }

            but.onClick.RemoveAllListeners();

            if (spec.isSelectedOnMission)
            {
                charactersPreSelectedToMission.Add(character);
                spec.isPreSelectedOnMission = true;
                uWindow.ActivateCoverPanel("Specialista je už vybran.");
                but.onClick.AddListener(() => PreSelectSpecialistToMission(character, uWindow));
            }
            else if (spec.IsOnMission)
            {
                uWindow.ActivateCoverPanel("Specialista je na misi.");
                // Some Action Todo
            }
            else
            {
                but.onClick.AddListener(() => PreSelectSpecialistToMission(character, uWindow));
            }

        }
    }



    public void ConfirmPreSelectedSpecialist ()
    {
        this.charactersReadyToMission.Clear();
        this.charactersReadyToMission.AddRange(this.charactersPreSelectedToMission);

        var specCount = charactersReadyToMission.Count;

        this.uWindowShowMission.SetSpecMinMaxText(charactersReadyToMission.Count, uWindowShowMission.GetMaxSpecOnMission);

        var siblingsCount = this.SiblingsObject.Count;

        for (int i = siblingsCount - 1; i >= 0; i--)
        {
            var ob = this.SiblingsObject[i];
            this.SiblingsObject.Remove(ob);
            Destroy(ob.gameObject);
        }

        var prefab = this.uWindowSpecSelection.SpecPrefab;
        var content = this.uWindowShowMission.SpecContent;


        foreach (Character character in charactersReadyToMission)
        {
            var readySpec = character.GetBlueprint();

            readySpec.isSelectedOnMission = true;
            readySpec.isPreSelectedOnMission = false;

            var specGameObject = Instantiate(prefab, content.transform);
            specGameObject.transform.SetAsFirstSibling();
            this.SiblingsObject.Add(specGameObject);

            var uWindow = specGameObject.GetComponent<uWindowSpecialist>();
            uWindow.SetAll(character);
            uWindow.PopulateItemSlots(character, true);
            uWindow.DeactivateCoverPanel();

            Button but = specGameObject.GetComponent<Button>();
            but.onClick.RemoveAllListeners();
            but.onClick.AddListener(OpenSelectionPanel);
        }

        uWindowSpecSelection.DisableWindow();
        uWindowSpecSelection.DisableBlocker();

        siblingsCount = this.SiblingsObject.Count;

      //  Debug.Log("specCount: " + specCount + " siblingsCount: " + siblingsCount + "  currentMission.SpecMax-> " + currentMission.SpecMax);

        if (siblingsCount < currentMission.SpecMax)
        {
            var pref = this.uWindowShowMission.PlusPrefab;
            var go = Instantiate(pref, content.transform);
            go.transform.SetAsLastSibling();
            Button but = go.GetComponent<Button>();
            this.SiblingsObject.Add(go);
            but.onClick.RemoveAllListeners();
            but.onClick.AddListener(OpenSelectionPanel);
        }


        
    }

    public void BackButton()
    {
        for (int i = this.charactersPreSelectedToMission.Count - 1; i >= 0; i--)
        {
            Character character = this.charactersPreSelectedToMission[i];

            if (character.GetBlueprint().isPreSelectedOnMission)
            {
                if (charactersPreSelectedToMission.Contains(character))
                    charactersPreSelectedToMission.Remove(character);
            }

            character.GetBlueprint().isPreSelectedOnMission = false;
        }


        for (int i = this.charactersReadyToMission.Count - 1; i >= 0; i--)
        {
            var character = this.charactersReadyToMission[i];
            character.GetBlueprint().isSelectedOnMission = true;
        }

        uWindowSpecSelection.DisableWindow();
        uWindowSpecSelection.DisableBlocker();

    }

    private void PreSelectSpecialistToMission(Character character, uWindowSpecialist uWindow)
    {
        Specialists spec = character.GetBlueprint();
        int count = charactersPreSelectedToMission.Count;
        if (count >= currentMission.SpecMax && !spec.isPreSelectedOnMission)
        {
            Debug.Log("Nemužes pridat dalsiho specialistu....  Limit je: " + currentMission.SpecMax + " na tuto missi.");
            return;
        }

        uWindow.SetActiveSuperimposePanel();

        if (spec.isPreSelectedOnMission)
        {
            spec.isPreSelectedOnMission = false;
            spec.isSelectedOnMission = false;
        }
        else
            spec.isPreSelectedOnMission = true;
            
        if (charactersPreSelectedToMission.Contains(character))
            charactersPreSelectedToMission.Remove(character);
        else
            charactersPreSelectedToMission.Add(character);


        this.uWindowSpecSelection.setInfoText(charactersPreSelectedToMission.Count, currentMission.SpecMax);
    }

    public List<Character> StartMission()
    {

        this.specialistController.MoveSpecialistToMission(this.charactersReadyToMission);
        time.UnpauseGame(fromPopup: true);
        this.DisableBlocker();

        return this.charactersReadyToMission;
    }

    public void SetUpWindow(Mission mission, bool isInProgress)
    {
        // for text StartButton
        this.currentMission = mission;

        // setup
        uWindowShowMission.MissionName = mission.Name;
        uWindowShowMission.MissionType = mission.ConvertMissionTypeStringData(mission.Type);
        uWindowShowMission.MissionDistance = mission.Distance;
       // uWindowShowMission.MissionLevel = LevelOfDangerous.jedna; //mission.LevelOfDangerous; // obtiznost misse pak dodelat..
        uWindowShowMission.MissionTerrainList = mission.GetEmergingTerrains;
       // uWindowShowMission.MissionTime = MissionTime.akorat; // ToDo // mission.MissionTime;
        uWindowShowMission.DesriptionText = mission.Description;
        uWindowShowMission.Sprite = mission.Image;
        
        this.CreateSpecAddButton(mission, isInProgress);

        uWindowShowMission.SetSpecMinMaxText(charactersReadyToMission.Count, mission.SpecMax);

    }

    private void CreateSpecAddButton(Mission mission, bool isInProgress)
    {
        var specPrefab = uWindowSpecSelection.SpecPrefab;
        var plusPrefab = uWindowShowMission.PlusPrefab;
        var holder = uWindowShowMission.SpecContent;
        var prefabCount = mission.SpecMin + 1;

        this.SiblingsObject.Clear();

        foreach (Transform item in holder.transform)
        {
            Destroy(item.gameObject);
        }

        this.charactersReadyToMission.Clear();

        if(isInProgress)
        {
            foreach (Character character in mission.GetCharactersOnMission)
            {
                var go = Instantiate(specPrefab, holder.transform);
                var button = go.GetComponent<Button>();
                this.SiblingsObject.Add(go);
                button.onClick.RemoveAllListeners();

                var uWindow = go.GetComponent<uWindowSpecialist>();
                uWindow.SetAll(character);
                uWindow.DeactivateCoverPanel();
                uWindow.PopulateItemSlots(character, true);

                charactersReadyToMission.Add(character);
            }
        }
        else
        {
            for (int i = 0; i < prefabCount; i++)
            {
                var go = Instantiate(plusPrefab, holder.transform);
                var button = go.GetComponent<Button>();
                this.SiblingsObject.Add(go);
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OpenSelectionPanel);
            }
        }

    }

    public Button GetWindowButton()
    {
        return uWindowShowMission.GetStartMissionButton;
    }

    public void Init()
    {
        uWindowShowMission.Init();
    }

    public void SetActivePanel(bool value)
    {
        uWindowShowMission.SetActivityMissionPanel = value;
    }

    public void DisableShowMissionPanel()
    {
        if (uWindowSpecSelection.IsWindowActive)
        {
            BackButton();
            return;
        }
                
        
        foreach (Character character in this.charactersReadyToMission)
        {
            character.GetBlueprint().isSelectedOnMission = false;
            //if (spec.isSelectedOnMission)
            //    spec.isSelectedOnMission = false;
        }

        this.uWindowShowMission.gameObject.SetActive(false);

        this.SiblingsObject.Clear();
        this.charactersReadyToMission.Clear();

        this.DisableBlocker();
        time.UnpauseGame(fromPopup: true);
    }


    public enum MissionPanelState { normal , inRepeatTime , inProgress , Complete}

}
