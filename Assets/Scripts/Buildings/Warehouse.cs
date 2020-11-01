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
                Debug.Log("Warehouse - worker registered");
                Unregister(character);
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
    }
}