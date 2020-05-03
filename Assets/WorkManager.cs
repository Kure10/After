using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorkManager : MonoBehaviour
{
    // Start is called before the first frame update
    private List<IWorkSource> workers;
    void Start()
    {
        workers = new List<IWorkSource>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var worker in workers.ToList())
        {
            worker.Update();
        }
    }

    public void Register(IWorkSource worker)
    {
        if (!workers.Contains(worker))
        {
            workers.Add(worker);
        }
    }

    public void Unregister(IWorkSource worker)
    {
        if (workers.Contains(worker))
        {
            workers.Remove(worker);
        }
    }
}
