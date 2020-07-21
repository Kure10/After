using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public partial class Building : IWorkSource
{
    private List<Worker> Workers;
    private BuildingState _state;
    private ResourceManager resourceManager;
    private TileFactory tileFactory;
    private readonly BuildingBlueprint blueprint;
    private GameObject prefab;
    private float timeToBuildRemaining;
    private HealthbarHandle statusHandle;

    private BuildingState State
    {
        get => _state;
        set
        {
            if (value == _state) return;
            _state = value;
            OnStateChanged();
        }
    }

    enum BuildingState: int
    {
        Init = 0,
        Designed = 1,
        UnderConstruction = 2,
        Build = 3
        //upgrading? destroyed? - pro kazdy stav by mela byt vlastni grafika
    };



    public Building(BuildingBlueprint blueprint, GameObject prefab)
    {
        this.blueprint = blueprint;
        this.prefab =
            prefab; //this is ugly hack just to get selected position easily- the prefab is reInstantiated later
        State = BuildingState.Designed;
        timeToBuildRemaining = blueprint.TimeToBuild;
        Workers = new List<Worker>();
        resourceManager = GameObject.FindGameObjectWithTag("ResourceManager").transform.GetComponent<ResourceManager>();
        tileFactory = GameObject.FindGameObjectWithTag("TileFactory").transform.GetComponent<TileFactory>();

    }


    public bool Register(Character character)
    {
        Unregister(character);
        var worker = new Worker();
        worker.character = character;
        worker.state = WorkerState.init;
        Workers.Add(worker);
        return true;
    }

    public void Unregister(Character character)
    {
        if (Workers.Select(w => w.character).Contains(character))
        {
            foreach (var w in Workers.ToList())
            {
                if (w.character == character)
                {
                    if (w.state == WorkerState.full)
                    {
                        w.character.AddCommand(new Drop(character.gameObject));
                        w.character.Execute();
                    }
                    Workers.Remove(w);
                }
            }
        }
    }

    private static int debug = 0;

  
    public void Update()
    {
        debug++;
        var workerNr = 0;
        if (State == BuildingState.Designed)
        {
            foreach (var worker in Workers.ToList())
            {
                var activeWorker = worker;
                switch (activeWorker.state)
                {
                    case WorkerState.wait:
                        activeWorker.time += Time.deltaTime;
                        if (activeWorker.time > 1f)
                        {
                            activeWorker.state = WorkerState.init;
                        }

                        break;
                    case WorkerState.init:
                        var charPosition = Geometry.GridFromPoint(activeWorker.character.transform.position);
                        var missingMaterials = GetMissingMaterials();
                        if (!missingMaterials.Any())
                        {
                            activeWorker.time = 0;
                            activeWorker.character.AddCommand(new Build());
                            activeWorker.state = WorkerState.working;
                            return;
                        }

                        Resource tile = null;
                        foreach (var missingMaterial in missingMaterials)
                        {
                            tile = resourceManager.Nearest(charPosition, missingMaterial);
                            if (tile != null) break;
                        }

                        if (tile == null)
                        {
                            //no available material, wait for some to appear in future
                            activeWorker.time = 0;
                            activeWorker.state = WorkerState.wait;
                            //Workers.Remove(worker);
                            break;
                        }

                        var nearestRes = (Tile) tile.Owner; //TODO pro vsechny matrose
                        var nearest = new Vector2Int(nearestRes.x, nearestRes.y);
                        var pathToMaterial = tileFactory.FindPath(charPosition, nearest);

                        activeWorker.character.AddCommand(new Move(activeWorker.character.gameObject, pathToMaterial, true));
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
                            MoveBack(activeWorker);
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
                        if (!GetMissingMaterials().Any())
                        {
                            State = BuildingState.UnderConstruction;
                            var hpPosition = Camera.main.WorldToScreenPoint(prefab.transform.position);
                            var canvas = GameObject.FindGameObjectWithTag("Canvas").transform.GetComponent<Canvas>();
                            statusHandle.SetHPValue(0);
                        }

                        break;
                }
            }
        }
        else if (State == BuildingState.UnderConstruction)
        {
            foreach (var worker in Workers.ToList())
            {
                workerNr++;
                switch (worker.state)
                {
                    case WorkerState.working:
                        worker.character.Execute();
                        float buildPoints = 0;
                        if (worker.character.GetCommand() is Build buildCmd)
                        {
                            buildPoints = buildCmd.GetBuildPoints(worker.character.GetTechLevel());
                            timeToBuildRemaining -= buildPoints;
                        }
                        if (debug % 100 == 0)
                        {
                            Debug.Log(
                                $"Worker {workerNr}: Time remaining: {timeToBuildRemaining} buildpoints : {buildPoints}");
                        }
                        statusHandle.SetHPValue(1 - (timeToBuildRemaining / blueprint.TimeToBuild));
                        if (timeToBuildRemaining <= 0)
                        {
                            State = BuildingState.Build;
                            Workers.Clear();
                        }
                        break;
                    case WorkerState.full:
                        worker.character.AddCommand(new Drop(worker.character.gameObject));
                        worker.state = WorkerState.drop;
                        break;
                    case WorkerState.drop:
                        worker.character.Execute();
                        worker.state = WorkerState.init;
                        break;
                    case WorkerState.init:
                        MoveBack(worker);
                        worker.state = WorkerState.move;
                        break;
                    case WorkerState.move:
                        if (worker.character.Execute() == Result.Success)
                        {
                            worker.state = WorkerState.working;
                            worker.character.AddCommand(new Build());
                        }
                        break;
                    default: worker.state = WorkerState.init;
                        break;
                }
            }
        }
    }

    private void MoveBack(Worker activeWorker)
    {
        var buildingPosition = Geometry.GridFromPoint(this.prefab.transform.position);
        var pathFromMatToBuilding = tileFactory.FindPath(
            Geometry.GridFromPoint(activeWorker.character.transform.position), buildingPosition);
        activeWorker.character.AddCommand(new Move(activeWorker.character.gameObject,
            pathFromMatToBuilding));
    }

    private List<ResourceManager.Material> GetMissingMaterials()
    {
        var missing = new List<ResourceManager.Material>();
        var onSiteResources = resourceManager.GetResourcesForOwner(this);
        var civil = blueprint.Civil - onSiteResources.Where(r => r.Material == ResourceManager.Material.Civilni)
                        .Select(r => r.Amount).Sum();
        if (civil > 0) missing.Add(ResourceManager.Material.Civilni);
        var military = blueprint.Military - onSiteResources
                           .Where(res => res.Material == ResourceManager.Material.Vojensky).Select(r => r.Amount).Sum();
        if (military > 0) missing.Add(ResourceManager.Material.Vojensky);
        var technicky = blueprint.Tech - onSiteResources
                            .Where(res => res.Material == ResourceManager.Material.Technicky).Select(r => r.Amount)
                            .Sum();
        if (technicky > 0) missing.Add(ResourceManager.Material.Technicky);
        return missing;
    }

    private void OnStateChanged()
    {
        if (prefab != null)
        {
            Object.Destroy(prefab);
        }

        switch (State)
        {
            case BuildingState.Designed:
                prefab = Object.Instantiate(blueprint.ConstructionPrefab, prefab.transform.position,
                    prefab.transform.rotation);
                prefab.transform.Find("background").GetComponent<Renderer>().material.color = blueprint.BackgroundColor;
                statusHandle = prefab.GetComponent<HealthbarHandle>();
                statusHandle.SetHPValue(0);
                break;
            case BuildingState.UnderConstruction:
                prefab = Object.Instantiate(blueprint.ConstructionPrefab, prefab.transform.position,
                    prefab.transform.rotation);
                resourceManager.GetResourcesForOwner(this).ForEach(res => res.Amount = 0);
                statusHandle = prefab.GetComponent<HealthbarHandle>();
                statusHandle.SetHPValue(0);
                break;
            case BuildingState.Build:
                Object.Destroy(prefab);
                prefab = Object.Instantiate(blueprint.Prefab, prefab.transform.position, prefab.transform.rotation);
                break;
            default: break; //TODO
        }
    }

    public Vector3 GetPosition()
    {
        var position = prefab.transform.position;
        return new Vector3(position.x + Random.Range(-0.4f, 0.4f), position.y + 0.5f,
            position.z + Random.Range(-0.4f, 0.4f));
    }
}