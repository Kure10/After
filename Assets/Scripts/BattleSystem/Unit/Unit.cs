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

    public void InitUnit(string name, int health, int dmg , int threat , int range, PositionSquar startPosition, int id, int movement, string imageName, Team tea)
    {
        _name = name;
        _maxHealth = health;
        _damage = dmg;
        _threat = threat;
        _range = range;

        _movement = movement;

        CurrentPos.XPosition = startPosition.XPosition;
        CurrentPos.YPosition = startPosition.YPosition;

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

        _iniciation = CalculateIniciation();

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
    public string imageName = "Defaul";

    public Unit.PositionSquar StartPos;

    public string _name = "Zombie";
    public int health = 5;
    public int damage = 0;
    public int threat = 0;
    public int range = 0;

    public int _movement = 1;


    // special abilities 


    private int currentHealth = 1;
}