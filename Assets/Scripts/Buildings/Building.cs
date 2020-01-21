using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Building
{

    private List<Worker> Workers;
    private BuildingState state;
    private ResourceManager resourceManager;
    private TileFactory tileFactory;
    private readonly BuildingBlueprint blueprint;
    private GameObject prefab;
    private BuildingState State
    {
        get => state;
        set { 
            state = value;
            OnStateChanged();
        }
    }

    enum BuildingState
    {
        Designed,
        UnderConstruction,
        Build
        //upgrading? destroyed? - pro kazdy stav by mela byt vlastni grafika
    };

    enum WorkerState
    {
        init,
        empty,
        pickup,
        full,
        drop
    }
    class Worker
    {
        public Character character;
        public WorkerState state;
    }
    public Building(BuildingBlueprint blueprint, GameObject prefab)
    {
        this.blueprint = blueprint;
        this.prefab = prefab; //this is ugly hack just to get selected position easily- the prefab is reInstantiated later
        State = BuildingState.Designed;
        Workers = new List<Worker>();
        resourceManager =  GameObject.FindGameObjectWithTag("ResourceManager").transform.GetComponent<ResourceManager>();
        tileFactory = GameObject.FindGameObjectWithTag("TileFactory").transform.GetComponent<TileFactory>();

    }

    public void AddWorker(Character character)
    {
        var worker = new Worker();
        worker.character = character;
        worker.state = WorkerState.init;
        Workers.Add(worker);
    }
    public void Update()
    {
        foreach (var worker in Workers.ToList())
        {
            var activeWorker = worker;
            switch (activeWorker.state)
            {
                case WorkerState.init:
                    var charPosition = Geometry.GridFromPoint(activeWorker.character.transform.position);
                    var tile = resourceManager.Nearest(charPosition, ResourceManager.Material.Civilni);
                    if (tile == null)
                    {
                        Workers.Remove(worker);
                        break;
                    }
                    var nearestRes = (Tile)tile.Owner; //TODO pro vsechny matrose
                    var nearest = new Vector2Int(nearestRes.x, nearestRes.y);
                    var pathToMaterial = tileFactory.FindPath(charPosition, nearest);
                   
                    activeWorker.character.AddCommand(new Move(activeWorker.character.gameObject, pathToMaterial));
                    activeWorker.state = WorkerState.empty;
                    break;
                case WorkerState.empty:
                    if (activeWorker.character.Execute() == Result.Success)
                    {
                        activeWorker.character.AddCommand(new PickUp(activeWorker.character.gameObject));
                        activeWorker.state = WorkerState.pickup;
                    }
                    break;
                case WorkerState.pickup:
                    var result = activeWorker.character.Execute();
                    if (result == Result.Failure)
                    {
                        activeWorker.state = WorkerState.init;
                    }
                    else
                    {
                        var buildingPosition = Geometry.GridFromPoint(this.prefab.transform.position);
                        var pathFromMatToBuilding = tileFactory.FindPath(Geometry.GridFromPoint(activeWorker.character.transform.position), buildingPosition);
                        activeWorker.character.AddCommand(new Move(activeWorker.character.gameObject, pathFromMatToBuilding));
                        activeWorker.state = WorkerState.full;
                    }
                    break;
                case WorkerState.full:
                    if (activeWorker.character.Execute() == Result.Success)
                    {
                        activeWorker.character.AddCommand(new Drop(activeWorker.character.gameObject));
                        activeWorker.state = WorkerState.drop;
                    }
                    break;
                case WorkerState.drop:
                    activeWorker.character.Execute();
                    activeWorker.state = WorkerState.init;
                    break;
            }
        }
    }

    private void OnStateChanged()
    {
        if (prefab != null)
        {
            Object.Destroy(prefab);
        }
        switch (state)
        {
            case BuildingState.Designed: prefab = Object.Instantiate(blueprint.ConstructionPrefab, prefab.transform.position, prefab.transform.rotation);
                prefab.transform.Find("background").GetComponent<Renderer>().material.color = blueprint.BackgroundColor;
                break;
            default: break; //TODO
        }
    }

    public Vector3 GetPosition()
    {
        var position = prefab.transform.position;
        return new Vector3(position.x + Random.Range(-0.4f, 0.4f), position.y + 0.5f,  position.z + Random.Range(-0.4f,0.4f));
    }
}