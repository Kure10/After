using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButtonBuilder : MonoBehaviour
{

    private Text NameText { get; set; }
    private Text CivilHolder { get; set; }
    private Text TechHolder { get; set; }
    private Text MilitaryHolder { get; set; }
    private Text ElectricHolder { get; set; }
    private Image IlustrationImage { get; set; }
    private Image BackgroundImage { get; set; }
    private Text InfoPanelText { get; set; }
    private Image[] Size;
    private Image[] RawMaterials;

    // vytvorit promene pro Raw material a ElectricConsumption // -> neni to udelane protoze Button Panel je nedodelany.


    private void Awake()
    {
        BuildingSetUp();
    }


    private void BuildingSetUp()
    {
        BackgroundImage = gameObject.transform.GetComponent<Image>();
        NameText = gameObject.transform.GetChild(0).GetComponent<Text>();
        CivilHolder = gameObject.transform.GetChild(1).GetChild(0).GetComponent<Text>();
        TechHolder = gameObject.transform.GetChild(2).GetChild(0).GetComponent<Text>();
        MilitaryHolder = gameObject.transform.GetChild(3).GetChild(0).GetComponent<Text>();
        ElectricHolder = gameObject.transform.GetChild(4).GetChild(0).GetComponent<Text>();
        IlustrationImage = gameObject.transform.GetChild(5).GetComponent<Image>();
        Size = gameObject.transform.GetChild(6).GetComponentsInChildren<Image>();
        InfoPanelText = gameObject.transform.GetChild(7).GetChild(0).GetComponent<Text>();
        RawMaterials = gameObject.transform.GetChild(8).GetComponentsInChildren<Image>();
    }

    public void BuildingChangeStats(BuildingBlueprint buildingBlueprint, BuildingManager bm)
    {
        this.name = buildingBlueprint.name;

        if (buildingBlueprint.Sprite == null)
        {
            Debug.Log("Image errors on the building");
        }
        NameText.text = buildingBlueprint.Name;
        CivilHolder.text = buildingBlueprint.Civil.ToString();
        TechHolder.text = buildingBlueprint.Tech.ToString();
        MilitaryHolder.text = buildingBlueprint.Military.ToString();
        ElectricHolder.text = buildingBlueprint.ElectricConsumption.ToString();
        IlustrationImage.sprite = buildingBlueprint.Sprite;

        for (int i = 0; i < buildingBlueprint.GetCountRawMaterials(); i++)
        {
            if (i > 5)
                return;
            // RawMaterials[i].SetActive(true);
            RawMaterials[i].color = Color.yellow;
        }

        for (int i = 0; i < buildingBlueprint.Size; i++)
        {
            Size[i].color = Color.black;
        }
        InfoPanelText.text = buildingBlueprint.Info;

        if (buildingBlueprint == null || bm == null)
        {
            return;
        }

        BackgroundImage.sprite =  bm.GetSprite(buildingBlueprint.GetSector()); // buildingBlueprint.Sprite;

        BroadcastMessage("CacheBuilding", buildingBlueprint);
    }

}
