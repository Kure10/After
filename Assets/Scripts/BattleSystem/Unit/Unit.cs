using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Unit : MonoBehaviour
{
    public Animator animator;
    
    public struct PositionSquar
    {
        public int XPosition;
        public int YPosition;
    }

    public int _id = 0;
    public string _imageName = "Defaul";
    public Sprite _sprite = null;

    public PositionSquar CurrentPos;
    //

    public string _name = "Zombie";
    private int _maxHealth = 5;
    public int _damage = 0;
    public int _threat = 0;
    public int _rangeMax = 0;
    public int _rangeMin = 0;

    public int _movement = 1;

    public int _iniciation = 0;

    public Unit.Team _team = Team.Human;

    [SerializeField] private GameObject activePanel;

    private UnitWindow _unitWindow;
    private bool _isActive;
    private int _currentHealth = 1;

    private bool _isDead = false;

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
        if(_unitWindow == null)
        {
            _unitWindow = GetComponent<UnitWindow>();
        }

        this._unitWindow.UpdateStats(unit);
    }

    public void InitUnit(string name, int health, int dmg , int threat , int range, int YPosition, int XPosition, int id, int movement, Sprite sprite, Team tea, int rangeMin)
    {
        _name = name;
        _maxHealth = health;
        _damage = dmg;
        _threat = threat;
        _rangeMax = range;
        _rangeMin = rangeMin;

        _movement = movement;

        CurrentPos.XPosition = XPosition;
        CurrentPos.YPosition = YPosition;

        _sprite = sprite;
        _id = id;

        _iniciation = CalculateIniciation();

        _team = tea;

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
        _damage = unit._damage;
        _threat = unit._threat;
        _rangeMax = unit._rangeMax;
        _rangeMin = unit._rangeMin;


        CurrentPos.XPosition = unit.CurrentPos.XPosition;
        CurrentPos.YPosition = unit.CurrentPos.YPosition;

        _imageName = unit._imageName;
        _sprite = unit._sprite;
        _id = unit._id;

        _iniciation = unit._iniciation;

        _team = unit._team;

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
            int number = Random.Range(1, 7);

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
    private string _imageName = "Defaul";  // todo musím se zamyslet jestli je potrebuji a nebo kde budu nahravat obrazek..
    private string _name = "Zombie";

    private Unit.PositionSquar StartPos;

    private int health = 5;
    private int damage = 0;
    private int threat = 0;
    private int rangeMax = 0;
    private int rangeMin = 0;
    private int _movement = 1;

    public string ImageName { get { return this._imageName; } }

    public string Name { get { return this._name; } }

    public int Health { get { return this.health; } }

    public int Damage { get { return this.damage; } }

    public int Threat { get { return this.threat; } }

    public int RangeMax { get { return this.rangeMax; } }

    public int RangeMin { get { return this.rangeMin; } }

    public int Movement { get { return this._movement; } }

    public int StartYPosition { get { return this.StartPos.YPosition; } }

    public int StartXPosition { get { return this.StartPos.XPosition; } }

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
        this._movement = 1; // Todo pak bude nato nejaky vzorec
    }

    // Testing will be remove
    public DataUnit(int xPos , int yPos , int health, int damage, int threat, int range, int movement, string name, string imgName, int minRange)
    {
        this._name = name;
        this.health = health;
        this.damage = damage;
        this.threat = threat;
        this.rangeMax = range;
        this._movement = movement;

        this.rangeMin = minRange;

        this._imageName = imgName;

        StartPos.XPosition = xPos;
        StartPos.YPosition = yPos;
    }

    public (int x , int y) SetRandomStartingPosition(List<(int x, int y)> freePosition)
    {
        int count = freePosition.Count;
        int randomPositionIndex = Random.Range(0, count);

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