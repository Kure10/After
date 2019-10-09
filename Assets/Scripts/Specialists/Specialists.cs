using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu (menuName = "Specialista", fileName = "No name!") ]
public class Specialists : ScriptableObject
{
    private enum Povolani { Doktor, Vojak, Vojín, Vedec, ZdravotniSestra, Seržant, JadernýFyzik, SpecNaz, Kuchař, Stavitel, Programator };

    [Header("Identification")]
    [SerializeField] private Image image = null;
    [SerializeField] private string fullName = "Pavel";
    [SerializeField] Povolani povolani;
    [Header("Main attributes")]
    [Range(0, 20)] [SerializeField] public int level = 0;
    [Range(0,10)] [SerializeField] public int vojak = 0;
    [Range(0, 10)] [SerializeField] public int vedec = 0;
    [Range(0, 10)] [SerializeField] public int technik = 0;
    [Range(0, 10)] [SerializeField] public int social = 0;
    [SerializeField] private int karma = 0;

    [Header("Health and Stamina")]
    [SerializeField] public int maxStamina;
    [SerializeField] public int maxZdravy;
    [Space]
    [SerializeField] public int currentStamina = 1;
    [SerializeField] public int currentZdravy = 1;


    // obnova zdravy a staminy neni dodelana v GameDe Doc. zatim


    // tohle se jeste musi rozmyslet jestli to vubec bude tady...
    private int[] pointPerHour = { 5, 10, 20, 40, 80, 160, 320, 640, 1280, 2560 };

    private void Awake()
    {
        povolani = Povolani.Doktor;
        CalcHealth(level,vojak,technik,vedec,social);
        CalcStamina(level);
    }

    private void CalcStamina(int lvl)
    {
        maxStamina = 57600 + 2400 * lvl;
    }

    private void CalcHealth(int lvl, int mil, int tel, int scl, int sol)
    {
        maxZdravy = 40 + lvl + 4 * mil + 2 * tel + scl + sol;
    }

    
}
