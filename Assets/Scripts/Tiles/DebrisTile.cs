using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = System.Object;

public class DebrisTile : Tile, IWorkSource
{
    public float hp;
    private List<Worker> Workers;
    private ResourceManager resourceManager;
    private TileFactory tileFactory;
    private const int MaxNumberOfWorkers = 4;
    private WorkManager workManager;
    private bool depleted = false;

    public DebrisTile(GameObject tile, int x, int y) : base(tile, x, y)
    {
        walkthrough = false;
        hp = 100;
        Workers = new List<Worker>();
        resourceManager = GameObject.FindGameObjectWithTag("ResourceManager").transform.GetComponent<ResourceManager>();
        tileFactory = GameObject.FindGameObjectWithTag("TileFactory").transform.GetComponent<TileFactory>();
        workManager = GameObject.FindGameObjectWithTag("WorkManager").transform.GetComponent<WorkManager>();

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
                    }
                    break;
                case WorkerState.working:
                    //do some proper calculation
                    hp -= Time.deltaTime * 10;
                    if (hp <= 0)
                    {
                        //done, we're depleted
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
