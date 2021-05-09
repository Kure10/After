using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoPanel : MonoBehaviour
{
    // mozna jmeno
    [Header("Stats")]
    [SerializeField] Text _damage;
    [SerializeField] Text _threat;
    [SerializeField] Text _range;
    [SerializeField] Text _level;

    [SerializeField] Text _health;

    [SerializeField] GameObject healthImageValue;

    [Header("Image")]
    [SerializeField] Image _image;

    [Header("Buttons")]
    [SerializeField] Button _skipTurnButton;

    public void UpdateStats(Unit unit)
    {
        _threat.text = unit._threat.ToString();

        UpdateHealthBar(unit.CurrentHealth, unit.MaxHealth);

        UpdateRange(unit._rangeMax);

        _damage.text = unit._damage.ToString();

        _image.sprite = unit._sprite;
    }

    private void UpdateHealthBar(int current, int max)
    {
        _health.text = $"{current} / {max}";

        float amount = (float)current / (float)max;

        healthImageValue.transform.localScale = new Vector3(amount, 1, 1);
    }

    private void UpdateRange(int range)
    {
        if (range == 0)
        {
            this._range.text = "M";
        }
        else
        {
            this._range.text = range.ToString();
        }
    }

    //public void DisablePanel()
    //{
    //    StartCoroutine(Disable(1.5f));
    //}

    //IEnumerator Disable (float time)
    //{
    //    yield return new WaitForSeconds(time);
    //    this.gameObject.SetActive(false);
    //}

}
