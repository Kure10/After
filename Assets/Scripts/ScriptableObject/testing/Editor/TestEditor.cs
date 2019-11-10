using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Testing))]
public class TestEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Testing test = (Testing)target;

        GUILayout.Label("Testing Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        test.Prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", test.Prefab, typeof(GameObject), allowSceneObjects: false);
    }

}
