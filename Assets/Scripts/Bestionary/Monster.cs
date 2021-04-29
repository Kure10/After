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

    private Sprite _sprite = null; // Todo doesnt have use

    public MonsterType _monsterType = MonsterType.Demon;

    public List<Loot> _loot = new List<Loot>();

    public List<Perk> _perk = new List<Perk>();

    public long ID { get { return _id; } }

    public Monster(long idNumber, string name, string description)
    {
        this._id = idNumber;
        this._name = name;
        this._description = description;
    }

    public Monster(long idNumber, string name, string description, int threat, int military, int health, decimal speed, int danger)
    {
        this._id = idNumber;
        this._name = name;
        this._description = description;

        this._threat = threat;
        this._military = military;
        this._health = health;
        this._speed = (float)speed;
        this._danger = danger;

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
