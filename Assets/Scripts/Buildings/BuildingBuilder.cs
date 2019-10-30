using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingBuilder : MonoBehaviour
{
    public Building building;


    private Text NameText { get; set; }
    private Text CivilHolder { get; set; }
    private Text TechHolder { get; set; }
    private Text MilitaryHolder { get; set; }
    private Image IlustrationImage { get; set; }
    private Image BackgroundImage { get; set; }
    private Text InfoPanelText { get; set; }
    private Image[] Size;



    private void Awake()
    {
        BuildingSetUp();
    }

    // Asi si Ashene uprav tady co budes chtit. 
    public Building GetBuildingInformation()
    {
        return building;
    }

    public void BuildingSetUp()
    {
        BackgroundImage = gameObject.transform.GetComponent<Image>();
        NameText = gameObject.transform.GetChild(0).GetComponent<Text>();
        CivilHolder = gameObject.transform.GetChild(1).GetChild(0).GetComponent<Text>();
        TechHolder = gameObject.transform.GetChild(2).GetChild(0).GetComponent<Text>();
        MilitaryHolder = gameObject.transform.GetChild(3).GetChild(0).GetComponent<Text>();
        IlustrationImage = gameObject.transform.GetChild(4).GetComponent<Image>();
        Size = gameObject.transform.GetChild(5).GetComponentsInChildren<Image>();
        InfoPanelText = gameObject.transform.GetChild(6).GetChild(0).GetComponent<Text>();
    }

    public void BuildingChangeStats(Building build)
    {
        building = build;
        if (building.Sprite == null)
        {
            Debug.Log("Image errors on the building");
        }
        NameText.text = building.Name;
        CivilHolder.text = building.Civil.ToString();
        TechHolder.text = building.Tech.ToString();
        MilitaryHolder.text = building.Military.ToString();
        IlustrationImage.sprite = building.Sprite;
        for (int i = 0; i < building.Size; i++)
        {
            Size[i].color = Color.black;
        }
        InfoPanelText.text = building.Info;
    }

    private void Correction()
    {
       GameObject go = InfoPanelText.gameObject.GetComponentInParent<GameObject>();
       go.SetActive(false);
    }

    public void SetBackgroundImage(BuildingControler bc)
    {
        if (building == null || bc == null)
        {
            return;
        }
        BackgroundImage.sprite = bc.GetSprite(building.GetSector());
    }

}
