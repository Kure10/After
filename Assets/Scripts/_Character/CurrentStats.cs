using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentStats 
{
    public int level;

    public int military;

    public int social;

    public int tech;

    public int karma;

    public int science;

    public CurrentStats(int _level, int _military, int _social, int _tech , int _karma , int _science)
    {
        level = _level;
        military = _military;
        social = _social;
        tech = _tech;
        karma = _karma;
        science = _science;
    }
}
