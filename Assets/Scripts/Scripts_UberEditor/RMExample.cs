using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResolveMachine;
using System;

public static class RMExample
{
    public static void RunExample()
    {
        // Create main class
        ResolveMaster rm = new ResolveMaster();
        rm.ResolveCondition += OnResolveCondition;
        rm.ResolveAction += OnResolveAction;

        // Read data nodes from XML file
        var data2 = StatsClass.LoadBinaryFile("D:/CompileTest", "Items");
        rm.AddDataNode("Items", data2);

        // Apply localized data from XML file
        var dataloc2 = StatsClass.LoadBinaryFile("D:/CompileTest", "Items-CZ");
        rm.ModifyDataNode("Items", dataloc2);
        foreach (var key in rm.GetDataKeys("Items")) Debug.LogWarning(key.ToLog());

        // Read data nodes from binary file
        var data = StatsClass.LoadXmlFile("D:/CompileTest", "Dialogues",false);
        rm.AddDataNode("Dialogues", data);

        // Apply localized data from binary file
        var dataloc = StatsClass.LoadXmlFile("D:/CompileTest", "Dialogues-CZ", false);
        rm.ModifyDataNode("Dialogues", dataloc);

        // Create new diagram resolver
        ResolveSlave slave = rm.AddDataSlave("Dialogues", rm.GetDataKeys("Dialogues")[0].Title);
        // Resolve
        slave.StartResolve();
        var output = slave.Resolve();
    }

    // Resolve actions
    private static bool OnResolveAction(string id,  string dialogID, StatsClass data)
    {
        return false;
    }

    // Resolve conditions
    private static bool OnResolveCondition(string id, string dialogID, StatsClass data)
    {
        return false;
    }
}
