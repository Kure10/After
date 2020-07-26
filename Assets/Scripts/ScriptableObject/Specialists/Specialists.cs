using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu (menuName = "ScriptableObject/Specialista", fileName = "Specialista") ]
public class Specialists : ScriptableObject
{
    /*  Povolani se nepouziva zatim.. Je to jenom string.. */
   // private enum Povolani { Doktor, Vojak, Vojín, Vedec, ZdravotniSestra, Seržant, JadernýFyzik, SpecNaz, Kuchař, Stavitel, Programator };

    [Header("Identification")]
    [SerializeField] private long identification = 0;
    [SerializeField] private Sprite sprite = null;
    [SerializeField] private string fullName = "Pavel";
    // [SerializeField] Povolani povolani;
    [SerializeField] private string story = "Byl jsme na ceste";
    [SerializeField] private string povolani = "Zahradnik";
    [SerializeField] private Color backgroundColor;
    [Header("Main attributes")]
    [Range(0, 20)] [SerializeField] private int level = 0;
    [Range(0,10)] [SerializeField] public int vojak = 0;
    [Range(0, 10)] [SerializeField] public int vedec = 0;
    [Range(0, 10)] [SerializeField] public int technik = 0;
    [Range(0, 10)] [SerializeField] public int social = 0;
    [SerializeField] private int karma = 0;

    [Header("Health and Stamina")]
    [SerializeField] public int maxStamina;
    [SerializeField] public int maxHP;
    [Space]
    [SerializeField] private int currentStamina = 1;
    [SerializeField] private int currentHP = 1;


    public string FullName { get { return fullName; } set {  this.fullName = value; } }
    public int Level { get { return level; } set { this.level = value; } }
    public int Mil { get { return vojak; } set { this.vojak = value; } }
    public int Scl { get { return vedec; } set { this.vedec = value; } }
    public int Tel { get { return technik; } set { this.technik = value; } }
    public int Sol { get { return social; } set { this.social = value; } }
    public int Kar { get { return karma; } set { this.karma = value; } }

    public string Povolani { get { return this.povolani; } set { this.povolani = value; } }
    public string Story { get { return this.story; } set { this.story = value; } }

    public long Id { get { return this.identification; } set { this.identification = value; } }

    public Sprite Sprite { get { return this.sprite; } set { this.sprite = value; } }

    public Color SpecialistColor { get { return this.backgroundColor; } set { this.backgroundColor = value; } }

    public float PercentHealth
    {
        get
        {
            return ( (float)currentHP / (float)maxHP ) * 100;
        }
    }

    public float PercentStamina
    {
        get
        {
            return ( (float)currentStamina / (float)maxStamina ) * 100;
        }
    }

    // obnova zdravy a staminy neni dodelana v GameDe Doc. zatim


    // tohle se jeste musi rozmyslet jestli to vubec bude tady...
    private int[] pointPerHour = { 5, 10, 20, 40, 80, 160, 320, 640, 1280, 2560 };

    private void Awake()
    {
        //povolani = Povolani.Doktor;
        CalcHealth(level,vojak,technik,vedec,social);
        CalcStamina(level);
    }

    private void CalcStamina(int lvl)
    {
        maxStamina = 57600 + 2400 * lvl;
    }

    private void CalcHealth(int lvl, int mil, int tel, int scl, int sol)
    {
        maxHP = 40 + lvl + 4 * mil + 2 * tel + scl + sol;
    }

    public Sprite GetSprite()
    {
        return sprite;
    }

}
