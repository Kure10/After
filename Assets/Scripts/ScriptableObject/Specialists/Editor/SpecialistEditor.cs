using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Specialists))]
public class SpecialistEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Specialists spec = (Specialists)target;



        spec.maxStamina = 57600 + 2400 * spec.Level;
        spec.maxHP = 40 + spec.Level + 4 * spec.vojak + 2 * spec.technik + spec.vedec + spec.social;


    }

}
