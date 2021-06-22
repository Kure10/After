using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using Object = System.Object;
using Random = System.Random;

public class DebrisTile : Tile, IWorkSource
{
    private float hp;
    private List<Worker> Workers;
    private ResourceManager resourceManager;
    private TileFactory tileFactory;
    private const int MaxNumberOfWorkers = 4;
    private WorkManager workManager;
    private bool depleted = false;
    //private GameObject healthbar;
    private HealthbarHandle hpHandle;
    private const float TOTALHP = 40000f;
    private Random rnd;
    public DebrisTile(GameObject tile, int x, int y) : base(tile, x, y)
    {
        walkthrough = false;
        Workers = new List<Worker>();
        resourceManager = GameObject.FindGameObjectWithTag("ResourceManager").transform.GetComponent<ResourceManager>();
        tileFactory = GameObject.FindGameObjectWithTag("TileFactory").transform.GetComponent<TileFactory>();
        workManager = GameObject.FindGameObjectWithTag("WorkManager").transform.GetComponent<WorkManager>();
        var canvas = GameObject.FindGameObjectWithTag("Canvas").transform.GetComponent<Canvas>();
        //healthbar = UnityEngine.Object.Instantiate(tileFactory.DebrisHealthbar, hpPosition, Quaternion.identity, canvas.transform);
        hpHandle = tile.GetComponent<HealthbarHandle>();
        hp = TOTALHP;
        hpHandle.SetHPValue(0);
        rnd = new Random();
    }

    private bool DoDamage(float dmg)
    {
        hp -= dmg;
        hpHandle.SetHPValue(hp / TOTALHP);
        return hp <= 0;
    }
    public void Update()
    {
        foreach (var worker in Workers.ToList())
        {
            if (depleted) break;
            switch (worker.state)
            {
                case WorkerState.init:
                    worker.character.State = "Going to remove debris";
                    if (worker.character.Execute() == Result.Success)
                    {
                        //check if on allowed tile (could ended elsewhere if the tile got occupied)
                        var charPlace = Geometry.GridFromPoint(worker.character.transform.position);
                        tileFactory.LeaveTile(charPlace);
                        if (!GetMiningSpots().Contains(charPlace))
                        {
                            //try again
                            Unregister(worker.character);
                            Register(worker.character);
                            continue;
                        }
                        tileFactory.OccupyTile(charPlace);
                        worker.state = WorkerState.working;
                        worker.character.State = "Clearing debris";
                        worker.character.AddCommand(new Build());
                    }
                    break;
                case WorkerState.resting:
                    worker.character.Execute();
                    if (worker.character.GetCommand() is Build bCmd)
                    {
                        var stamina = bCmd.GetBuildPoints(worker.character.Stats.tech);
                        var remaining = worker.character.ModifyStamina(stamina);
                        if (remaining > stamina * 1000) //TODO how much stamina is gained and when to start again?
                        {
                            worker.state = WorkerState.working;
                            worker.character.State = "Clearing debris";
                        }
                    }
                    break;
                case WorkerState.working:
                    worker.character.Execute();
                    float buildPoints = 0;
                    if (worker.character.GetCommand() is Build buildCmd)
                    {
                        buildPoints = buildCmd.GetBuildPoints(worker.character.Stats.tech);
                        var remaining = worker.character.ModifyStamina(-buildPoints);
                        if (remaining < 1)
                        {
                            worker.state = WorkerState.resting;
                            worker.character.State = "Resting";
                        }
                    }
                    if (DoDamage(buildPoints))
                    {
                        //done, we're depleted
                        var amount = rnd.Next(5, 10);
                        var res = new ResourceManager.ResourceAmount();
                        if (amount % 2 == 0)
                        {
                            res.Civilian += amount;
                        }
                        else
                        {
                            res.Technical += amount;
                        }
                        resourceManager.SpawnResource(res, Geometry.GridFromPoint(worker.character.transform.position));
                        depleted = true;
                        break;
                    }
                    break;
                default:
                    break;

            }
        }

        if (depleted)
        {
            workManager.Unregister(this);
            Workers.Clear();
            tileFactory.ClearDebris(x, y);
            GameObject.Destroy(tile);
        }
    }
    
    
    

    public bool Register(Character who)
    {
        if (Workers.Count < MaxNumberOfWorkers)
        {
            var charPlace = Geometry.GridFromPoint(who.transform.position);
            tileFactory.LeaveTile(charPlace);
            var possiblePlaces = GetMiningSpots();
            var path = tileFactory.FindPath(Geometry.GridFromPoint(who.gameObject.transform.position),
                possiblePlaces.First());
            foreach (var possiblePlace in possiblePlaces)
            {
                var shorter = tileFactory.FindPath(Geometry.GridFromPoint(who.gameObject.transform.position),
                    possiblePlace);
                if (shorter is null) continue;
                if (path is null) path = shorter;
                if (shorter.Count < path.Count)
                {
                    path = shorter;
                }
            }
            tileFactory.OccupyTile(charPlace);

            if (path is null)
            {
                return false;
            }
            //path.Remove(path.Last());
            Unregister(who);
            var worker = new Worker();
            worker.character = who;
            worker.state = WorkerState.init;
            worker.character.AddCommand(new Move(worker.character.gameObject, path));
            Workers.Add(worker);

            if (Workers.Count == 1)
            {
                workManager.Register(this);
            }
            return true;
        }
        return false;
    }

    private List<Vector2Int> GetMiningSpots()
    {
        var allowedSpots = new List<Vector2Int>() {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};
        var freeSpots = new List<Vector2Int>();
        var center = new Vector2Int(x, y);
        foreach (var where in allowedSpots)
        {
            //check if 1) is empty tile 2) no other character is there
            var candidate = center + where;
            if (!(tileFactory.getTile(candidate) is DebrisTile))
            {
                if (!tileFactory.IsOccupied(candidate)) freeSpots.Add(candidate);
            }
        }
        return freeSpots;
    }

    public void Unregister(Character character)
    {
        if (Workers.Select(w => w.character).Contains(character))
        {
            foreach (var w in Workers.ToList())
            {
                if (w.character == character)
                {
                    Workers.Remove(w);
                }
            }
        }
    }
}
