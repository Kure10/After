using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowMissionController : MonoBehaviour
{
    [SerializeField] private SpecialistControler theSC;

    [SerializeField] uWindowMission uWindowShowMission;

    [SerializeField] uWindowMissionSpecSelection uWindowSpecSelection;

    

    private MissionPanelState state;

    private Mission currentMission;

    private List<Specialists> specialistPreSelectedToMission = new List<Specialists>();

    private List<Specialists> specialistReadyToMission = new List<Specialists>();

    private List<GameObject> SiblingsObject = new List<GameObject>();

    public MissionPanelState State { set { this.state = value; } }


    private void Awake()
    {
        var button = uWindowShowMission.GetCloseMissionButton;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(DisableShowMissionPanel);
    }

    public void OnEnable()
    {
        switch (this.state)
        {
            case MissionPanelState.normal:
                uWindowShowMission.ButtonStartText = "Zacit missi";
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
        this.PrepairSelectionWindow(this.theSC.PassSpecToMissionSelection());
        uWindowSpecSelection.gameObject.SetActive(true);
    }

    private void PrepairSelectionWindow(List<Specialists> specList)
    {
        var prefab = this.uWindowSpecSelection.SpecPrefab;
        var holder = this.uWindowSpecSelection.SpecHolder;

        this.uWindowSpecSelection.GetBackButton.onClick.RemoveAllListeners();
        this.uWindowSpecSelection.GetConfirmButton.onClick.RemoveAllListeners();

        this.uWindowSpecSelection.GetBackButton.onClick.AddListener(this.BackButton);
        this.uWindowSpecSelection.GetConfirmButton.onClick.AddListener(this.ConfirmSelectedSpecialist);

        this.specialistPreSelectedToMission.Clear();


        foreach (var item in specList)
        {
            Debug.Log("PreSelected : " + item.isPreSelectedOnMission + " isSelectedOnMission : " + item.isSelectedOnMission);
        }


        foreach (Transform item in holder.transform)
        {
            Destroy(item.gameObject);
        }

        var currentAmountSelectedSpec = specialistReadyToMission.Count;
        var missionSelectedMaxSpec = currentMission.SpecMax;
        this.uWindowSpecSelection.setInfoText(currentAmountSelectedSpec, missionSelectedMaxSpec);

        foreach (Specialists spec in specList)
        {
            var go = Instantiate(prefab, holder.transform);
            var uWindow = go.GetComponent<uWindowSpecialist>();
            uWindow.SetAll(spec);

            var but = go.GetComponent<Button>();

            if (but == null)
            {
                Debug.LogError("Error Nejaky buttonSpecialist prefab nemel button component");
                continue;
            }

            but.onClick.RemoveAllListeners();

            if (spec.isSelectedOnMission)
            {
                // Debug.Log(spec.FullName + "   toto je muj specialista co tu nema uz byt..");

                specialistPreSelectedToMission.Add(spec);
                spec.isPreSelectedOnMission = true;
                uWindow.SetSuperimposePanel(true,"Specialista je už vybran.");
                but.onClick.AddListener(() => PreSelectSpecialistToMission(spec, uWindow));
            }
            else if (spec.isOnMission)
            {
                uWindow.SetSuperimposePanel(true, "Specialista je na missi.");
                // Some Action Todo
            }
            else
            {
                but.onClick.AddListener(() => PreSelectSpecialistToMission(spec,uWindow));
            }

        }
    }



    public void ConfirmSelectedSpecialist ()
    {
        //this.specialistReadyToMission.Clear();
        //this.specialistReadyToMission.AddRange(this.specialistPreSelectedToMission);
        this.specialistReadyToMission = this.specialistPreSelectedToMission;

        var specCount = specialistReadyToMission.Count;

        this.uWindowShowMission.SetSpecMinMaxText(specCount, uWindowShowMission.GetMaxSpecOnMission);

        var siblingsCount = this.SiblingsObject.Count;

        for (int i = siblingsCount - 1; i >= 0; i--)
        {
            var ob = this.SiblingsObject[i];
            this.SiblingsObject.Remove(ob);
            Destroy(ob.gameObject);
        }

        var prefab = this.uWindowSpecSelection.SpecPrefab;
        var content = this.uWindowShowMission.SpecContent;


        foreach (Specialists readySpec in specialistReadyToMission)
        {
           // Debug.Log(specialistReadyToMission.Count + "   jsem tady ");

            readySpec.isSelectedOnMission = true;
            readySpec.isPreSelectedOnMission = false;

            var specGameObject = Instantiate(prefab, content.transform);
            specGameObject.transform.SetAsFirstSibling();
            this.SiblingsObject.Add(specGameObject);

            var uWindow = specGameObject.GetComponent<uWindowSpecialist>();
            uWindow.SetAll(readySpec);
            uWindow.SetSuperimposePanel(false);

            Button but = specGameObject.GetComponent<Button>();
            but.onClick.RemoveAllListeners();
            but.onClick.AddListener(OpenSelectionPanel);
        }

        uWindowSpecSelection.gameObject.SetActive(false);

        siblingsCount = this.SiblingsObject.Count;

      //  Debug.Log("specCount: " + specCount + " siblingsCount: " + siblingsCount + "  currentMission.SpecMax-> " + currentMission.SpecMax);

        if (siblingsCount < currentMission.SpecMax)
        {
            Debug.Log("pridavam kurva");
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
        for (int i = this.specialistPreSelectedToMission.Count - 1; i >= 0; i--)
        {
            var spec = this.specialistPreSelectedToMission[i];

            if (spec.isPreSelectedOnMission)
            {
                if (specialistPreSelectedToMission.Contains(spec))
                    specialistPreSelectedToMission.Remove(spec);
            }

            spec.isPreSelectedOnMission = false;
        }


        for (int i = this.specialistReadyToMission.Count - 1; i >= 0; i--)
        {
            var spec = this.specialistReadyToMission[i];
            spec.isSelectedOnMission = true;
        }

        uWindowSpecSelection.gameObject.SetActive(false);

    }

    private void PreSelectSpecialistToMission(Specialists spec, uWindowSpecialist uWindow)
    {
       
        var count = specialistPreSelectedToMission.Count;
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
            
        if (specialistPreSelectedToMission.Contains(spec))
            specialistPreSelectedToMission.Remove(spec);
        else
            specialistPreSelectedToMission.Add(spec);


        var currentAmountSelectedSpec = specialistReadyToMission.Count;
        var missionSelectedMaxSpec = currentMission.SpecMax;

        this.uWindowSpecSelection.setInfoText(currentAmountSelectedSpec, missionSelectedMaxSpec);

    }

    public List<Specialists> StartMission()
    {
        foreach (var item in this.specialistReadyToMission)
        {
            item.isOnMission = true;
        }

        this.theSC.MoveSpecialistToMission(this.specialistReadyToMission);

        return this.specialistReadyToMission;
    }

    public void SetUpWindow(Mission mission, bool isInProgress)
    {
        // for text StartButton
        this.currentMission = mission;

        // setup
        uWindowShowMission.MissionName = mission.Name;
        uWindowShowMission.MissionType = mission.ConvertMissionTypeStringData(mission.Type);
        uWindowShowMission.MissionDistance = mission.Distance;
        uWindowShowMission.MissionLevel = mission.LevelOfDangerous;
        uWindowShowMission.MissionTerrainList = mission.GetEmergingTerrains;
        uWindowShowMission.MissionTime = mission.MissionTime;
        uWindowShowMission.DesriptionText = mission.Description;
        uWindowShowMission.Sprite = mission.Image;
        
        this.CreateSpecAddButton(mission, isInProgress);

        uWindowShowMission.SetSpecMinMaxText(mission.SpecMin, mission.SpecMax);

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

        this.specialistReadyToMission.Clear();

        Debug.Log("isInProgress: " + isInProgress);
        if(isInProgress)
        {
            foreach (var item in mission.SpecialistOnMission)
            {
                var go = Instantiate(specPrefab, holder.transform);
                var button = go.GetComponent<Button>();
                this.SiblingsObject.Add(go);
                button.onClick.RemoveAllListeners();

                var uWindow = go.GetComponent<uWindowSpecialist>();
                uWindow.SetAll(item);
                uWindow.SetSuperimposePanel(false);

                specialistReadyToMission.Add(item);
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

    private void DisableShowMissionPanel()
    {
        foreach (Specialists spec in this.specialistReadyToMission)
        {
            spec.isSelectedOnMission = false;
            //if (spec.isSelectedOnMission)
            //    spec.isSelectedOnMission = false;
        }

        this.uWindowShowMission.gameObject.SetActive(false);

        this.SiblingsObject.Clear();
        this.specialistReadyToMission.Clear();
    }


    public enum MissionPanelState { normal , inRepeatTime , inProgress , Complete}

}
