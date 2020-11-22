using System;
using System.Collections.Generic;
using System.Linq;
using Resources;
using UnityEngine;

namespace Buildings
{
    public class Warehouse : Building, IWorkSource, IResourceHolder
    {
        private List<Worker> workers;
        public static int MaxMaterials = 16;
        public Warehouse(BuildingBlueprint blueprint, GameObject prefab) : base(blueprint, prefab)
        {
            workers = new List<Worker>();
        }

        public new void Update()
        {
            if (State == BuildingState.Build)
            {
                foreach (var worker in workers.ToList())
                {
                    switch (worker.state)
                    {
                        case WorkerState.init:

                            if (resources.Count >= MaxMaterials)
                            {
                                worker.state = WorkerState.wait;
                                Debug.Log("Warehouse is full");
                                break;
                                
                            }
                            var charPosition = Geometry.GridFromPoint(worker.character.transform.position);
                        
                            var filter = new ResourceManager.ResourceAmount();
                            //for now take anything, allow to filter what kind of resources to look for in the future
                            filter.Civilian = 1;
                            filter.Food = 1;
                            filter.Fuel = 1;
                            filter.Military = 1;
                            filter.Technical = 1;
                            var tile = resourceManager.Nearest(charPosition, filter);
                            if (tile == Vector2Int.Max(default, default))
                            {
                                //nothing left, end
                                worker.state = WorkerState.wait;
                                break;
                            }
                            var pathToMaterial = tileFactory.FindPath(charPosition, tile);

                            worker.character.AddCommand(new Move(worker.character.gameObject, pathToMaterial, true));
                            worker.state = WorkerState.empty;
                            worker.character.State = "Getting material";
                            break;
                        case WorkerState.empty:
                            if (worker.character.Execute() == Result.Success)
                            {
                                worker.character.AddCommand(new PickUp(worker.character.gameObject));
                                worker.state = WorkerState.pickup;
                            }
                            break;
                    case WorkerState.pickup:
                        var result = worker.character.Execute();
                        if (result == Result.Failure)
                        {
                            worker.state = WorkerState.init;
                        }
                        else
                        {
                            MoveBack(worker);
                            worker.state = WorkerState.full;
                        }
                        break;
                    case WorkerState.full:
                        if (worker.character.Execute() == Result.Success)
                        {
                            worker.character.AddCommand(new Drop(worker.character.gameObject));
                            worker.state = WorkerState.drop;
                        }
                        break;
                    case WorkerState.drop:
                        worker.character.Execute();
                        worker.state = WorkerState.init;
                        break;
                    }
                }
                workers.RemoveAll(w => w.state == WorkerState.wait);
            }
            else
            {
                base.Update();
            }
        }

        public new bool Register(Character character)
        {
            if (State == BuildingState.Build)
            {
                if (workers.Count >= blueprint.row * blueprint.column) return false;
                Debug.Log("Warehouse - worker registered");
                Unregister(character);
                if (resources.Count >= Warehouse.MaxMaterials) return false;
                var worker = new Worker();
                worker.character = character;
                worker.state = WorkerState.init;
                workers.Add(worker);
                return true;
            }

            return base.Register(character);
        }

        public new void Unregister(Character character)
        {
            if (workers.Select(w => w.character).Contains(character))
            {
                foreach (var w in workers.ToList())
                {
                    if (w.character == character)
                    {
                        if (w.state == WorkerState.full)
                        {
                            w.character.AddCommand(new Drop(character.gameObject));
                            w.character.Execute();
                        }
                        workers.Remove(w);
                    }
                }
            }
        }
    public new Vector3 GetPosition(int field = 0)
    {
        if (State == BuildingState.Build)
        {
            var row = blueprint.row * 2;
            var column = blueprint.column * 2;
            var maxField = row * column;
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
            var x = nextField % row;
            var y = (nextField / row) % column;
            float xx, zz;
            var fx = x * 0.5f;
            var fy = y * 0.5f;
            switch (rotation)
            {
                case 0:
                    xx = position.x + fx;
                    zz = position.z + fy;
                    break;
                case 1:
                    xx = position.x + fy;
                    zz = position.z - fx;
                    break;
                case 2:
                    xx = position.x - fx;
                    zz = position.z - fy;
                    break;
                default:
                    xx = position.x - fy;
                    zz = position.z + fx;
                    break;
            }

            return new Vector3(xx - 0.25f, position.y + 0.5f,
                zz - 0.25f );
        }

        return base.GetPosition(field);
    }
    }
}