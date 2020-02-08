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
    private float timeToBuildRemaining;

    private BuildingState State
    {
        get => state;
        set
        {
            if (value == state) return;
            state = value;
            OnStateChanged();
        }
    }

    enum BuildingState
    {
        Init,
        Designed,
        UnderConstruction,
        Build
        //upgrading? destroyed? - pro kazdy stav by mela byt vlastni grafika
    };

    enum WorkerState
    {
        init,
        wait,
        empty,
        pickup,
        full,
        drop,
        building
    }

    class Worker
    {
        public Character character;
        public WorkerState state;
        public float time;
    }

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

    public void AddWorker(Character character)
    {
        var worker = new Worker();
        worker.character = character;
        worker.state = WorkerState.init;
        Workers.Add(worker);
    }

    private static int debug = 0;

    public void Update()
    {
        debug++;
        var workerNr = 0;
        if (state == BuildingState.Designed)
        {
            foreach (var worker in Workers.ToList())
            {
                workerNr++;
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
                            activeWorker.state = WorkerState.building;
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
                            var pathFromMatToBuilding = tileFactory.FindPath(
                                Geometry.GridFromPoint(activeWorker.character.transform.position), buildingPosition);
                            activeWorker.character.AddCommand(new Move(activeWorker.character.gameObject,
                                pathFromMatToBuilding));
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
                    case WorkerState.building:
                        //TODO nejaka logika na zapocitavani prace od workeru, zatim jen casove
                        activeWorker.character.Execute();
                        float buildPoints = 0;
                        if (activeWorker.character.GetCommand() is Build buildCmd)
                        {
                            buildPoints = buildCmd.GetBuildPoints(activeWorker.character.GetTechLevel());
                            timeToBuildRemaining -= buildPoints;
                        }

                        if (debug % 100 == 0)
                        {
                            Debug.Log($"Worker {workerNr}: Time remaining: {timeToBuildRemaining} buildpoints : {buildPoints}");

                        }
                        
                        if (timeToBuildRemaining <= 0) 
                        {
                            Workers.Remove(worker);
                            State = BuildingState.Build;
                        }
                        break;
                }
            }
        }
    }

    private List<ResourceManager.Material> GetMissingMaterials()
    {
        var missing = new List<ResourceManager.Material>();
        var onSiteResources = resourceManager.GetResourcesForOwner(this);
        var civil = blueprint.Civil - onSiteResources.Where(r=>r.Material == ResourceManager.Material.Civilni).Select(r => r.Amount).Sum();
        if (civil > 0) missing.Add(ResourceManager.Material.Civilni);
        var military = blueprint.Military - onSiteResources.Where(res => res.Material == ResourceManager.Material.Vojensky).Select(r => r.Amount).Sum();
        if (military > 0) missing.Add(ResourceManager.Material.Vojensky);
        var technicky = blueprint.Tech - onSiteResources.Where(res => res.Material == ResourceManager.Material.Technicky).Select(r => r.Amount).Sum();
        if (technicky > 0) missing.Add(ResourceManager.Material.Technicky);
        return missing;
    }

    private void OnStateChanged()
    {
        if (prefab != null)
        {
            Object.Destroy(prefab);
        }
        
        switch (state)
        {
            case BuildingState.Designed:
                prefab = Object.Instantiate(blueprint.ConstructionPrefab, prefab.transform.position,
                    prefab.transform.rotation);
                prefab.transform.Find("background").GetComponent<Renderer>().material.color = blueprint.BackgroundColor;
                break;
            case BuildingState.Build:
                resourceManager.GetResourcesForOwner(this).ForEach(res=>res.Amount = 0);
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