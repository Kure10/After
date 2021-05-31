using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.Events;

public class BattleController : MonoBehaviour
{
    [Header("Testing")]
    public bool TestingBattle = false;

    [Header("Main")]
    [SerializeField] InventoryManager _inventoryManager;

    [Header("Info Panels")]
    [SerializeField] private BattleInfoPanel battleInfoPanel;
    [SerializeField] private UnitInfoPanel leftUnitInfo;
    [SerializeField] private UnitInfoPanel rightUnitInfo;
    [SerializeField] private BattleLogPanel battleLog;
    [SerializeField] private BattleResultPopup battleResultPopup;

    [Header("Info Popup")]
    [SerializeField] private DetailUnitPopup detailPopup;

    [Header("Dimensions")]
    [SerializeField] List<GameObject> _rows = new List<GameObject>();
    [Space]
    [SerializeField] int _collumCount = 16;
    [SerializeField] int _rowsCount = 16;

    [Space]
    [Header("Others")]
    private Squar[,] _squaresInBattleField; 

    public List<Unit> unitsOnBattleField = new List<Unit>();

    [Space]
    public GameObject _unit;

    public GameObject squarTemplate;


    /// <summary>
    /// ////////////////////
    ///   FOR now needed we will see 
    ///    musim v tom udelat poradek neco budu potrebovat neco ne
    /// </summary>
    /// 

    bool _turnIsOver = false;
    private bool _isBattleOnline = false;
    private bool _isPlayerTurn = false;

    private Unit _activeUnit = null;

    private int _order = 0;

    private int roundCount = 1;

    private List<Squar> _squaresInUnitRange = new List<Squar>();

    private List<Squar> _squaresInUnitAttackRange = new List<Squar>();

    private ResourceSpriteLoader spriteLoader = null;

    private BattleStartData _battleStartData;

    private void Awake()
    {
        
    }
    private void Start()
    {
        // This is for testing purpose only when u are in BATTLEGROUND scene!!
        if(TestingBattle)
        {
            spriteLoader = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceSpriteLoader>();
            _battleStartData = InitTestBattleData();

            CreateBattleField(_battleStartData.Rows, _battleStartData.Collumn);
            InitBattle(_battleStartData);
            TestStartBattle();
        }

    }

    private void Update()
    {
        if (_isBattleOnline)
        {
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
                foreach (Unit unit in unitsOnBattleField)
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
                    battleLog.AddBattleLog($"<---------- Turn {roundCount} ---------->");

                   // new iniciative
                    foreach (Unit unit in unitsOnBattleField)
                    {
                        if(!unit.IsDead)
                        {
                            unit._iniciation = unit.CalculateIniciation();
                        }
                    }

                    SortUnitAccordingIniciation();
                    battleInfoPanel.UpdateUnitNewTurnOrder(unitsOnBattleField, _unit);
                }
 
                UpdateActiveUnit();

                FindSquaresInUnitMoveRange();
                ShowSquaresWithinRange(true);


                FindSquaresInUnitAttackRange(_activeUnit.ActiveWeapon);
                ShowSquaresWithingAttackRange();

                leftUnitInfo.UpdateStats(_activeUnit);

                _turnIsOver = false;

                VictoryConditionCheck();
            }
        }
    }

    private void UpdateActiveUnit()
    {
        battleInfoPanel.UpdateUnitOrder(_activeUnit, false);
        _activeUnit.IsActive = false;
        _activeUnit.UpdateAnim();

        for (int i = _order; i < unitsOnBattleField.Count; i++)
        {
            _activeUnit = unitsOnBattleField[i];

            if (!_activeUnit.IsDead)
                break;
        }

        _activeUnit.IsActive = true;
        _activeUnit.UpdateAnim();
        battleInfoPanel.UpdateUnitOrder(_activeUnit, true);

        battleLog.AddBattleLog($"{_activeUnit._name} has turn");
    }

    private bool DetectPlayerInputs()
    {
        bool result = false;
        BattleAction action = BattleAction.None;
        Squar squarToMove = null;
        Unit unitOnSquare = null;

        if (Input.GetMouseButtonDown(0))
        {
            squarToMove = RaycastTargetSquar();
            if (squarToMove != null)
            {
                action = OnClickIntoGrid(squarToMove);
                unitOnSquare = squarToMove.unitInSquar;
                result = true;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            squarToMove = RaycastTargetSquar();
            if (squarToMove != null)
            {
                if(squarToMove.unitInSquar != null)
                {
                    detailPopup.ShowPopup(squarToMove.unitInSquar);
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
                    MoveToSquar(squarToMove);
                    //battleLog.AddLog($"{_activeUnit._name} moved to square {squarToMove.xCoordinate} / {squarToMove.yCoordinate}");
                    battleLog.AddBattleLog($"{_activeUnit._name} moved");
                    result = true;
                    break;
                case BattleAction.Attack:
                    AttackInfo attackInfo = null;
                    attackInfo = AttackToUnit(unitOnSquare);
                    battleLog.AddAttackBattleLog(attackInfo, _activeUnit, unitOnSquare);
                    battleInfoPanel.UpdateUnitData(unitOnSquare);
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

    // for buttons
    public void SkipUnitTurn()
    {
        _turnIsOver = true;
        battleLog.AddBattleLog($"{_activeUnit._name} skip round");
    }

    // for buttons
    public void ChangeWeapon(bool changeToLeft)
    {
        if(changeToLeft)
        {
            if (_activeUnit.ActiveWeapon == _activeUnit.SecondWeapon)
                return;

            _activeUnit.ActiveWeapon = _activeUnit.SecondWeapon;
        }
        else
        {
            if (_activeUnit.ActiveWeapon == _activeUnit.FirstWeapon)
                return;

            _activeUnit.ActiveWeapon = _activeUnit.FirstWeapon;
        }

        SetSquaresOutOfAttackReach();
        FindSquaresInUnitAttackRange(_activeUnit.ActiveWeapon);
        ShowSquaresWithingAttackRange();

        _activeUnit.UpdateData(_activeUnit);
        leftUnitInfo.UpdateStats(_activeUnit);
        battleInfoPanel.UpdateUnitData(_activeUnit);

        // zmenit panel jednotky
        // i info panel
        // a i panel left botton

    }

    public void StartBattle(BattleStartData battleStartData)
    {
        _battleStartData = battleStartData;

        spriteLoader = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceSpriteLoader>();


        // this is for testing purpose .. Right now is not decided how we will set battlefield Size.
        // minumum is 6 and 10 
        _battleStartData.Rows = 8;
        _battleStartData.Collumn = 16;

        CreateBattleField(_battleStartData.Rows, _battleStartData.Collumn);
        SetUnitPosition(_battleStartData);
        InitBattle(_battleStartData);
        TestStartBattle();
    }

    private void CreateBattleField(int rowsCount, int collumCount)
    {
        this._collumCount = collumCount;
        this._rowsCount = rowsCount;

        _squaresInBattleField = new Squar[this._rowsCount, this._collumCount];

        for (int j = 0; j < this._rowsCount; j++)
        {
            GameObject row = _rows[j];

            for (int i = 0; i < this._collumCount; i++)
            {
                GameObject squarGameObject = Instantiate(squarTemplate, row.transform);
                Squar square = squarGameObject.GetComponent<Squar>();
                square.SetCoordinates(j, i);

                _squaresInBattleField[j, i] = square;

                square.InitEvent(delegate (Squar squ)
                {
                    SetCursor(squ);
                    UpdateRightPanel(squ);
                });
            }
        }
    }

    private void InitBattle(BattleStartData battleData)
    {
        battleLog.AddBattleLog("Battle Start");

        this.gameObject.SetActive(true);
        _isPlayerTurn = true; // todo for now is not decided who will turn first .. ? ?? Now player...

        _order = 0;
        int amountEnemies = battleData.enemyData.enemieUnits.Count;
        int amountPlayers = battleData.playerData.playerUnits.Count;


        for (int i = 0; i < amountEnemies; i++)
        {
            DataUnit dataUnit = battleData.enemyData.enemieUnits[i];
            Squar squar = GetSquareFromGrid(dataUnit.StartXPosition, dataUnit.StartYPosition); // tady musí byt chech jestli nejsem mimo pole

            GameObject unt = Instantiate(_unit, squar.container.transform);
            Unit newUnit = unt.GetComponent<Unit>();

            var sprite = spriteLoader.LoadUnitSprite(dataUnit.ImageName);

            newUnit.InitUnit(dataUnit, sprite, Unit.Team.Demon);

            squar.unitInSquar = newUnit;
            unitsOnBattleField.Add(newUnit); 
        }

        for (int i = 0; i < amountPlayers; i++)
        {
            DataUnit dataUnit = battleData.playerData.playerUnits[i];
            Squar squar = GetSquareFromGrid(dataUnit.StartXPosition, dataUnit.StartYPosition);  // tady musí byt chech jestli nejsem mimo pole

            GameObject unit1 = Instantiate(_unit, squar.container.transform);
            Unit newUnit = unit1.GetComponent<Unit>();

            var sprite = spriteLoader.LoadSpecialistSprite(dataUnit.ImageName);

            newUnit.InitUnit(dataUnit, sprite, Unit.Team.Human);

            squar.unitInSquar = newUnit;
            unitsOnBattleField.Add(newUnit);
        }

        // Sort unit order.
        SortUnitAccordingIniciation();
        battleInfoPanel.InitStartOrder(unitsOnBattleField, _unit);

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

        if (unitsOnBattleField.Count > 0)
        {
            _activeUnit = unitsOnBattleField[_order];
            _activeUnit.IsActive = true;
            _activeUnit.UpdateAnim();
            battleInfoPanel.UpdateUnitOrder(_activeUnit, true);

            //
            FindSquaresInUnitMoveRange();
            ShowSquaresWithinRange(true);

            // testing fire range
            FindSquaresInUnitAttackRange(_activeUnit.ActiveWeapon);
            ShowSquaresWithingAttackRange();

            leftUnitInfo.UpdateStats(_activeUnit);
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
    private Squar FindSquarForAI()
    {
        int rowsCount = _rowsCount - 1;

        while (true)
        {
            int xCor = Random.Range(0, rowsCount);
            int yCor = Random.Range(0, _collumCount);

            if (_squaresInBattleField[xCor, yCor].unitInSquar == null)
            {
                return _squaresInBattleField[xCor, yCor];
            }
        }
    }

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
            if (_squaresInUnitRange.Contains(squarToMove))
            {
                action = BattleAction.Move;
            }

            return action;
        }
    }
    private void OnSimpleAIMove()
    {
        Squar squarToMove = FindSquarForAI();
        MoveToSquar(squarToMove);
    }

    private void MoveToSquar(Squar squarToMove)
    {
        Squar selectedUnitSquar = GetSquareFromGrid(_activeUnit.CurrentPos.XPosition,_activeUnit.CurrentPos.YPosition);

        selectedUnitSquar.unitInSquar.transform.SetParent(squarToMove.container.transform);
        selectedUnitSquar.unitInSquar.SetNewCurrentPosition(squarToMove.xCoordinate, squarToMove.yCoordinate);
        squarToMove.unitInSquar = selectedUnitSquar.unitInSquar;
        selectedUnitSquar.unitInSquar = null;
    }

    private AttackInfo AttackToUnit(Unit defendUnit)
    {
        AttackInfo attackInfo = new AttackInfo();

        int dices = BattleSystem.CalculateAmountDices(_activeUnit);
        int success = BattleSystem.CalculateAmountSuccess(dices , _activeUnit ,defendUnit, out attackInfo.dicesValueRoll);

        defendUnit.CurrentHealth = defendUnit.CurrentHealth - success;

        var isDead = defendUnit.CheckIfUnitIsNotDead();

        if (isDead)
        {
            DestroyUnitFromBattleField(defendUnit);
            battleInfoPanel.DeleteUnitFromOrder(defendUnit);
            battleLog.AddBattleLog($"{defendUnit._name} is dead");
        }

        // for info
        attackInfo.dices = dices;
        attackInfo.success = success;

        return attackInfo;
    }

    private void DestroyUnitFromBattleField(Unit unit)
    {
        Squar sq = GetSquareFromGrid(unit.CurrentPos.XPosition, unit.CurrentPos.YPosition);

        sq.unitInSquar.gameObject.SetActive(false);

       // deadUnitsOnBattleField.Add(sq.unitInSquar);

        // Destroy(sq.unitInSquar.gameObject, 0.5f);
        sq.unitInSquar = null;
    }

    private bool VictoryConditionCheck ()
    {
        bool battleIsOver = false;
        int humanUnit = 0;
        int demonUnit = 0;
        int neutralUnit = 0;

        foreach (var unit in unitsOnBattleField)
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
            // human wins
            Debug.Log("Human wins");
            //battleResultPopup.neco`
            _battleStartData.UpdateMainPlayerData(unitsOnBattleField);

            battleResultPopup.InitPlayerUnits(unitsOnBattleField, _unit);

            List<ItemBlueprint> itemsLoot = new List<ItemBlueprint>();

            foreach (DataUnit dataUnit in _battleStartData.enemyData.enemieUnits)
            {
                foreach (Monster.Loot loot in dataUnit.GetLoot)
                {
                    int randomNumber = Random.Range(0,101); // Todo Lepsi random.
                    int itemCount = Random.Range(loot.itemCountMin, loot.itemCountMax +1); // Todo Lepsi random.
                    if (randomNumber <= loot.dropChange)
                    {
                        for (int i = 0; i < itemCount; i++)
                        {
                            itemsLoot.Add(_inventoryManager.GetResourcesByID(loot.lootID));
                        } 
                    }
                }
            }

            battleResultPopup.InicializedStartInventory(itemsLoot);

            battleResultPopup.InicializedCharacter(_battleStartData.GetCharacterFromBattle);
            battleIsOver = true;

            battleResultPopup.ShowBattleResult();
        }

        if (humanUnit == 0 && neutralUnit == 0)
        {
            // demon wins
            Debug.Log("Demon wins");
            battleIsOver = true;
        }

        if (demonUnit == 0 && humanUnit == 0)
        {
            // neutral wins
            Debug.Log("Neutral wins");
            battleIsOver = true;
        }

        if (battleIsOver)
        {
            _isBattleOnline = false;
        }
        

        return false;
    }

    // in this method i can mark squares and maybe change method for move..  Can be more easy to find..
    private void FindSquaresInUnitMoveRange()
    {
        _squaresInUnitRange.Clear();

        int moveRange = _activeUnit._movement;
        Squar centerSquar = _squaresInBattleField[_activeUnit.CurrentPos.XPosition, _activeUnit.CurrentPos.YPosition];

        _squaresInUnitRange.AddRange(GetTheAdjacentSquare(centerSquar));

        List<Squar> adjectedSq = new List<Squar>();
        adjectedSq.AddRange(_squaresInUnitRange);

        for (int i = 1; i < moveRange; i++)
        {
            List<Squar> adjectedSquarsInCurrentRange = new List<Squar>();

            foreach (Squar sq in adjectedSq)
            {
                adjectedSquarsInCurrentRange.AddRange(GetTheAdjacentSquare(sq));
            }

            foreach (Squar sq in adjectedSq)
            {
                sq.isInMoveRange = true;
            }

            adjectedSq.Clear();
            adjectedSq.AddRange(adjectedSquarsInCurrentRange);

            _squaresInUnitRange.AddRange(adjectedSq);
        }

        _squaresInUnitRange.Add(centerSquar);
    }

    private void FindSquaresInUnitAttackRange(Item weapon)
    {
        int attackMaxRange = 0;
        int attackMinRange = 0;
        //todo
        if (weapon == null)
        {
            attackMinRange = _activeUnit._rangeMin;
            attackMaxRange = _activeUnit._rangeMax;
        }
        else
        {
             attackMaxRange = _activeUnit.ActiveWeapon.RangeMax;
             attackMinRange = _activeUnit.ActiveWeapon.RangeMin;
        }

        _squaresInUnitAttackRange.Clear();

        Squar centerSquar = _squaresInBattleField[_activeUnit.CurrentPos.XPosition, _activeUnit.CurrentPos.YPosition];

        _squaresInUnitAttackRange.AddRange(GetTheAdjacentAttackSquare(centerSquar));

        List<Squar> squaresToMinAtackRange = new List<Squar>();

        List<Squar> lastAdjectedSq = new List<Squar>();
        lastAdjectedSq.AddRange(_squaresInUnitAttackRange);

        squaresToMinAtackRange.AddRange(_squaresInUnitAttackRange);


        for (int i = 1; i < attackMaxRange; i++)
        {
            List<Squar> adjectedSquarsInCurrentRange = new List<Squar>();

            foreach (Squar sq in lastAdjectedSq)
            {
                adjectedSquarsInCurrentRange.AddRange(GetTheAdjacentAttackSquare(sq));
            }

            lastAdjectedSq.Clear();
            lastAdjectedSq.AddRange(adjectedSquarsInCurrentRange);

            if (i < attackMinRange)  //  i <= by znamenalo ze zahrnuje hranici minimalniho range.
            {
                squaresToMinAtackRange.AddRange(adjectedSquarsInCurrentRange);
            }

            _squaresInUnitAttackRange.AddRange(lastAdjectedSq);
        }

        _squaresInUnitAttackRange.Add(centerSquar);

        if (attackMinRange != 0 && attackMinRange < attackMaxRange)
        {
            _squaresInUnitAttackRange.Remove(centerSquar);
            centerSquar.isInAttackReach = false;

            foreach (Squar squar in squaresToMinAtackRange)
            {
                _squaresInUnitAttackRange.Remove(squar);
                squar.isInAttackReach = false;
            }
        }
    }

    private void ShowSquaresWithinRange(bool makeVisible)
    {
        foreach (Squar sq in _squaresInUnitRange)
        {
            if (sq.unitInSquar == null)
            {
                sq.inRangeBackground.SetActive(makeVisible);
            }
        }
    }

    private void ShowSquaresWithingAttackRange()
    {
        foreach (Squar squ in _squaresInUnitAttackRange)
        {
            GetTheAdjacentAttackSquare(squ, true);
        }
    }

    private List<Squar> GetTheAdjacentSquare(Squar centerSquar) 
    {
        List<Squar> checkedSquars = new List<Squar>();

        Squar rightSquare = null;
        Squar leftSquare = null;
        Squar upSquare = null;
        Squar downSquare = null;

        // check up direction
        if (centerSquar.xCoordinate + 1 >= _rowsCount)
            upSquare = null;
        else
            upSquare = GetSquareFromGrid(centerSquar.xCoordinate + 1, centerSquar.yCoordinate);
   
        // check down direction
        if (centerSquar.xCoordinate - 1 < 0)
            downSquare = null;
        else
            downSquare = GetSquareFromGrid(centerSquar.xCoordinate - 1, centerSquar.yCoordinate);

        // check right direction
        if (centerSquar.yCoordinate + 1 >= _collumCount)
            rightSquare = null;
        else
            rightSquare = GetSquareFromGrid(centerSquar.xCoordinate, centerSquar.yCoordinate + 1);

        // check left direction
        if (centerSquar.yCoordinate - 1 < 0)
            leftSquare = null;
        else
            leftSquare = GetSquareFromGrid(centerSquar.xCoordinate, centerSquar.yCoordinate - 1);

        if (rightSquare != null && !rightSquare.isInMoveRange)
            checkedSquars.Add(rightSquare);
        if (leftSquare != null && !leftSquare.isInMoveRange)
            checkedSquars.Add(leftSquare);
        if (upSquare != null && !upSquare.isInMoveRange)
            checkedSquars.Add(upSquare);
        if (downSquare != null && !downSquare.isInMoveRange)
            checkedSquars.Add(downSquare);

        centerSquar.isInMoveRange = true;

        foreach (Squar sq in checkedSquars)
        {
            sq.isInMoveRange = true;
        }

        return checkedSquars;
    }

    private List<Squar> GetTheAdjacentAttackSquare(Squar centerSquar, bool searchForBorders = false)
    {
        List<Squar> checkedSquars = new List<Squar>();

        Squar rightSquare = null;
        Squar leftSquare = null;
        Squar upSquare = null;
        Squar downSquare = null;

        // check up direction
        if (centerSquar.xCoordinate + 1 >= _rowsCount)
            upSquare = null;
        else
            upSquare = GetSquareFromGrid(centerSquar.xCoordinate + 1, centerSquar.yCoordinate);

        // check down direction
        if (centerSquar.xCoordinate - 1 < 0)
            downSquare = null;
        else
            downSquare = GetSquareFromGrid(centerSquar.xCoordinate - 1, centerSquar.yCoordinate);

        // check right direction
        if (centerSquar.yCoordinate + 1 >= _collumCount)
            rightSquare = null;
        else
            rightSquare = GetSquareFromGrid(centerSquar.xCoordinate, centerSquar.yCoordinate + 1);

        // check left direction
        if (centerSquar.yCoordinate - 1 < 0)
            leftSquare = null;
        else
            leftSquare = GetSquareFromGrid(centerSquar.xCoordinate, centerSquar.yCoordinate - 1);

        if(searchForBorders)
        {
            if (leftSquare == null || !leftSquare.isInAttackReach)
                centerSquar.leftBorder.SetActive(true);

            if (rightSquare == null || !rightSquare.isInAttackReach)
                centerSquar.rightBorder.SetActive(true);

            if (downSquare == null || !downSquare.isInAttackReach)
                centerSquar.downBorder.SetActive(true);

            if (upSquare == null || !upSquare.isInAttackReach)
                centerSquar.upBorder.SetActive(true);
        }
        else
        {
            if (rightSquare != null && !rightSquare.isInAttackReach)
                checkedSquars.Add(rightSquare);
            if (leftSquare != null && !leftSquare.isInAttackReach)
                checkedSquars.Add(leftSquare);
            if (upSquare != null && !upSquare.isInAttackReach)
                checkedSquars.Add(upSquare);
            if (downSquare != null && !downSquare.isInAttackReach)
                checkedSquars.Add(downSquare);

            centerSquar.isInAttackReach = true;

            foreach (Squar sq in checkedSquars)
            {
                sq.isInAttackReach = true;
            }
        }

        return checkedSquars;
    }

    // Get Cross Squares (maybe will be useable)
    private List<Squar> GetTheAdjacentCrossSquare(Squar centerSquar)
    {
        List<Squar> checkedSquars = new List<Squar>();
        centerSquar.isInAttackReach = true;

        Squar rightCrossSquare = null;
        Squar leftCrossSquare = null;
        Squar upCrossSquare = null;
        Squar downCrossSquare = null;

        // check upLeft direction
        if (centerSquar.xCoordinate + 1 >= _rowsCount || centerSquar.yCoordinate - 1 < 0)
            upCrossSquare = null;
        else
            upCrossSquare = GetSquareFromGrid(centerSquar.xCoordinate + 1, centerSquar.yCoordinate - 1);

        // check downLeft direction
        if (centerSquar.xCoordinate - 1 < 0 || centerSquar.yCoordinate - 1 < 0)
            downCrossSquare = null;
        else
            downCrossSquare = GetSquareFromGrid(centerSquar.xCoordinate - 1, centerSquar.yCoordinate - 1);

        // check upRight direction
        if (centerSquar.xCoordinate + 1 >= _rowsCount || centerSquar.yCoordinate + 1 >= _collumCount)
            rightCrossSquare = null;
        else
            rightCrossSquare = GetSquareFromGrid(centerSquar.xCoordinate + 1, centerSquar.yCoordinate + 1);

        // check downRight direction
        if (centerSquar.xCoordinate - 1 < 0 || centerSquar.yCoordinate + 1 >= _collumCount)
            leftCrossSquare = null;
        else
            leftCrossSquare = GetSquareFromGrid(centerSquar.xCoordinate - 1, centerSquar.yCoordinate + 1);

        if (rightCrossSquare != null && !rightCrossSquare.isInAttackReach)
            checkedSquars.Add(rightCrossSquare);
        if (leftCrossSquare != null && !leftCrossSquare.isInAttackReach)
            checkedSquars.Add(leftCrossSquare);
        if (upCrossSquare != null && !upCrossSquare.isInAttackReach)
            checkedSquars.Add(upCrossSquare);
        if (downCrossSquare != null && !downCrossSquare.isInAttackReach)
            checkedSquars.Add(downCrossSquare);

        foreach (Squar sq in checkedSquars)
        {
            sq.isInAttackReach = true;
        }

        return checkedSquars;
    }

    private Squar GetSquareFromGrid(int x , int y)
    {
        Squar sq = _squaresInBattleField[x, y];
        return sq;
    }

    private void SetSquaresOutOfMoveRange()
    {
        foreach (Squar squar in _squaresInUnitRange)
        {
            squar.inRangeBackground.SetActive(false);
            squar.isInMoveRange = false;
        }
    }

    private void SetSquaresOutOfAttackReach()
    {
        foreach (Squar item in _squaresInUnitAttackRange)
        {
            item.DisableAttackBorders();
            item.isInAttackReach = false;
        }
    }

    private void SetCursor(Squar sq)
    {
        sq.CursorEvent.canAttack = false;
        sq.CursorEvent.isInMoveRange = false;

        if (sq.unitInSquar is null)
        {
            if (_squaresInUnitRange.Contains(sq))
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
        unitsOnBattleField.Sort((x, y) => y._iniciation.CompareTo(x._iniciation));
    }

    private void UpdateRightPanel (Squar sq)
    {
        Unit unit = sq.unitInSquar;

        if (unit != null)
        {
            rightUnitInfo.UpdateStats(unit);
            rightUnitInfo.gameObject.SetActive(true);

        }
        //else
        //{
        //   // rightUnitInfo.DisablePanel();
        //}
    }

    enum BattleAction 
    {
        None,
        Move,
        Attack,
        Heal
    }

    public class AttackInfo
    {
        public int dices = 0;
        public int success = 0;

        public List<int> dicesValueRoll = new List<int>();      
    }



}
