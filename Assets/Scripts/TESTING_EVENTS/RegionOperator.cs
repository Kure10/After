using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionOperator : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] Region region;
    [SerializeField] GameObject exploreQuestionButton;

    [SerializeField] List<GameObject> missionsAfterExplore = new List<GameObject>();

    private RegionControler regionControler;

    // info about region
    private bool currentRegionIsExplored = false; // atim se nepouziva ale asi bude

    private Image image;

    #region Properities

    public bool SetCurrentRegionIsExplored { set { currentRegionIsExplored = value; } } // myslim si ze to budu potrebovat na rozeznani typu misse

    #endregion

    private void Awake()
    {
        this.regionControler = GameObject.FindObjectOfType<RegionControler>();
        image = GetComponent<Image>();
        image.sprite = this.region.GetSprite;
        InicializationRegion();
    }


    public void ExploreRegion()
    {
        if (this.region.IsInShadow)
        {
            this.region.IsExplored = true;
            this.region.RevealNeighbors();
            this.ActivateAdditionalMissions(true);
            regionControler.RefreshRegions();
        }
    }

    public void InicializationRegion()
    {
        this.name = this.region.regionName;
        if (this.region.IsStartingRegion)
        {
            this.region.IsInShadow = true;
        }
        else
        {
            this.region.IsInDarkness = true;
        }
        this.ActivateAdditionalMissions(false);
        regionControler.ChangeRegionState(this.region, this.image);
    }

    public void OpenExplorePanel()
    {
        if (this.region.IsExplored || this.region.IsInDarkness)
            return;

        exploreQuestionButton.SetActive(true);
        exploreQuestionButton.transform.position = Input.mousePosition;
    }

    private void ActivateAdditionalMissions(bool activate)
    {

        foreach (GameObject item in missionsAfterExplore)
        {
            item.SetActive(activate);
        }
    }


    public void StartExploreMission()
    {
        regionControler.StartExploreMision(this);
    }
    public void CloseExplorePanel()
    {
        this.exploreQuestionButton.SetActive(false);
    }

    public Image GetImage()
    {
        return this.image;
    }

    public Region GetRegion()
    {
        return this.region;
    }

    public void ShowMissionWithID (int id)
    {

    }

}
