using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Unit : MonoBehaviour
{
    public Animator animator;

    public struct PositionSquar
    {
        public int XPosition;
        public int YPosition;
    }

    public int _id = 0;
    public string imageName = "Defaul";

    public PositionSquar CurrentPos;
    //

    public string _name = "Zombie";
    public int _health = 5;
    public int _damage = 0;
    public int _threat = 0;
    public int _range = 0;

    public int _movement = 1;

    [SerializeField] private GameObject activePanel;

    private bool _isActive;



    public bool IsActive
    { 
        get { return this._isActive; }
        set 
        { 
            _isActive = value;
            activePanel.SetActive(_isActive);
        } 
    }

    // special abilities 


    private int currentHealth = 1;

    public void InitUnit(string name, int health, int dmg , int threat , int range, PositionSquar startPosition, int id, int movement)
    {
        _name = name;
        _health = health;
        _damage = dmg;
        _threat = threat;
        _range = range;

        _movement = movement;

        CurrentPos.XPosition = startPosition.XPosition;
        CurrentPos.YPosition = startPosition.YPosition;

        _id = id;
    }

    public void InitUnit(Unit unit)
    {
        _name = unit.name;
        _health = unit._health;
        _damage = unit._damage;
        _threat = unit._threat;
        _range = unit._range;

        CurrentPos.XPosition = unit.CurrentPos.XPosition;
        CurrentPos.YPosition = unit.CurrentPos.YPosition;

        _id = unit._id;
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


}



public class DataUnit 
{
    //public struct PositionSquar
    //{
    //    public int XPosition;
    //    public int YPosition;
    //}

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