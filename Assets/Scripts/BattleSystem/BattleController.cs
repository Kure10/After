using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

using UnityEngine.Events;

public class BattleController : MonoBehaviour
{
    [Header("Testing")]
    public bool TestingBattle = false;

    [Header("Main")]
    [SerializeField] InventoryManager _inventoryManager;
    [SerializeField] BattleGridController _battleGridController;

    [Header("Info Panels")]
    [SerializeField] private BattleInfoPanel _battleInfoPanel;
    [SerializeField] private UnitInfoPanel _leftUnitInfo;
    [SerializeField] private UnitInfoPanel _rightUnitInfo;
    [SerializeField] private BattleLogPanel _battleLog;
    [SerializeField] private BattleResultPopup _battleResultPopup;

    [Header("Info Popup")]
    [SerializeField] private DetailUnitPopup detailPopup;

    [Space]
    [Header("Others")]

    public List<Unit> _unitsOnBattleField = new List<Unit>();

    [Space]
    public GameObject _unit;

    public static event Action<StatsClass> OnBattleLost = delegate { };
    public static event Action OnBattleEnd = delegate { };

    bool _turnIsOver = false;
    private bool _isBattleOnline = false;
    private bool _isPlayerTurn = false;

    private Unit _activeUnit = null;

    private int _order = 0;
    private int roundCount = 1;

    private List<Squar> _squaresInUnitMoveRange = new List<Squar>();
    private List<Squar> _squaresInUnitAttackRange = new List<Squar>();

    private ResourceSpriteLoader spriteLoader = null;
    private BattleStartData _battleStartData;

    private void Awake()
    {
        _battleResultPopup.InicializedControlles( ()=> CloseBattle ());
    }
    private void Start()
    {
        // This is for testing purpose only when u are in BATTLEGROUND scene!!
        if(TestingBattle)
        {
            spriteLoader = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceSpriteLoader>();
            _battleStartData = InitTestBattleData();

            _battleGridController.CreateBattleField(_battleStartData);

           // CreateBattleField(_battleStartData.Rows, _battleStartData.Collumn);

            InitBattle(_battleStartData);
            TestStartBattle();
        }
    }

    private void Update()
    {
        if (_isBattleOnline)
        {
            InputProcess();

            if (_isPlayerTurn)
            {
                if (DetectPlayerInputs())
                {
                    _turnIsOver = true;
                    _isPlayerTurn = false;
                }
            }
            else
            {
                // OnSimpleAIMove();

                if (DetectPlayerInputs())
                {
                    _turnIsOver = true;
                    _isPlayerTurn = true;
                }

                // AI is on the move 
                // start corotine
                // switch to player turn
            }

            if (_turnIsOver)
            {
                
                SetSquaresOutOfAttackReach();
                SetSquaresOutOfMoveRange();
                ShowSquaresWithinRange(false);

                 _order++;

                int countAliveUnit = 0;
                foreach (Unit unit in _unitsOnBattleField)
                {
                    if(!unit.IsDead)
                    {
                        countAliveUnit++;
                    }
                }

                // Next round so calc new iniciative
                if (_order >= countAliveUnit)
                {
                    _order = 0;
                    roundCount++;
                    _battleLog.AddBattleLog($"<---------- Turn {roundCount} ---------->");

                   // new iniciative
                    foreach (Unit unit in _unitsOnBattleField)
                    {
                        if(!unit.IsDead)
                        {
                            unit._iniciation = unit.CalculateIniciation();
                        }
                    }

                    SortUnitAccordingIniciation();
                    _battleInfoPanel.UpdateUnitNewTurnOrder(_unitsOnBattleField, _unit);
                }
 
                UpdateActiveUnit();

                _squaresInUnitMoveRange.AddRange(_battleGridController.FindSquaresInUnitMoveRange(_activeUnit));

                ShowSquaresWithinRange(true);

                _squaresInUnitAttackRange.AddRange(_battleGridController.FindSquaresInUnitAttackRange(_activeUnit));

                ShowSquaresWithingAttackRange();

                _leftUnitInfo.UpdateStats(_activeUnit);

                _turnIsOver = false;

                VictoryConditionCheck();
            }
        }
    }

    // for buttons
    public void CloseBattle()
    {
        OnBattleEnd.Invoke();
        this.gameObject.SetActive(false);
        _battleResultPopup.OnPressExit();
    }

    // for buttons
    public void SkipUnitTurn()
    {
        _turnIsOver = true;
        _battleLog.AddBattleLog($"{_activeUnit._name} skip round");
    }

    private void UpdateActiveUnit()
    {
        _battleInfoPanel.UpdateUnitOrder(_activeUnit, false);
        _activeUnit.IsActive = false;
        _activeUnit.UpdateAnim();

        for (int i = _order; i < _unitsOnBattleField.Count; i++)
        {
            _activeUnit = _unitsOnBattleField[i];

            if (!_activeUnit.IsDead)
                break;
        }

        _activeUnit.IsActive = true;
        _activeUnit.UpdateAnim();
        _battleInfoPanel.UpdateUnitOrder(_activeUnit, true);

        _battleLog.AddBattleLog($"{_activeUnit._name} has turn");
    }

    private bool DetectPlayerInputs()
    {
        bool result = false;
        BattleAction action = BattleAction.None;
        Squar actionOnSquare = null;
        Unit unitOnSquare = null;

        if (Input.GetMouseButtonDown(0))
        {
            actionOnSquare = RaycastTargetSquar();
            if (actionOnSquare != null)
            {
                action = OnClickIntoGrid(actionOnSquare);
                unitOnSquare = actionOnSquare.unitInSquar;
                result = true;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            actionOnSquare = RaycastTargetSquar();
            if (actionOnSquare != null)
            {
                if (actionOnSquare.unitInSquar != null)
                {
                    detailPopup.ShowPopup(actionOnSquare.unitInSquar);
                }

                Debug.Log("Info unit panel popup");
            }
        }

        if (result)
        {
            switch (action)
            {
                case BattleAction.None:
                    Debug.Log("None Action");
                    result = false;
                    break;
                case BattleAction.Move:
                    _battleGridController.MoveToSquar(_activeUnit, actionOnSquare);

                    //battleLog.AddLog($"{_activeUnit._name} moved to square {squarToMove.xCoordinate} / {squarToMove.yCoordinate}");

                    _battleLog.AddBattleLog($"{_activeUnit._name} moved");
                    result = true;
                    break;
                case BattleAction.Attack:

                    BattleGridController.AttackInfo attackInfo = null;
                    attackInfo = _battleGridController.AttackToUnit(_activeUnit, unitOnSquare);

                    if (attackInfo.unitDied)
                    {
                        _battleGridController.DestroyUnitFromBattleField(unitOnSquare);
                        _battleInfoPanel.DeleteUnitFromOrder(unitOnSquare);
                        _battleLog.AddBattleLog($"{unitOnSquare._name} is dead");
                    }

                    _battleLog.AddAttackBattleLog(attackInfo, _activeUnit, unitOnSquare);
                    _battleInfoPanel.UpdateUnitData(unitOnSquare);
                    result = true;
                    break;
                case BattleAction.Heal:
                    Debug.Log("Heal action");
                    result = false;
                    break;
                default:
                    break;
            }
        }

        return result;
    }


    public void ChangeWeapon()
    {
        bool changeFirst = false;
        if (_activeUnit.ActiveWeapon != _activeUnit.FirstWeapon)
        {
            changeFirst = true;
        }
        else
        {
            changeFirst = false;
        }

        if (changeFirst)
        {
            _activeUnit.ActiveWeapon = _activeUnit.FirstWeapon;
        }
        else
        {
            _activeUnit.ActiveWeapon = _activeUnit.SecondWeapon;
        }

      
        SetSquaresOutOfAttackReach();
        _squaresInUnitAttackRange.AddRange(_battleGridController.FindSquaresInUnitAttackRange(_activeUnit));
        ShowSquaresWithingAttackRange();

        _activeUnit.UpdateData(_activeUnit);
        _leftUnitInfo.UpdateStats(_activeUnit);
        _battleInfoPanel.UpdateUnitData(_activeUnit);
    }

    // for buttons
    public void ChangeWeapon(bool changeFirst)
    {
        if (changeFirst)
        {
            if (_activeUnit.ActiveWeapon == _activeUnit.FirstWeapon)
                return;

            _activeUnit.ActiveWeapon = _activeUnit.FirstWeapon;
        }
        else
        {
            if (_activeUnit.ActiveWeapon == _activeUnit.SecondWeapon)
                return;

            _activeUnit.ActiveWeapon = _activeUnit.SecondWeapon;
        }

        SetSquaresOutOfAttackReach();
        _squaresInUnitAttackRange.AddRange(_battleGridController.FindSquaresInUnitAttackRange(_activeUnit));
        ShowSquaresWithingAttackRange();

        _activeUnit.UpdateData(_activeUnit);
        _leftUnitInfo.UpdateStats(_activeUnit);
        _battleInfoPanel.UpdateUnitData(_activeUnit);
    }

    public void StartBattle(BattleStartData battleStartData)
    {
        _battleStartData = battleStartData;

        spriteLoader = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceSpriteLoader>();

        // Setup Before Battle Start
        SetupForNewBattle();
        _battleResultPopup.InitBeforeBattleStart();
        
        // this is for testing purpose .. Right now is not decided how we will set battlefield Size.
        // minumum is 6 and 10 
        _battleStartData.Rows = 8;
        _battleStartData.Collumn = 16;

        _battleGridController.CreateBattleField(_battleStartData);

        SetUnitPosition(_battleStartData);
        InitBattle(_battleStartData);
        TestStartBattle();
    }

    private void InitBattle(BattleStartData battleData)
    {
        _battleLog.AddBattleLog("Battle Start");

        this.gameObject.SetActive(true);
        _isPlayerTurn = true; // todo for now is not decided who will turn first .. ? ?? Now player...

        _order = 0;
        int amountEnemies = battleData.enemyData.enemieUnits.Count;
        int amountPlayers = battleData.playerData.playerUnits.Count;


        for (int i = 0; i < amountEnemies; i++)
        {
            DataUnit dataUnit = battleData.enemyData.enemieUnits[i];
            Squar squar = _battleGridController.GetSquareFromGrid(dataUnit.StartXPosition, dataUnit.StartYPosition); // tady musí byt chech jestli nejsem mimo pole

            GameObject unt = Instantiate(_unit, squar.container.transform);
            Unit newUnit = unt.GetComponent<Unit>();

            var sprite = spriteLoader.LoadUnitSprite(dataUnit.ImageName);

            newUnit.InitUnit(dataUnit, sprite, Unit.Team.Demon);

            squar.unitInSquar = newUnit;
            _unitsOnBattleField.Add(newUnit); 
        }

        for (int i = 0; i < amountPlayers; i++)
        {
            DataUnit dataUnit = battleData.playerData.playerUnits[i];
            Squar squar = _battleGridController.GetSquareFromGrid(dataUnit.StartXPosition, dataUnit.StartYPosition);  // tady musí byt chech jestli nejsem mimo pole

            GameObject unit1 = Instantiate(_unit, squar.container.transform);
            Unit newUnit = unit1.GetComponent<Unit>();

            var sprite = spriteLoader.LoadSpecialistSprite(dataUnit.ImageName);

            newUnit.InitUnit(dataUnit, sprite, Unit.Team.Human);

            squar.unitInSquar = newUnit;
            _unitsOnBattleField.Add(newUnit);
        }

        // Sort unit order.
        SortUnitAccordingIniciation();
        _battleInfoPanel.InitStartOrder(_unitsOnBattleField, _unit);

        // init Event
        foreach (Squar square in _battleGridController.GetSquarsFromBattleField)
        {
            square.InitEvent(delegate (Squar squ)
            {
                SetCursor(squ);
                UpdateRightPanel(squ);
            });
        }

    }

    private void SetUnitPosition(BattleStartData battleStartData)
    {

        SetPlayersUnitPosition(battleStartData);

        if (battleStartData.isRandomEnemyPosition)
        {
            SetEnemyRandomPosition(battleStartData);
        }
        else
        {
            // TODo
            SetEnemyRandomPosition(battleStartData); // třeba když je ambush tak by se měli nastavit specialni pozice atd..
        }
    }

    private void SetPlayersUnitPosition (BattleStartData battleStartData)
    {
        List<(int x, int y)> posiblePositions = new List<(int x , int y)>();

        int startSeachXPosition = 0;
        int radius = 3; // Hard coded not good..

        if (battleStartData.Rows % 2 == 0)
        {
            startSeachXPosition = (battleStartData.Rows -1 )/ 2 - 1;
        }
        else
        {
            startSeachXPosition = (battleStartData.Rows - 1) / 2 ;
        }

        for (int i = startSeachXPosition; i < startSeachXPosition + radius; i++)
        {
            (int x, int y) position = (0,0);

            for (int j = 0; j < radius; j++)
            {
                position = (i, j);
                posiblePositions.Add(position);
            }
        }

        // todo
        if (battleStartData.playerData.playerUnits.Count > posiblePositions.Count)
        {
            // max position is 9
            Debug.LogError("Number of Players is bigger than posiblePositions to put unit on BattleField Critical Error");
        }

        // set randomPosition from list of available positions
        foreach (DataUnit dataUnit in battleStartData.playerData.playerUnits)
        {
            (int x, int y) occupiedPosition = dataUnit.SetRandomStartingPosition(posiblePositions);

            posiblePositions.Remove(occupiedPosition);
        }
    }

    private void SetEnemyRandomPosition (BattleStartData battleStartData)
    {
        List<(int x, int y)> posiblePositions = new List<(int x, int y)>();

        int startSeachYPosition = 0;
        int radius = 3; // Hard coded not good..
        int minDistanceFromPlayer = 5; // Hard coded not good..

        foreach (DataUnit dataUnit in battleStartData.playerData.playerUnits)
        {
            if(dataUnit.StartYPosition > startSeachYPosition)
            {
                startSeachYPosition = dataUnit.StartYPosition;
            }
        }

        startSeachYPosition += minDistanceFromPlayer;

        for (int i = 0; i < battleStartData.Rows -1; i++)
        {
            (int x, int y) position = (0, 0);

            for (int j = startSeachYPosition; j < battleStartData.Collumn -1; j++)
            {
                position = (i, j);
                posiblePositions.Add(position);
            }
        }

        // todo
        if (battleStartData.enemyData.enemieUnits.Count > posiblePositions.Count)
        {
            Debug.LogError("Number of Enemis is bigger than posiblePositions to put unit on BattleField Critical Error");
        }

        foreach (DataUnit dataUnit in battleStartData.enemyData.enemieUnits)
        {
            (int x, int y) occupiedPosition = dataUnit.SetRandomStartingPosition(posiblePositions);

            posiblePositions.Remove(occupiedPosition);
        }
    }

    private BattleStartData InitTestBattleData()
    {
        BattleStartData battleStartData = new BattleStartData();

        battleStartData.Rows = 8;
        battleStartData.Collumn = 16;

        Weapon weapon = new Weapon(3,1,5,6);
        Weapon weapon2 = new Weapon(4, 2, 3, 4);

        DataUnit newA = new DataUnit(2,3,10,5,3,3,2, "Player1", "Gargoyle", 2, weapon, weapon2);
        DataUnit newB = new DataUnit(4, 5, 5, 5, 7, 5, 2, "Player2", "Gargoyle", 2, weapon, weapon2);
        DataUnit newC = new DataUnit(3, 0, 6, 2, 2, 1, 2, "Zombie1", "Zombie 1", 1);
        DataUnit newx = new DataUnit(2, 7, 8, 2, 1, 2, 1, "Zombie2", "Zombie 2", 0);
        DataUnit newy = new DataUnit(5, 10, 6, 1, 3, 1, 1, "Zombie3", "Zombie 1", 0);

        battleStartData.playerData.playerUnits.Add(newA);
        battleStartData.playerData.playerUnits.Add(newB);
        battleStartData.enemyData.enemieUnits.Add(newC);
        //battleStartData.enemyData.enemieUnits.Add(newx);
        //battleStartData.enemyData.enemieUnits.Add(newy);

        return battleStartData;
    }

    private void TestStartBattle()
    {
        _isBattleOnline = true;

        if (_unitsOnBattleField.Count > 0)
        {
            _activeUnit = _unitsOnBattleField[_order];
            _activeUnit.IsActive = true;
            _activeUnit.UpdateAnim();
            _battleInfoPanel.UpdateUnitOrder(_activeUnit, true);

            _squaresInUnitMoveRange.AddRange(_battleGridController.FindSquaresInUnitMoveRange(_activeUnit));
            ShowSquaresWithinRange(true);

            _squaresInUnitAttackRange.AddRange(_battleGridController.FindSquaresInUnitAttackRange(_activeUnit));
            ShowSquaresWithingAttackRange();

            _leftUnitInfo.UpdateStats(_activeUnit);
        }
        else
        {
            Debug.LogError("No units");
        }
    }

    private Squar RaycastTargetSquar()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultsList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultsList);

        for (int i = 0; i < raycastResultsList.Count; i++)
        {
            var sq = raycastResultsList[i].gameObject.GetComponent<Squar>();

            if (sq != null)
            {
                return sq;
            }

        }

        return null;
    }

    // this is TMP method

    //private void OnSimpleAIMove()
    //{
    //    Squar squarToMove = FindSquarForAI();
    //    MoveToSquar(squarToMove);
    //}

    //private Squar FindSquarForAI()
    //{
    //    int rowsCount = _rowsCount - 1;

    //    while (true)
    //    {
    //        int xCor = UnityEngine.Random.Range(0, rowsCount);
    //        int yCor = UnityEngine.Random.Range(0, _collumCount);

    //        if (_squaresInBattleField[xCor, yCor].unitInSquar == null)
    //        {
    //            return _squaresInBattleField[xCor, yCor];
    //        }
    //    }
    //}

    private BattleAction OnClickIntoGrid(Squar squarToMove)
    {
        BattleAction action = BattleAction.None;

        if (squarToMove.unitInSquar != null)
        {
            bool isFriendlyUnit = squarToMove.unitInSquar._team == _activeUnit._team;

            if(!isFriendlyUnit && _squaresInUnitAttackRange.Contains(squarToMove))
            {
                action = BattleAction.Attack;
            }

            if (isFriendlyUnit && _squaresInUnitAttackRange.Contains(squarToMove))
            {
                action = BattleAction.Heal;
            }

            return action;
        }
        else
        {
            if (_squaresInUnitMoveRange.Contains(squarToMove))
            {
                action = BattleAction.Move;
            }

            return action;
        }
    }


    private bool VictoryConditionCheck ()
    {
        bool battleIsOver = false;
        int humanUnit = 0;
        int demonUnit = 0;
        int neutralUnit = 0;

        foreach (var unit in _unitsOnBattleField)
        {
            if(!unit.IsDead)
            {
                if (unit._team == Unit.Team.Human)
                {
                    humanUnit++;
                }
                else if (unit._team == Unit.Team.Demon)
                {
                    demonUnit++;
                }
                else if (unit._team == Unit.Team.Demon)
                {
                    neutralUnit++;
                }
            }
        }

        if (demonUnit == 0 && neutralUnit == 0)
        {
            HumanVictory();
            battleIsOver = true;
        }

        if (humanUnit == 0 && neutralUnit == 0)
        {
            DemonVictory();
            battleIsOver = true;
        }

        if (demonUnit == 0 && humanUnit == 0)
        {
            NeutralVictory();
            battleIsOver = true;
        }

        if (battleIsOver)
        {
            _battleResultPopup.ShowBattleResult(_unitsOnBattleField, _unit, _battleStartData.GetCharacterFromBattle);
            _isBattleOnline = false;
            // Todo Clean BattleField . And prepair for next Battle
        }
        
        return false;
    }

    private StatsClass CreateStatClassBattleResult (bool positiveResult)
    {
        StatsClass statClass = new StatsClass();

        if(positiveResult)
        {
            statClass.AddStat("$T", _battleStartData.WinEvaluation.statClassNumber);
        }
        else
        {
            statClass.AddStat("$T", 5);
            statClass.AddStat("BattleLost", 1);

        }
        statClass.AddStat("Mission", _battleStartData.WinEvaluation.mission);
        statClass.AddStat("BattleResult", positiveResult);
   
        return statClass;
    }

    private void ShowSquaresWithinRange(bool makeVisible)
    {
        foreach (Squar sq in _squaresInUnitMoveRange)
        {
            if (sq.unitInSquar == null)
            {
                sq.inRangeBackground.SetActive(makeVisible);
            }
        }
    }

    private void ShowSquaresWithingAttackRange()
    {
        // Todo Predelat metodu GetTheAdjacentAttackSquare  -> rozdelit
        foreach (Squar squ in _squaresInUnitAttackRange)
        {
            _battleGridController.GetTheAdjacentAttackSquare(squ, true);
        }
    }

    public void HumanVictory()
    {
        _battleStartData.UpdateMainPlayerData(_unitsOnBattleField);

        List<ItemBlueprint> itemsLoot = new List<ItemBlueprint>();
        foreach (DataUnit dataUnit in _battleStartData.enemyData.enemieUnits)
        {
            foreach (Monster.Loot loot in dataUnit.GetLoot)
            {
                int randomNumber = UnityEngine.Random.Range(0, 101); // Todo Lepsi random.
                int itemCount = UnityEngine.Random.Range(loot.itemCountMin, loot.itemCountMax + 1); // Todo Lepsi random.
                if (randomNumber <= loot.dropChange)
                {
                    for (int i = 0; i < itemCount; i++)
                    {
                        itemsLoot.Add(_inventoryManager.GetResourcesByID(loot.lootID));
                    }
                }
            }
        }
        _battleResultPopup.InicializedStartInventory(itemsLoot);

        StatsClass statClass = CreateStatClassBattleResult(true);
        OnBattleLost.Invoke(statClass);
    }

    public void DemonVictory()
    {
        StatsClass statClass = CreateStatClassBattleResult(false);
        OnBattleLost.Invoke(statClass);
    }
    public void NeutralVictory()
    {
        StatsClass statClass = CreateStatClassBattleResult(false);
        OnBattleLost.Invoke(statClass);
    }
    
    private void SetSquaresOutOfMoveRange()
    {
        foreach (Squar squar in _squaresInUnitMoveRange)
        {
            squar.inRangeBackground.SetActive(false);
            squar.isInMoveRange = false;
        }
        _squaresInUnitMoveRange.Clear();
    }

    private void SetSquaresOutOfAttackReach()
    {
        foreach (Squar item in _squaresInUnitAttackRange)
        {
            item.DisableAttackBorders();
            item.isInAttackReach = false;
        }
        _squaresInUnitAttackRange.Clear();
    }

    private void SetCursor(Squar sq)
    {
        sq.CursorEvent.canAttack = false;
        sq.CursorEvent.isInMoveRange = false;

        if (sq.unitInSquar is null)
        {
            if (_squaresInUnitMoveRange.Contains(sq))
            {
                sq.CursorEvent.isInMoveRange = true;
            }
        }
        else
        {
            if(_squaresInUnitAttackRange.Contains(sq) && sq.unitInSquar._team != _activeUnit._team)
            {
                sq.CursorEvent.canAttack = true;
            }
        }
    }

    private void SortUnitAccordingIniciation()
    {
        _unitsOnBattleField.Sort((x, y) => y._iniciation.CompareTo(x._iniciation));
    }

    private void UpdateRightPanel (Squar sq)
    {
        Unit unit = sq.unitInSquar;

        if (unit == _activeUnit)
            return;

        if (unit != null)
        {
            _rightUnitInfo.UpdateStats(unit);
            _rightUnitInfo.gameObject.SetActive(true);

        }
        //else
        //{
        //   // rightUnitInfo.DisablePanel();
        //}
    }

    private void SetupForNewBattle()
    {
        foreach (Unit unit in _unitsOnBattleField)
        {
            Destroy(unit.gameObject);
        }

        _unitsOnBattleField.Clear();
        _squaresInUnitAttackRange.Clear();
        _squaresInUnitMoveRange.Clear();

        _battleInfoPanel.RestartDataForNewBattle();

        // Todo Hodne smutne..  Musím to delat tak ze budu deaktivovat ty ktere nepotrebuji a hnedna na startu určím jak velke je pole
        // a tím padem jaké battleSquary maji byt aktivni.

        _battleGridController.ClearnSquaresInBattleField();
    }

    private void InputProcess ()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ChangeWeapon();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            SkipUnitTurn();
        }
    }

    enum BattleAction 
    {
        None,
        Move,
        Attack,
        Heal
    }
}
