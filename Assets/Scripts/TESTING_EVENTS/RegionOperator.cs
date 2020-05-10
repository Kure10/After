﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionOperator : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] Region region;
    [SerializeField] GameObject exploreQuestionButton;
    //[SerializeField] 
    private RegionControler regionControler;

    private Image image;

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
        regionControler.ChangeRegionState(this.region, this.image);
    }

    public void OpenExplorePanel()
    {
        if (this.region.IsExplored || this.region.IsInDarkness)
            return;

        exploreQuestionButton.SetActive(true);
        exploreQuestionButton.transform.position = Input.mousePosition;
        uButtonExploreScript exploreButton = exploreQuestionButton.GetComponent<uButtonExploreScript>();
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

}