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
    private Image IlustrationImage { get; set; }
    private Image BackgroundImage { get; set; }
    private Text InfoPanelText { get; set; }
    private Image[] Size;

    // vytvorit promene pro Raw material a ElectricConsumption // -> neni to udelane protoze Button Panel je nedodelany.


    private void Awake()
    {
        BuildingSetUp();
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
        // musim vytvořit promenou a nacachovat ElectricConsumption // To same se tyka Raw materials //
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
        IlustrationImage.sprite = building.Sprite;
        // no a tady priradim odpovídající hodnotu co ma budova.ElectricConsumption // To same se tyka Raw materials //
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
