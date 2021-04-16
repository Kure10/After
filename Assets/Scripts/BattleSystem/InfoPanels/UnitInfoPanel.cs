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

    public void UpdateStats(Unit unit)
    {
        _threat.text = unit._threat.ToString();

        UpdateHealthBar(unit.CurrentHealth, unit._maxHealth);

        UpdateRange(unit._range);

        _damage.text = unit._damage.ToString();

        // tmp
        var spriteLoader = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceSpriteLoader>();
        var sprite = spriteLoader.LoadUnitSprite(unit._imageName);
        _image.sprite = sprite;

    }

    public void UpdateHealthBar(int current, int max)
    {
        _health.text = $"{current} / {max}";

        float amount = (float)current / (float)max;

        healthImageValue.transform.localScale = new Vector3(amount, 1, 1);
    }

    public void UpdateRange(int range)
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

}
