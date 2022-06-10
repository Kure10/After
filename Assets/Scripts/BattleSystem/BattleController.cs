using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;
using System.Threading.Tasks;

public partial class BattleController : MonoBehaviour
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

    [Tooltip("In MiliSec")]
    [SerializeField] private int _delayWalk = 350;

    [Header("Cursor Images")]
    [SerializeField] Sprite attackAction;
    [SerializeField] Sprite outOfRange;
    [SerializeField] Sprite healAction;
    [SerializeField] Sprite moveAction;


    [Space]
    public GameObject _unit;

    public static event Action<StatsClass> OnBattleLost = delegate { };
    public static event Action OnBattleEnd = delegate { };
    public static event Action OnBattleStart = delegate { };

    public static bool IsBattleAlive = false;

    bool _turnIsOver = false;
    private bool _isPlayerTurn = false;
    private bool _isPerformingAction = false;

    private Unit _activeUnit = null;

    private int _order = 0;
    private int roundCount = 1;

    private List<Squar> _squaresInUnitMoveRange = new List<Squar>();
    private List<Squar> _squaresInUnitAttackRange = new List<Squar>();

    private ResourceSpriteLoader spriteLoader = null;
    private BattleStartData _battleStartData;
    private ResultTurnAction _playerInput = new ResultTurnAction();

    private BattlePathFinding _battlePathFinder;

    public List<Squar> GetSquaresInAttackRange { get { return _squaresInUnitAttackRange; } }
    public List<Squar> GetSquaresInMoveRange { get { return _squaresInUnitMoveRange; } }
    public int GetDelayWalk { get { return _delayWalk; } }
    public Unit GetActiveUnit { get { return _activeUnit; } }
    public bool IsPerformingAction { get { return _isPerformingAction; } set {  _isPerformingAction = value; } }

    public BattleGridController GetBattleGridController { get { return _battleGridController; } }

    public BattlePathFinding GetBattkePathFinder { get { return _battlePathFinder; } }

    // testing 
    List<Squar> shootPathSq = new List<Squar>();

    private void Awake()
    {
        _battlePathFinder = new BattlePathFinding(_battleGridController);
        _battleResultPopup.InicializedControlles(() => CloseBattle());
    }
    private void Start()
    {
        // This is for testing purpose only when u are in BATTLEGROUND scene!!
        if (TestingBattle)
        {
            spriteLoader = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceSpriteLoader>();
            _battleStartData = InitTestBattleData();

            _battleGridController.CreateBattleField(_battleStartData);

            SetUnitPosition(_battleStartData);
            InitBattle(_battleStartData);
            TestStartBattle();
        }
    }

    private async void Update()
    {
       // bool actionPerformed = false;
        if (IsBattleAlive)
        {
            InputProcess();

            if (_isPlayerTurn)
            {
                _playerInput = await DetectPlayerInputs();

                if (_playerInput.inputResult)
                {
                   // actionPerformed = _playerInput.inputResult;
                    DecreaseActionPoints(_playerInput);

                    if (_activeUnit.ActionPoints <= 0 || _turnIsOver)
                    {
                        _turnIsOver = true;
                        _isPlayerTurn = false;
                    }
                }
            }
            else
            {
                _playerInput = await DetectPlayerInputs();
                // OnSimpleAIMove();

                if (_playerInput.inputResult)
                {
                  //  actionPerformed = _playerInput.inputResult;
                    DecreaseActionPoints(_playerInput);

                    if (_activeUnit.ActionPoints <= 0 || _turnIsOver)
                    {
                        _turnIsOver = true;
                        _isPlayerTurn = true;
                    }
                }

                // AI is on the move 
                // start corotine
                // switch to player turn
            }

            if (_playerInput.inputResult && !_turnIsOver)
            {
                RecalculatePosibleActions(_playerInput);
            }

            if (_turnIsOver)
            {
                ConfigurateNewTurn();
            }
        }
    }

    // for buttons
    public void CloseBattle()
    {
        OnBattleEnd.Invoke();
        IsBattleAlive = false;
        this.gameObject.SetActive(false);
        _battleResultPopup.OnPressExit();
    }

    // for buttons
    public void SkipUnitTurn()
    {
        _turnIsOver = true;
        _battleLog.AddBattleLog($"{_activeUnit._name} skip round");
    }

    public void ConfigurateNewTurn()
    {
        _battleGridController.SetSquaresOutOfAttackReach(_squaresInUnitAttackRange);
        _battleGridController.SetSquaresOutOfMoveRange(_squaresInUnitMoveRange);
        _battleGridController.ShowSquaresWithinMoveRange(_squaresInUnitMoveRange, false); 

        _order++;

        int countAliveUnit = 0;
        foreach (Unit unit in _unitsOnBattleField)
        {
            if (!unit.IsDead)
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
                if (!unit.IsDead)
                {
                    unit._iniciation = unit.CalculateIniciation();
                }
            }

            SortUnitAccordingIniciation();
            _battleInfoPanel.UpdateUnitNewTurnOrder(_unitsOnBattleField, _unit);
        }

        UpdateActiveUnit();
        _squaresInUnitMoveRange.AddRange(_battleGridController.FindSquaresInUnitMoveRange(_activeUnit));
        _battleGridController.ShowSquaresWithinMoveRange(_squaresInUnitMoveRange, true);
        _squaresInUnitAttackRange.AddRange(_battleGridController.FindSquaresInUnitAttackRange(_activeUnit));
        _battleGridController.ShowSquaresWithingAttackRange(_squaresInUnitAttackRange);
        _leftUnitInfo.UpdateStats(_activeUnit);

        _turnIsOver = false;

        VictoryConditionCheck();
    }

    private void UpdateActiveUnit()
    {
        _battleInfoPanel.UpdateUnitOrder(_activeUnit, false);
        _activeUnit.IsActive = false;
        _activeUnit.UpdateAnim();
        _playerInput.ResertTurn();

        for (int i = _order; i < _unitsOnBattleField.Count; i++)
        {
            _activeUnit = _unitsOnBattleField[i];

            if (!_activeUnit.IsDead)
                break;
        }

        _activeUnit.ActionPoints = 2;
        _activeUnit.IsActive = true;
        _activeUnit.UpdateAnim();
        _activeUnit.RefreshMovementPoints();
        _battleInfoPanel.UpdateUnitOrder(_activeUnit, true);

        _battleLog.AddBattleLog($"{_activeUnit._name} has turn");
    }

    private async Task<ResultTurnAction> DetectPlayerInputs()
    {
        ResultTurnAction resultPlayerInput = _playerInput;

        resultPlayerInput.inputResult = false;
        resultPlayerInput.battleAction = BattleAction.None;
        Squar targetActionOnSquare = null;
        Unit targetUnit = null;

        // Testing Space middle mouse button
        if (Input.GetMouseButtonDown(2))
        {
            targetActionOnSquare = RaycastTargetSquar();
            if (targetActionOnSquare != null)
            {
                TryGetAim(targetActionOnSquare, true);
            }
        }


        if (Input.GetMouseButtonDown(0))
        {
            targetActionOnSquare = RaycastTargetSquar();
            if (targetActionOnSquare != null)
            {
                resultPlayerInput.battleAction = OnClickIntoGrid(targetActionOnSquare);
                targetUnit = targetActionOnSquare.UnitInSquar;
                resultPlayerInput.inputResult = true;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            targetActionOnSquare = RaycastTargetSquar();
            if (targetActionOnSquare != null)
            {
                if (targetActionOnSquare.UnitInSquar != null)
                {
                    detailPopup.ShowPopup(targetActionOnSquare.UnitInSquar);
                }

                Debug.Log("Info unit panel popup");
            }
        }

        if (resultPlayerInput.inputResult && !IsPerformingAction)
        {
            switch (resultPlayerInput.battleAction)
            {
                case BattleAction.None:
                    Debug.Log("None Action");
                    resultPlayerInput.inputResult = false;
                    break;
                case BattleAction.Move:

                    if (!_playerInput.moveIsBlocked)
                    {
                        await Move(targetActionOnSquare);
                        _battleLog.AddBattleLog($"{_activeUnit._name} moved");
                        resultPlayerInput.inputResult = true;
                    }

                    break;
                case BattleAction.Attack:

                    if (!_playerInput.attackIsBlocked)
                    {
                        MelleAttack(targetUnit);
                        resultPlayerInput.inputResult = true;
                    }
                    break;
                case BattleAction.RangeAttack:
                    if (!_playerInput.attackIsBlocked)
                    {
                        MelleAttack(targetUnit);
                        resultPlayerInput.inputResult = true;
                    }
                    break;
                case BattleAction.Heal:
                    Debug.Log("Heal action");
                    resultPlayerInput.inputResult = false;
                    break;
                default:
                    break;
            }
        }
        else
        {
            resultPlayerInput.inputResult = false;
        }

        return resultPlayerInput;
    }

    public void RecalculatePosibleActions(ResultTurnAction playerInput)
    {
        _battleGridController.SetSquaresOutOfMoveRange(_squaresInUnitMoveRange);

        _battleGridController.SetSquaresOutOfAttackReach(_squaresInUnitAttackRange);
        _squaresInUnitAttackRange.AddRange(_battleGridController.FindSquaresInUnitAttackRange(_activeUnit));

        if(_activeUnit.GetMovementPoints > 0)
        {
            _squaresInUnitMoveRange.AddRange(_battleGridController.FindSquaresInUnitMoveRange(_activeUnit));
            _battleGridController.ShowSquaresWithinMoveRange(_squaresInUnitMoveRange, true);
        }

        _battleGridController.ShowSquaresWithingAttackRange(_squaresInUnitAttackRange);

        _leftUnitInfo.UpdateStats(_activeUnit);

        playerInput.inputResult = false;
    }

    public void DecreaseActionPoints(ResultTurnAction playerInput)
    {
        switch (playerInput.battleAction)
        {
            case BattleAction.None:
                break;
            case BattleAction.Move:
                if(!playerInput.movePerformed)
                {
                    _activeUnit.ActionPoints -= 1;
                    playerInput.movePerformed = true;
                }

                if (_activeUnit.GetMovementPoints <= 0)
                {
                    playerInput.moveIsBlocked = true;
                }

                break;
            case BattleAction.Attack:
                playerInput.attackIsBlocked = true;
                _activeUnit.ActionPoints -= 1;
                _turnIsOver = true;
                break;
            case BattleAction.Heal:
                break;
            default:
                break;
        }
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


        _battleGridController.SetSquaresOutOfAttackReach(_squaresInUnitAttackRange);
        _squaresInUnitAttackRange.AddRange(_battleGridController.FindSquaresInUnitAttackRange(_activeUnit));
        _battleGridController.ShowSquaresWithingAttackRange(_squaresInUnitAttackRange);

        _activeUnit.UpdateData();
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

        _battleGridController.SetSquaresOutOfAttackReach(_squaresInUnitAttackRange);
        _squaresInUnitAttackRange.AddRange(_battleGridController.FindSquaresInUnitAttackRange(_activeUnit));
        _battleGridController.ShowSquaresWithingAttackRange(_squaresInUnitAttackRange);

        _activeUnit.UpdateData();
        _leftUnitInfo.UpdateStats(_activeUnit);
        _battleInfoPanel.UpdateUnitData(_activeUnit);
    }

    public void StartBattle(BattleStartData battleStartData)
    {
        IsBattleAlive = true;
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

        
        OnBattleStart.Invoke();
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
            Squar squar = _battleGridController.GetUnBlockedSquareFromGrid(dataUnit.StartXPosition, dataUnit.StartYPosition); // tady musí byt chech jestli nejsem mimo pole

            GameObject unt = Instantiate(_unit, squar.GetContainer.transform);
            Unit newUnit = unt.GetComponent<Unit>();

            var sprite = spriteLoader.LoadUnitSprite(dataUnit.ImageName);

            newUnit.InitUnit(dataUnit, sprite, Unit.Team.Demon);

            squar.UnitInSquar = newUnit;
            _unitsOnBattleField.Add(newUnit);
        }

        for (int i = 0; i < amountPlayers; i++)
        {
            DataUnit dataUnit = battleData.playerData.playerUnits[i];
            Squar squar = _battleGridController.GetUnBlockedSquareFromGrid(dataUnit.StartXPosition, dataUnit.StartYPosition);  // tady musí byt chech jestli nejsem mimo pole

            GameObject unit1 = Instantiate(_unit, squar.GetContainer.transform);
            Unit newUnit = unit1.GetComponent<Unit>();

            var sprite = spriteLoader.LoadSpecialistSprite(dataUnit.ImageName);

            newUnit.InitUnit(dataUnit, sprite, Unit.Team.Human);

            squar.UnitInSquar = newUnit;
            _unitsOnBattleField.Add(newUnit);
        }

        // Sort unit order.
        SortUnitAccordingIniciation();
        _battleInfoPanel.InitStartOrder(_unitsOnBattleField, _unit);

        // init Event
        foreach (Squar square in BattleGridController.GetSquarsFromBattleField)
        {
            square.InitEvent(delegate (Squar squ)
            {
                SetCursor(squ);
                UpdateRightPanel(squ);
                EvaluateGridWTF(squ);
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
            // TODo třeba když je ambush tak by se měli nastavit specialni pozice atd..
            SetEnemyRandomPosition(battleStartData);
        }
    }

    private void SetPlayersUnitPosition(BattleStartData battleStartData)
    {
        List<(int x, int y)> posiblePositions = new List<(int x, int y)>();

        int startSeachXPosition = 0;
        int radius = 3; // Hard coded not good..

        if (battleStartData.Rows % 2 == 0)
        {
            startSeachXPosition = (battleStartData.Rows - 1) / 2 - 1;
        }
        else
        {
            startSeachXPosition = (battleStartData.Rows - 1) / 2;
        }

        for (int i = startSeachXPosition; i < startSeachXPosition + radius; i++)
        {
            (int x, int y) position = (0, 0);

            for (int j = 0; j < radius; j++)
            {
                position = (i, j);

                if(!BattleGridController.GetSquarsFromBattleField[i,j].IsSquearBlocked && BattleGridController.GetSquarsFromBattleField[i, j].UnitInSquar == null)
                {
                    posiblePositions.Add(position);
                }
            }
        }

        // todo
        if (battleStartData.playerData.playerUnits.Count > posiblePositions.Count)
        {
            // Muže se stát pokud jsou square zablokované tak se ani nepridaji do (posiblePositions)  a nebo je moc jednotek
            // max je 9 zatím
            Debug.LogError("Number of Players is bigger than posiblePositions to put unit on BattleField Critical Error");
        }

        // set randomPosition from list of available positions
        foreach (DataUnit dataUnit in battleStartData.playerData.playerUnits)
        {
            (int x, int y) occupiedPosition = dataUnit.SetRandomStartingPosition(posiblePositions);

            posiblePositions.Remove(occupiedPosition);
        }
    }

    private void SetEnemyRandomPosition(BattleStartData battleStartData)
    {
        List<(int x, int y)> posiblePositions = new List<(int x, int y)>();

        int startSeachYPosition = 0;
        int radius = 3; // Hard coded not good..
        int minDistanceFromPlayer = 5; // Hard coded not good..

        foreach (DataUnit dataUnit in battleStartData.playerData.playerUnits)
        {
            if (dataUnit.StartYPosition > startSeachYPosition)
            {
                startSeachYPosition = dataUnit.StartYPosition;
            }
        }

        startSeachYPosition += minDistanceFromPlayer;

        for (int i = 0; i < battleStartData.Rows - 1; i++)
        {
            (int x, int y) position = (0, 0);

            for (int j = startSeachYPosition; j < battleStartData.Collumn - 1; j++)
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

        Weapon weapon = new Weapon(3, 1, 5, 6);
        Weapon weapon2 = new Weapon(4, 2, 3, 4);

        DataUnit newA = new DataUnit(2, 3, 10, 5, 3, 3, 2, "Player1", "Gargoyle", 2, weapon, weapon2);
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
        if (_unitsOnBattleField.Count > 0)
        {

            //  UpdateActiveUnit();

            
            _playerInput.ResertTurn();
            _activeUnit = _unitsOnBattleField[_order];
            _activeUnit.ActionPoints = 2;
            _activeUnit.IsActive = true;
            _activeUnit.UpdateAnim();
            _activeUnit.RefreshMovementPoints();
            _battleInfoPanel.UpdateUnitOrder(_activeUnit, true);

            _squaresInUnitMoveRange.AddRange(_battleGridController.FindSquaresInUnitMoveRange(_activeUnit));
            _battleGridController.ShowSquaresWithinMoveRange(_squaresInUnitMoveRange, true);

             _squaresInUnitAttackRange.AddRange(_battleGridController.FindSquaresInUnitAttackRange(_activeUnit));
            _battleGridController.ShowSquaresWithingAttackRange(_squaresInUnitAttackRange);

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
            Squar sq = raycastResultsList[i].gameObject.GetComponent<Squar>();

            if (sq != null)
            {
                return sq;
            }
        }

        return null;
    }

    private List<Squar> RaycastPointsOnGrid(List<Vector2> points)
    {
        List<Squar> posibleSQ = new List<Squar>();
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);

        foreach (Vector2 vec in points)
        {
            pointerEventData.position = vec;
            List<RaycastResult> raycastResultsList = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResultsList);

            for (int i = 0; i < raycastResultsList.Count; i++)
            {
                Squar sq = raycastResultsList[i].gameObject.GetComponent<Squar>();

                if (sq != null)
                {
                    if(!posibleSQ.Contains(sq))
                        posibleSQ.Add(sq);

                    break;
                }
            }
        }
        return posibleSQ;
    }

    private bool VictoryConditionCheck()
    {
        bool battleIsOver = false;
        int humanUnit = 0;
        int demonUnit = 0;
        int neutralUnit = 0;

        foreach (var unit in _unitsOnBattleField)
        {
            if (!unit.IsDead)
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
            // Todo Clean BattleField . And prepair for next Battle
        }

        return false;
    }

    private StatsClass CreateStatClassBattleResult(bool positiveResult)
    {
        StatsClass statClass = new StatsClass();

        if (positiveResult)
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
                    ItemBlueprint itemBlueprint = _inventoryManager.GetResourcesByID(loot.lootID);
                    itemBlueprint.sizeStock = itemCount;
                    itemsLoot.Add(itemBlueprint);

                    // Kdyby je nef chtel mit rozdelene..
                    //for (int i = 0; i < itemCount; i++)
                    //{
                    //    itemsLoot.Add(_inventoryManager.GetResourcesByID(loot.lootID));
                    //}
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

    private void SetCursor(Squar sq)
    {
        sq.CursorEvent.action = BattleController.BattleAction.Move;

        if (sq.UnitInSquar is null)
        {
            if (_squaresInUnitMoveRange.Contains(sq))
            {
                sq.CursorEvent.action = BattleController.BattleAction.Move;
                sq.SetActionMark(moveAction);
            }
        }
        else
        {
            if (_squaresInUnitAttackRange.Contains(sq) && sq.UnitInSquar._team != _activeUnit._team)
            {
                if(_activeUnit.ActiveWeapon.IsMelleWeapon)
                {
                    sq.CursorEvent.action = BattleController.BattleAction.Attack;
                    sq.SetActionMark(attackAction);
                }
                else
                {
                    if (TryGetAim(sq))
                    {
                        Debug.Log("JSem tady ale nemel bych byt");
                        sq.CursorEvent.action = BattleController.BattleAction.Attack;
                        sq.SetActionMark(attackAction);
                    }
                    else
                    {
                        sq.SetActionMark(outOfRange);
                    }
                }
            }
            else
            {
                sq.CursorEvent.action = BattleController.BattleAction.Heal;
                sq.SetActionMark(healAction);
            }
        }
    }

    private void SortUnitAccordingIniciation()
    {
        _unitsOnBattleField.Sort((x, y) => y._iniciation.CompareTo(x._iniciation));
    }

    private void UpdateRightPanel(Squar sq)
    {
        Unit unit = sq.UnitInSquar;

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

    // Testing Only
    public void EvaluateGridWTF(Squar endSquare)
    {
        Squar activeUnitSquare = _battleGridController.GetSquareFromGrid(_activeUnit.CurrentPos.XPosition, _activeUnit.CurrentPos.YPosition);

        _battlePathFinder.EvaluateGridCost(activeUnitSquare, endSquare);
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

    private void InputProcess()
    {
        if (IsPerformingAction)
            return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ChangeWeapon();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            SkipUnitTurn();
        }
    }

    public enum BattleAction
    {
        None,
        Move,
        Attack,
        RangeAttack,
        Heal
    }

    public class ResultTurnAction
    {
        public bool movePerformed = false;
        public bool moveIsBlocked;
        public bool attackIsBlocked;

        public BattleAction battleAction;
        public bool inputResult;

        public void ResertTurn()
        {
            movePerformed = false;

            moveIsBlocked = false;
            attackIsBlocked = false;        

            battleAction = BattleAction.None;
            inputResult = false;
        }
    }
}
