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

    [SerializeField] GameObject _healthImageValue;
    [SerializeField] GameObject _movementImageValue;

    [Header("Image")]
    [SerializeField] Image _image;

    [Header("Buttons")]
    [SerializeField] Button _skipTurnButton;
    [SerializeField] Button _firstWeaponTurnButton;
    [SerializeField] Button _secondWeaponButton;

    public void UpdateStats(Unit unit)
    {
        DisableUnWantedAtributes(unit);
        _threat.text = unit._threat.ToString();

        UpdateHealthBar(unit.CurrentHealth, unit.MaxHealth);
        UpdateMovementBar(unit.GetMovementPoints, unit.GetMaxMovement);

        UpdateRange(unit);

        UpdateMilitary(unit);

        _image.sprite = unit._sprite;

        _level.text = unit._level.ToString();
    }

    public void UpdateStats(Obstacle obstacle)
    {
        DisableUnWantedAtributes(obstacle: obstacle);

        if(obstacle is IDamageable damagable)
        {
            _threat.text = damagable.GetThreat.ToString();
            UpdateHealthBar(damagable.GetCurrentHealth, damagable.GetMaxHealth);
        }

        _image.sprite = obstacle.GetSprite;
    }

    private void UpdateHealthBar(int current, int max)
    {
        _health.text = $"{current} / {max}";

        float amount = (float)current / (float)max;

        _healthImageValue.transform.localScale = new Vector3(amount, 1, 1);
    }

    private void UpdateMovementBar(int current, int max)
    {
        if (_movement == null)
            return;
        
        _movement.text = $"{current} / {max}";
        float amount = (float)current / (float)max;
        _movementImageValue.transform.localScale = new Vector3(amount, 1, 1);
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

    private void DisableUnWantedAtributes(Unit unit = null , Obstacle obstacle = null)
    {
        if(unit != null)
        {
            _threat.transform.parent.gameObject.SetActive(true);
            _health.transform.parent.gameObject.SetActive(true);
            _healthImageValue.transform.parent.gameObject.SetActive(true);
           // _movement.gameObject.SetActive(true);
            //_movementImageValue.gameObject.SetActive(true);
            _level.transform.parent.gameObject.SetActive(true);
            _damage.transform.parent.gameObject.SetActive(true);
            _range.transform.parent.gameObject.SetActive(true);
        }

        if(obstacle != null)
        {
            _threat.transform.parent.gameObject.SetActive(false);
            _health.transform.parent.gameObject.SetActive(false);
            _healthImageValue.transform.parent.gameObject.SetActive(false);
           // _movement.gameObject.SetActive(false);
           // _movementImageValue.gameObject.SetActive(false);
            _level.transform.parent.gameObject.SetActive(false);
            _damage.transform.parent.gameObject.SetActive(false);
            _range.transform.parent.gameObject.SetActive(false);

            if(obstacle is IDamageable)
            {
                _health.transform.parent.gameObject.SetActive(true);
                _healthImageValue.transform.parent.gameObject.SetActive(true);
                _threat.transform.parent.gameObject.SetActive(true);
            }
        }
    }
}
