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
    [SerializeField] Text _movement;

    [SerializeField] GameObject healthImageValue;
    [SerializeField] GameObject movementImageValue;

    [Header("Image")]
    [SerializeField] Image _image;

    [Header("Buttons")]
    [SerializeField] Button _skipTurnButton;
    [SerializeField] Button _firstWeaponTurnButton;
    [SerializeField] Button _secondWeaponButton;

    public void UpdateStats(Unit unit)
    {
        _threat.text = unit._threat.ToString();

        UpdateHealthBar(unit.CurrentHealth, unit.MaxHealth);
        UpdateMovementBar(unit.GetMovementPoints, unit.GetMaxMovement);

        UpdateRange(unit);

        UpdateMilitary(unit);

        _image.sprite = unit._sprite;

        _level.text = unit._level.ToString();
    }

    private void UpdateHealthBar(int current, int max)
    {
        _health.text = $"{current} / {max}";

        float amount = (float)current / (float)max;

        healthImageValue.transform.localScale = new Vector3(amount, 1, 1);
    }

    private void UpdateMovementBar(int current, int max)
    {
        if (_movement == null)
            return;
        
        _movement.text = $"{current} / {max}";
        float amount = (float)current / (float)max;
        movementImageValue.transform.localScale = new Vector3(amount, 1, 1);
    }

    private void UpdateRange(Unit unit)
    {
        int rangeMax = 0;
        int rangeMin = 0;

        if (unit.ActiveWeapon == null)
        {
            rangeMax = unit._rangeMax;
            rangeMin = unit._rangeMin;
        }
        else
        {
            rangeMax = unit.ActiveWeapon.RangeMax;
            rangeMin = unit.ActiveWeapon.RangeMin;
        }

        string rangeText = $"{rangeMin}/{rangeMax}";

        if(rangeMin == rangeMax)
        {
            rangeText = rangeMax.ToString();
        }

        _range.text = rangeText;
    }

    private void UpdateMilitary(Unit unit)
    {
        if (unit.ActiveWeapon != null)
        {
            int military = BattleSystem.CalcMilitary(unit);

            _damage.text = military.ToString();
        }
        else
        {
            _damage.text = unit._military.ToString();
        }
    }
}
