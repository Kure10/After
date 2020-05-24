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
        hpHandle.SetHPValue(1);
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
        foreach (var worker in Workers)
        {
            if (depleted) break;
            switch (worker.state)
            {
                case WorkerState.init:
                    if (worker.character.Execute() == Result.Success)
                    {
                        worker.state = WorkerState.working;
                        worker.character.AddCommand(new Build());
                    }
                    break;
                case WorkerState.working:
                    worker.character.Execute();
                    float buildPoints = 0;
                    if (worker.character.GetCommand() is Build buildCmd)
                    {
                        buildPoints = buildCmd.GetBuildPoints(worker.character.GetTechLevel());
                    }
                    if (DoDamage(buildPoints))
                    {
                        //done, we're depleted
                        var amount = rnd.Next(5, 10);
                        resourceManager.SpawnMaterial(amount % 2 == 0 ? ResourceManager.Material.Civilni : ResourceManager.Material.Technicky, amount, Geometry.GridFromPoint(worker.character.transform.position));
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
            var path = tileFactory.FindPath(Geometry.GridFromPoint(who.gameObject.transform.position), Geometry.GridPoint(x, y));
            if (path is null || path.Count == 0) return false;
            path.Remove(path.Last());
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
