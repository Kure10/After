using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Resources;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public partial class Building : IWorkSource, IResourceHolder
{
    private List<Worker> Workers;
    private BuildingState _state;
    private ResourceManager resourceManager;
    private TileFactory tileFactory;
    public readonly BuildingBlueprint blueprint;
    private GameObject prefab;
    public float TimeToBuildRemaining;
    private HealthbarHandle statusHandle;
    private List<Resource> resources;

    public List<Worker> getWorkers()
    {
        return Workers;
    }

    public BuildingState State
    {
        get => _state;
        set
        {
            if (value == _state) return;
            _state = value;
            OnStateChanged();
        }
    }

    public enum BuildingState: int
    {
        Init = 0,
        Designed = 1,
        UnderConstruction = 2,
        Build = 3
        //upgrading? destroyed? - pro kazdy stav by mela byt vlastni grafika
    };



    public Building(BuildingBlueprint blueprint, GameObject prefab)
    {
        Workers = new List<Worker>();
        resources = new List<Resource>();
        _amount = new ResourceManager.ResourceAmount();
        Amount = new ResourceManager.ResourceAmount();
        this.blueprint = blueprint;
        this.prefab = prefab; //this is ugly hack just to get selected position easily- the prefab is reInstantiated later
        State = BuildingState.Designed;
        TimeToBuildRemaining = blueprint.TimeToBuild;
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
                        var missingMaterials = GetMissingResources();
                        if (missingMaterials.Empty())
                        {
                            activeWorker.time = 0;
                            activeWorker.character.AddCommand(new Build());
                            activeWorker.state = WorkerState.working;
                            return;
                        }

                        var tile = resourceManager.Nearest(charPosition, missingMaterials);

                        if (tile == Vector2Int.Max(default, default))
                        {
                            //no available material, wait for some to appear in future
                            activeWorker.time = 0;
                            activeWorker.state = WorkerState.wait;
                            activeWorker.character.State = "Looking for material";
                            //Workers.Remove(worker);
                            break;
                        }

                        var pathToMaterial = tileFactory.FindPath(charPosition, tile);

                        activeWorker.character.AddCommand(new Move(activeWorker.character.gameObject, pathToMaterial, true));
                        activeWorker.state = WorkerState.empty;
                        activeWorker.character.State = "Getting material";
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
                        activeWorker.character.State = "Walking";
                        if (GetMissingResources().Empty())
                        {
                            State = BuildingState.UnderConstruction;
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
                            TimeToBuildRemaining -= buildPoints;
                            worker.character.State = "Building";
                        }
                        if (debug % 100 == 0)
                        {
                            Debug.Log(
                                $"Worker {workerNr}: Time remaining: {TimeToBuildRemaining} buildpoints : {buildPoints}");
                        }
                        statusHandle.SetHPValue(1 - (TimeToBuildRemaining / blueprint.TimeToBuild));
                        if (TimeToBuildRemaining <= 0)
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
                        worker.character.State = "Moving";
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

    private ResourceManager.ResourceAmount GetMissingResources()
    {
        var missing = new ResourceManager.ResourceAmount();
        missing.Civilian = blueprint.Civil - Amount.Civilian;
        missing.Military = blueprint.Military - Amount.Military;
        missing.Technical = blueprint.Tech - Amount.Technical;
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
                prefab.transform.Find("Selection").GetComponent<BuildingPointer>().Building = this;
                Amount = new ResourceManager.ResourceAmount();
                break;
            case BuildingState.UnderConstruction:
                prefab = Object.Instantiate(blueprint.ConstructionPrefab, prefab.transform.position,
                    prefab.transform.rotation);
                Set(new ResourceManager.ResourceAmount());
                resourceManager.ResourceAmountChanged();
                statusHandle = prefab.GetComponent<HealthbarHandle>();
                statusHandle.SetHPValue(0);
                prefab.transform.Find("Selection").GetComponent<BuildingPointer>().Building = this;
                break;
            case BuildingState.Build:
                Object.Destroy(prefab);
                prefab = Object.Instantiate(blueprint.Prefab, prefab.transform.position, prefab.transform.rotation);
                prefab.transform.Find("Selection").GetComponent<BuildingPointer>().Building = this;
                break;
            default: break; //TODO
        }
    }

    private int nextField = 0;
    public Vector3 GetPosition(int field = 0)
    {
        var maxField = blueprint.row * blueprint.column;
        if (field == 0)
        {
            nextField = resources.Count % maxField;
        }
        else
        {
            nextField = field;
        }
        var position = prefab.transform.position;
        int rotation = (int) (prefab.transform.rotation.eulerAngles.y / 90);
        //x = row, y = column
        var x = nextField % blueprint.row;
        var y = nextField % blueprint.column;
        float xx, zz;
        switch (rotation)
                {
                    case 0: xx = position.x + x; zz = position.z + y; break;
                    case 1: xx = position.x + y; zz = position.z - x; break;
                    case 2: xx = position.x - x; zz = position.z - y; break;
                    default: xx = position.x - y; zz = position.z + x; break;
                }

        return new Vector3( xx, position.y + 0.5f,
            zz);
    }

    public void RedrawResources()
    {
        foreach (var resource in resources)
        {
            resource.Amount = 0;
        }
        resources.Clear();
        var remaining = Amount;
        while (remaining.Civilian > 0)
        {
            var toStack = remaining.Civilian > 10 ? 10 : remaining.Civilian;
            resources.Add(new Resource(toStack, ResourceManager.Material.Civilni, this));
            remaining.Civilian -= toStack;
        }
        while (remaining.Technical> 0)
        {
            var toStack = remaining.Technical > 10 ? 10 : remaining.Technical;
            resources.Add(new Resource(toStack, ResourceManager.Material.Technicky, this));
            remaining.Technical -= toStack;
        }
    }

    private ResourceManager.ResourceAmount _amount;

    public ResourceManager.ResourceAmount Amount
    {
        get => _amount;
        set
        {
            _amount = value;
            RedrawResources(); 
        }
    }

    public void Set(ResourceManager.ResourceAmount amount)
    {
        Amount = amount;
    }

    public ResourceManager.ResourceAmount Add(ResourceManager.ResourceAmount amount)
    {
        Amount += amount;
        resourceManager.Register(this);
        return new ResourceManager.ResourceAmount();
    }

    public ResourceManager.ResourceAmount Remove(ResourceManager.ResourceAmount amount)
    {
        Amount -= amount;
        if (Amount.Empty())
        {
            resourceManager.Unregister(this);
        }
        return amount;
    }
}