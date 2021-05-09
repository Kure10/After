using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster
{

    private long _id = 0;
    private string _name = string.Empty;
    private string _description = string.Empty;

    private int _threat = 0;
    private int _military = 0;
    private int _health = 0;
    private float _speed = 0;
    private int _danger = 0;
    private int _battleSpeed = 0;
    private int _range = 0;

    private string _spriteName = null;

    public MonsterType _monsterType = MonsterType.Demon;

    public List<Loot> _loot = new List<Loot>();

    public List<Perk> _perk = new List<Perk>();

    public long ID { get { return _id; } }

    public string Name { get { return this._name; } }
    public string Description { get { return this._description; } }
    public int Threat { get { return this._threat; } }
    public int Military { get { return this._military; } }
    public int Range { get { return this._range; } }
    public int Health { get { return this._health; } }
    public float Speed { get { return this._speed; } }
    public int Danger { get { return this._danger; } }
    public int BattleSpeed { get { return this._battleSpeed; } }
    public string SpriteName { get { return this._spriteName; } }

    public Monster(long idNumber, string name, string description)
    {
        this._id = idNumber;
        this._name = name;
        this._description = description;
    }

    public Monster(long idNumber, string name, string description, int threat, int military, int health, decimal speed, int danger, int range, string spriteName)
    {
        this._id = idNumber;
        this._name = name;
        this._description = description;

        this._threat = threat;
        this._military = military;
        this._health = health;
        this._speed = (float)speed;
        this._danger = danger;
        this._range = range;
        this._spriteName = spriteName;

        float value = (float)speed + 1;
        _battleSpeed = Mathf.RoundToInt(value);  
    }

    public struct Loot
    {
        public long lootID;
        public int dropChange;
        public int itemCountMin;
        public int itemCountMax;
    }

    public struct Perk
    {
        public PerkType perks;
        public int value;
    }

    public enum PerkType
    {
        FirstStrike,
        Reload,
        SlowReflex,
        Dumb
    }

    public enum MonsterType
    {
        Obsessed,
        Demon,
        Human,
    }
}
