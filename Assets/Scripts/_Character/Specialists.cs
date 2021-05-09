using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



[System.Serializable]
public class Specialists
{
    [Header("Identification")]
    [SerializeField] private long identification = 0;
    [SerializeField] private Sprite sprite = null;
    private string spriteString = null;
    [SerializeField] private string fullName = "Pavel";
    // [SerializeField] Povolani povolani;
    [SerializeField] private string story = "Byl jsme na ceste";
    [SerializeField] private string povolani = "Zahradnik";
    [SerializeField] private Color backgroundColor;
    [Header("Main attributes")]
    [Range(0, 20)] [SerializeField] private int level = 0;
    [Range(0, 10)] [SerializeField] private int vojak = 0;
    [Range(0, 10)] [SerializeField] private int vedec = 0;
    [Range(0, 10)] [SerializeField] private int technik = 0;
    [Range(0, 10)] [SerializeField] private int social = 0;
    [SerializeField] private int karma = 0;

    [Header("Additional information")]
    [SerializeField] private bool isDefault;
    [SerializeField] private string localization;

    public bool isPreSelectedOnMission = false;
    public bool isSelectedOnMission = false;
    private bool isOnMission = false;



    #region Properties

    public string FullName { get { return fullName; } set {  this.fullName = value; } }
    public int Level { get { return level; } set { this.level = value; } }
    public int Mil { get { return vojak; } set { this.vojak = value; } }
    public int Scl { get { return vedec; } set { this.vedec = value; } }
    public int Tel { get { return technik; } set { this.technik = value; } }
    public int Sol { get { return social; } set { this.social = value; } }
    public int Kar { get { return karma; } set { this.karma = value; } }

    public bool IsDefault { get { return this.isDefault; } set { this.isDefault = value; } }

    public string Localization { get { return this.localization; } set { this.localization = value; } }

    public string Povolani { get { return this.povolani; } set { this.povolani = value; } }
    public string Story { get { return this.story; } set { this.story = value; } }

    public long Id { get { return this.identification; } set { this.identification = value; } }

    public Sprite Sprite { get { return this.sprite; } set { this.sprite = value; } }

    public string SpriteString { get { return this.spriteString; } set { this.spriteString = value; } }

    public Color SpecialistColor { get { return this.backgroundColor; } set { this.backgroundColor = value; } }

    public bool IsOnMission
    {
        get 
        {
            return this.isOnMission; 
        }
        set 
        {
            if (value)
            {
                this.isPreSelectedOnMission = false;
                this.isSelectedOnMission = false;
                this.isOnMission = value;
            }
            else
            {
                this.isOnMission = value;
            }
        } 
    }

    #endregion

    #region Methods

    public void SetColor(int Red, int Green, int Blue)
    {
        this.backgroundColor.r = Red;
        this.backgroundColor.g = Green;
        this.backgroundColor.b = Blue;
        this.backgroundColor.a = 255;
    }

    #endregion
}
