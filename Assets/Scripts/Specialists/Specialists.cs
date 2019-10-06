using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu (menuName = "Specialista", fileName = "No name!") ]
public class Specialists : ScriptableObject
{
    private enum Povolani { Doktor, Vojak, Vojín, Vedec, ZdravotniSestra, Seržant, };


    [SerializeField] private Image image = null;
    [SerializeField] private string fullName = "Pavel";
    [SerializeField] Povolani povolani;
    // [SerializeField] private string povolani = "No Spec Tady by mel byt asi enum jeste uvidim";

    [Range(0, 20)] [SerializeField] private int level = 0;
    [Range(0,10)] [SerializeField]  private int vojak = 0;
    [Range(0, 10)] [SerializeField] private int vedec = 0;
    [Range(0, 10)] [SerializeField] private int technik = 0;
    [Range(0, 10)] [SerializeField] private int social = 0;
    [SerializeField] private int karma = 0;

    // tohle se jeste musi rozmyslet jestli to vubec bude tady...
    private int[] pointPerHour = { 5, 10, 20, 40, 80, 160, 320, 640, 1280, 2560 };

    private void Awake()
    {
        povolani = Povolani.Doktor;
    }
}
