using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DebrisTile : Tile, IWorkSource
{
    public int hp;
    private List<Worker> Workers;

    public DebrisTile(GameObject tile, int x, int y) : base(tile, x, y)
    {
        walkthrough = false;
        hp = 100;
        Workers = new List<Worker>();
    }

    public void Register(Character who)
    {
        Unregister(who);
        var worker = new Worker();
        worker.character = who;
        worker.state = WorkerState.init;
        Workers.Add(worker);
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
}
