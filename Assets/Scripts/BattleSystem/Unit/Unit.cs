using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Unit : MonoBehaviour
{
    public Animator animator;

    public struct PositionSquar
    {
        public int XPosition;
        public int YPosition;
    }

    public long _id = 0;
    public int _level = 0;
    public string _imageName = "Defaul";
    public Sprite _sprite = null;

    public PositionSquar CurrentPos;
    //

    public string _name = "Zombie";
    private int _maxHealth = 5;
    public int _military = 0;
    public int _threat = 0;
    public int _rangeMax = 0;
    public int _rangeMin = 0;

    private int _movement = 1;

    public int _iniciation = 0;

    private int _actionPoints = 0;
    private int _movementPoints = 0;

    public Unit.Team _team = Team.Human;

    [SerializeField] private GameObject activePanel;

    private Weapon _activeWeapon;
    private Weapon _firstWeapon;
    private Weapon _secondWeapon;

    private UnitWindow _unitWindow;
    private bool _isActive;
    private int _currentHealth = 1;

    private bool _isDead = false;

    public Weapon FirstWeapon { get { return this._firstWeapon; } }

    public Weapon SecondWeapon { get { return this._secondWeapon; } }

    public Weapon ActiveWeapon { get { return this._activeWeapon; } set { this._activeWeapon = value; } }

    public int ActionPoints { get { return this._actionPoints; } set { this._actionPoints = value; } }

    public int GetMaxMovement { get { return _movement; } }
    public int GetMovementPoints { get { return _movementPoints; } }


    public long Id { get { return this._id; } }
    public bool IsDead { get { return this._isDead; } }
    public int MaxHealth { get { return this._maxHealth; } }
    public bool IsActive
    {
        get { return this._isActive; }
        set
        {
            _isActive = value;
            activePanel.SetActive(_isActive);
        }
    }

    public int CurrentHealth
    {
        get { return this._currentHealth; }
        set
        {
            _currentHealth = value;

            if (_currentHealth < 0)
            {
                _currentHealth = 0;
            }
            else if (_currentHealth > _maxHealth)
            {
                _currentHealth = _maxHealth;
            }

            _unitWindow.UpdateHealthBar(_currentHealth, _maxHealth);
        }
    }

    public void UpdateData(Unit unit)
    {
        if (_unitWindow == null)
        {
            _unitWindow = GetComponent<UnitWindow>();
        }

        this._unitWindow.UpdateStats(unit);
    }

    public void DecreaseMovementPoints(int points)
    {
        _movementPoints -= points;
        if (_movementPoints <= 0)
        {
            _movementPoints = 0;
        }
    }

    public void RefreshMovementPoints ()
    {
        _movementPoints = _movement;
    }

    public void InitUnit(DataUnit dataUnit, Sprite sprite , Team tea)
    {
        _name = dataUnit.Name;
        _maxHealth = dataUnit.Health;
        _military = dataUnit.Damage;
        _threat = dataUnit.Threat;
        _rangeMax = dataUnit.RangeMax;
        _rangeMin = dataUnit.RangeMin;

        _movement = 5; //dataUnit.Movement;
        _movementPoints = _movement;

        CurrentPos.XPosition = dataUnit.StartXPosition;
        CurrentPos.YPosition = dataUnit.StartYPosition;

        _sprite = sprite;
        _id = dataUnit.Id;
        _level = dataUnit.Level;

        _iniciation = CalculateIniciation();

        _team = tea;

        _firstWeapon = dataUnit.firstWeapon;
        _secondWeapon = dataUnit.secondWeapon;

        _activeWeapon = _firstWeapon;

        // tmp
        _currentHealth = _maxHealth;
        //

        if (_unitWindow == null)
        {
            _unitWindow = GetComponent<UnitWindow>();
        }

        _unitWindow.UpdateStats(this);
        _unitWindow.UpdateHealthBar(_currentHealth, _maxHealth);
    }

    public void InitUnit(Unit unit)
    {
        _name = unit.name;
        _maxHealth = unit.MaxHealth;
        _military = unit._military;
        _threat = unit._threat;
        _rangeMax = unit._rangeMax;
        _rangeMin = unit._rangeMin;


        CurrentPos.XPosition = unit.CurrentPos.XPosition;
        CurrentPos.YPosition = unit.CurrentPos.YPosition;

        _imageName = unit._imageName;
        _sprite = unit._sprite;
        _id = unit._id;
        _level = unit._level;

        _iniciation = unit._iniciation;

        _team = unit._team;

        _activeWeapon = unit._activeWeapon;

        _currentHealth = unit.CurrentHealth;

        if (_unitWindow == null)
        {
            _unitWindow = GetComponent<UnitWindow>();
        }

        _unitWindow.UpdateStats(this);
        _unitWindow.UpdateHealthBar(_currentHealth, _maxHealth);
    }

    // threat x hod kostkou.
    public int CalculateIniciation ()
    {
        int result = 0;
 
        for (int i = 0; i < _threat; i++)
        {
            int number = UnityEngine.Random.Range(1, 7);

            if(number == 6)
                i--;
            
            result += number;
        }

        return result;
    }

    public bool CheckIfUnitIsNotDead()
    {
        bool isDead = false;
        if (_currentHealth <= 0)
        {
            _isDead = true;

            // animation
            // what ever call back///

            return isDead = _isDead;
        }

        return isDead;
    }

    public void SetNewCurrentPosition(int posX , int posY)
    {
        CurrentPos.XPosition = posX;
        CurrentPos.YPosition = posY;
    }

    // tmp pak poresit lepe
    public void UpdateAnim()
    {
        animator.SetBool("Active", _isActive);
    }

    // enums
    public enum Team
    {
        Human,
        Demon,
        Neutral,
    }
}


public class DataUnit 
{
    private long identification = 0;

    private string _imageName = "Defaul";  // todo musím se zamyslet jestli je potrebuji a nebo kde budu nahravat obrazek..
    private string _name = "Zombie";

    private Unit.PositionSquar StartPos;

    private int health = 5;
    private int damage = 0;
    private int threat = 0;
    private int rangeMax = 0;
    private int rangeMin = 0;
    private int _movement = 1;
    private int _level = 0;

    private List<Monster.Loot> _loot = new List<Monster.Loot>();

    public Weapon firstWeapon;
    public Weapon secondWeapon;

    public long Id { get { return this.identification; } }
    public string ImageName { get { return this._imageName; } }

    public string Name { get { return this._name; } }

    public int Health { get { return this.health; } }

    public int Damage { get { return this.damage; } }

    public int Threat { get { return this.threat; } }

    public int RangeMax { get { return this.rangeMax; } }

    public int RangeMin { get { return this.rangeMin; } }

    public int Movement { get { return this._movement; } }

    public int Level { get { return this._level; } }

    public int StartYPosition { get { return this.StartPos.YPosition; } }

    public int StartXPosition { get { return this.StartPos.XPosition; } }

    public List<Monster.Loot> GetLoot { get { return this._loot; } }

    public DataUnit()
    {

    }

    public DataUnit(Monster monster)
    {
        this._name = monster.Name;
        this.health = monster.Health;
        this.damage = monster.Military;
        this.threat = monster.Threat;
        this.rangeMax = monster.Range;
        this._movement = monster.BattleSpeed;
        this._imageName = monster.SpriteName;

        this._level = 1; // monster.level; Todo mozna se bude hodit.

        this.identification = monster.ID;

        this._loot.AddRange(monster._loot);
    }

    // Todo .....
    public DataUnit(Character character)
    {
        this._imageName = character.GetBlueprint().SpriteString;
        this._name = character.GetName();
        this.health = (int)character.LifeEnergy.CurrentLife;
        this.damage = character.Stats.military;
        this.threat = CalculateThreat(character.Stats);
        this.rangeMax = 1; // Todo
        this.rangeMin = 1; // todo  Range pro Fist souboj.
        this._movement = 1; // Todo pak bude nato nejaky vzorec
        this._level = character.Stats.level;

        this.identification = character.GetBlueprint().Id;

        firstWeapon = character.GetCharacterWeapon(false);
        secondWeapon = character.GetCharacterWeapon(true);
    }

    // Testing will be remove
    public DataUnit(int xPos , int yPos , int health, int damage, int threat, int range, int movement, string name, string imgName, int minRange , Weapon weapon = null , Weapon weapon2 = null)
    {
        this._name = name;
        this.health = health;
        this.damage = damage;
        this.threat = threat;
        this.rangeMax = range;
        this._movement = movement;

        this.rangeMin = minRange;

        this._imageName = imgName;

        firstWeapon = weapon;
        secondWeapon = weapon2;

        StartPos.XPosition = xPos;
        StartPos.YPosition = yPos;
    }

    public (int x , int y) SetRandomStartingPosition(List<(int x, int y)> freePosition)
    {
        int count = freePosition.Count;
        int randomPositionIndex = UnityEngine.Random.Range(0, count);

        (int x, int y) newPos = freePosition[randomPositionIndex];

        StartPos.XPosition = newPos.x;
        StartPos.YPosition = newPos.y;

        return newPos;
    }

    public int CalculateThreat (CurrentStats stats)
    {
        //  Patern -> 2+(0,5MiL+0,25TiL+0,15SvL+0,1SoL+Kar/30)↓ (min 2)
        float floatThreat = 2 + (0.5f * stats.military + 0.25f * stats.tech + 0.15f * stats.science + 0.1f * stats.social);
        int finalThreat = Mathf.FloorToInt(floatThreat);

        return finalThreat;
    }
}
