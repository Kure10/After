using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Unit : MonoBehaviour
{
    public Animator animator;
    public UnitWindow _unitWindow;

    public struct PositionSquar
    {
        public int XPosition;
        public int YPosition;
    }

    public int _id = 0;
    public string _imageName = "Defaul";

    public PositionSquar CurrentPos;
    //

    public string _name = "Zombie";
    private int _maxHealth = 5;
    public int _damage = 0;
    public int _threat = 0;
    public int _range = 0;

    public int _movement = 1;

    public int _iniciation = 0;

    public Unit.Team _team = Team.Human;

    [SerializeField] private GameObject activePanel;

    private bool _isActive;

    private int _currentHealth = 1;

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

            if(_currentHealth < 0)
            {
                _currentHealth = 0;
            }
            else if (_currentHealth > _maxHealth)
            {
                _currentHealth = _maxHealth;
            }

            _unitWindow.UpdateHealthBar(_currentHealth,_maxHealth);
        }
    }

    public void InitUnit(string name, int health, int dmg , int threat , int range, int YPosition, int XPosition, int id, int movement, string imageName, Team tea)
    {
        _name = name;
        _maxHealth = health;
        _damage = dmg;
        _threat = threat;
        _range = range;

        _movement = movement;

        CurrentPos.XPosition = XPosition;
        CurrentPos.YPosition = YPosition;

        _imageName = imageName;
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
        _maxHealth = unit._maxHealth;
        _damage = unit._damage;
        _threat = unit._threat;
        _range = unit._range;

        CurrentPos.XPosition = unit.CurrentPos.XPosition;
        CurrentPos.YPosition = unit.CurrentPos.YPosition;

        _imageName = unit._imageName;
        _id = unit._id;

        _iniciation = unit._iniciation;

        _team = unit._team;

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
            // unit is dead
            // animation
            // what ever call back///

            return isDead = true;
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
    private string imageName = "Defaul";  // todo musím se zamyslet jestli je potrebuji a nebo kde budu nahravat obrazek..
    private string _name = "Zombie";

    private Unit.PositionSquar StartPos;

    private int health = 5;
    private int damage = 0;
    private int threat = 0;
    private int range = 0;
    private int _movement = 1;

    public string ImageName { get { return this.imageName; } }

    public string Name { get { return this._name; } }

    public int Health { get { return this.health; } }

    public int Damage { get { return this.damage; } }

    public int Threat { get { return this.threat; } }

    public int Range { get { return this.range; } }

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
        this.range = monster.Range;
        this._movement = monster.BattleSpeed;
    }

    // Todo .....
    public DataUnit(Character character)
    {
        this._name = character.GetName();
        this.health = (int)character.LifeEnergy.CurrentLife;
        this.damage = character.Stats.military;
        this.threat = 2; // Todo
        this.range = 1; // Todo
        this._movement = 1; // Todo
    }

    // Testing will be remove
    public DataUnit(int xPos , int yPos , int health, int damage, int threat, int range, int movement, string name, string imgName)
    {
        this._name = name;
        this.health = health;
        this.damage = damage;
        this.threat = threat;
        this.range = range;
        this._movement = movement;

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
}