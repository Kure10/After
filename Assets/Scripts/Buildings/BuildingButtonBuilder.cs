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

    public void BuildingChangeStats(Building building, BuildingManager bm)
    {
        this.name = building.name;

        if (building.Sprite == null)
        {
            Debug.Log("Image errors on the building");
        }
        NameText.text = building.Name;
        CivilHolder.text = building.Civil.ToString();
        TechHolder.text = building.Tech.ToString();
        MilitaryHolder.text = building.Military.ToString();
        ElectricHolder.text = building.ElectricConsumption.ToString();
        IlustrationImage.sprite = building.Sprite;

        for (int i = 0; i < building.GetCountRawMaterials(); i++)
        {
            if (i > 5)
                return;
            // RawMaterials[i].SetActive(true);
            RawMaterials[i].color = Color.yellow;
        }

        for (int i = 0; i < building.Size; i++)
        {
            Size[i].color = Color.black;
        }
        InfoPanelText.text = building.Info;

        if (building == null || bm == null)
        {
            return;
        }
        BackgroundImage.sprite = bm.GetSprite(building.GetSector());

        BroadcastMessage("CacheBuilding", building);
    }

}
