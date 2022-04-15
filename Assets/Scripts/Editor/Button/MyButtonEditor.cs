using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(MyButton))]
[CanEditMultipleObjects]
public class MyButtonEditor : ButtonEditor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("audioType"));

		if (GUI.changed)
		{
			serializedObject.ApplyModifiedProperties();
		}
	}
}
