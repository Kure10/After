using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecSetGui : MonoBehaviour
{
    Transform firstPanel;

    Transform panelImage;

    Transform panelStats;


    // Start is called before the first frame update
    void Start()
    {
        firstPanel = transform.GetChild(0);
        panelImage = firstPanel.GetChild(0);
        panelStats = firstPanel.GetChild(1);
    }

    private void SetImagePanel(Specialists spec)
    {
        var image = panelImage.GetChild(0).GetComponent<Image>();
        image.sprite = spec.GetSprite();
    }

    private void CalcHealtandStamina(Specialists spec)
    {
        var barHealth = panelImage.GetChild(1).GetChild(2);
        barHealth.transform.localScale = new Vector3(spec.PercentHealt / 100, 1f, 1f);


        var barStamina = panelImage.GetChild(2).GetChild(2);
        barStamina.transform.localScale = new Vector3(spec.PercentStamina / 100, 1f, 1f);
    }

    private void SetStatsPanel(Specialists spec)
    {
        Text text = panelStats.GetChild(0).GetChild(1).GetComponentInChildren<Text>();
        text.text = spec.FullName.ToString();

        text = panelStats.GetChild(1).GetChild(1).GetComponentInChildren<Text>();
        text.text = spec.Level.ToString();

        text = panelStats.GetChild(2).GetChild(1).GetComponentInChildren<Text>();
        text.text = spec.Mil.ToString();

        text = panelStats.GetChild(3).GetChild(1).GetComponentInChildren<Text>();
        text.text = spec.Scl.ToString();

        text = panelStats.GetChild(4).GetChild(1).GetComponentInChildren<Text>();
        text.text = spec.Tel.ToString();

        text = panelStats.GetChild(5).GetChild(1).GetComponentInChildren<Text>();
        text.text = spec.Sol.ToString();

        // ToDo Jeste nevim jak to udelam a za jakych podminek se meni stav.
        text = panelStats.GetChild(6).GetChild(1).GetComponentInChildren<Text>();
        text.text = "Todo Nezapomen dodelat :D";

        text = panelStats.GetChild(10).GetChild(0).GetComponentInChildren<Text>();
        text.text = spec.Kar.ToString();
    }

    public void SetAll(Specialists spec)
    {
        SetImagePanel(spec);
        CalcHealtandStamina(spec);
        SetStatsPanel(spec);
    }

    public void SetUp()
    {
        firstPanel = transform.GetChild(0);
        panelImage = firstPanel.GetChild(0);
        panelStats = firstPanel.GetChild(1);
    }

    
}
