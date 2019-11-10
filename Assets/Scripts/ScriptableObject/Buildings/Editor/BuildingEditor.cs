using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Building))]
public class BuildingEditor : Editor
{

    //RawMaterials rawMat = RawMaterials.dynamit;
    
    private List<RawMaterials> listRawMaterials = new List<RawMaterials>();

    Sector sec = Sector.agregat;
    TypeOfBuilding type = TypeOfBuilding.basis;

    RawMaterials mat1 = RawMaterials.dynamit;
    RawMaterials mat2 = RawMaterials.dynamit;
    RawMaterials mat3 = RawMaterials.dynamit;
    RawMaterials mat4 = RawMaterials.dynamit;
    RawMaterials mat5 = RawMaterials.dynamit;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Building build = (Building)target;

        EditorGUILayout.Space();

        GUILayout.Label("Main Settings", EditorStyles.boldLabel);
        build.Name = EditorGUILayout.TextField("Building Name", build.Name);
        sec = (Sector)EditorGUILayout.EnumPopup("Sector", sec);
        type = (TypeOfBuilding)EditorGUILayout.EnumPopup("Type", type);
        build.TimeToBuild = EditorGUILayout.FloatField("Time To Build", build.TimeToBuild);
        build.Prefab = (GameObject)EditorGUILayout.ObjectField("Prefab",build.Prefab, typeof(GameObject), allowSceneObjects: false);
        GUILayout.Label("Building requirements ", EditorStyles.boldLabel);
        build.Size = (int)EditorGUILayout.Slider("Size Of Building", build.Size, 1, 6);
        build.ElectricConsumption = EditorGUILayout.FloatField("Electric Consumption ",build.ElectricConsumption);
        GUILayout.Label("Resources Cost", EditorStyles.boldLabel);
        build.Civil = EditorGUILayout.IntField("Civilni Material", build.Civil);
        build.Tech = EditorGUILayout.IntField("Technicky Material", build.Tech);
        build.Military = EditorGUILayout.IntField("Vojensky Material", build.Military);
        EditorGUILayout.Space();
        build.RawMaterial = (int)EditorGUILayout.Slider("Number of Raw Material", build.RawMaterial,0,5);
        OnRawMaterials(build.RawMaterial, build, listRawMaterials);
        GUILayout.Label("Ilustration Image", EditorStyles.boldLabel);
        build.Sprite = (Sprite)EditorGUILayout.ObjectField(build.Sprite, typeof(Sprite), allowSceneObjects: false);
        GUILayout.Label("Text Information", EditorStyles.boldLabel);
        build.Info = EditorGUILayout.TextArea(build.Info, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
        ProperytyLimits(build);

        build.SetSector(sec);
        build.SetTypeOfBuilding(type);

        build.SynchronizedList(listRawMaterials);
    }

    private void OnRawMaterials (int size, Building building, List<RawMaterials> listRawMaterials)
    {
        if(listRawMaterials.Count == 0)
        {
            for (int i = 0; i < 5; i++)
            {
                listRawMaterials.Add(RawMaterials.None);
            }
        }

        switch (size)
        {
            case 0:
                for (int i = 0; i <listRawMaterials.Count; i++)
                {
                    listRawMaterials[i] = RawMaterials.None;
                }
                break;
            case 1:
                mat1 = (RawMaterials)EditorGUILayout.EnumPopup("Material 1:", mat1);
                listRawMaterials[0] = mat1;
                for (int i = 1; i < listRawMaterials.Count; i++)
                {
                    listRawMaterials[i] = RawMaterials.None;
                }
                break;
            case 2:
                mat1 = (RawMaterials)EditorGUILayout.EnumPopup("Material 1:", mat1);
                listRawMaterials[0] = mat1;
                mat2 = (RawMaterials)EditorGUILayout.EnumPopup("Material 2:", mat2);
                listRawMaterials[1] = mat2;
                for (int i = 2; i < listRawMaterials.Count; i++)
                {
                    listRawMaterials[i] = RawMaterials.None;
                }
                break;
            case 3:
                mat1 = (RawMaterials)EditorGUILayout.EnumPopup("Material 1:", mat1);
                listRawMaterials[0] = mat1;
                mat2 = (RawMaterials)EditorGUILayout.EnumPopup("Material 2:", mat2);
                listRawMaterials[1] = mat2;
                mat3 = (RawMaterials)EditorGUILayout.EnumPopup("Material 3:", mat3);
                listRawMaterials[2] = mat3;
                for (int i = 3; i < listRawMaterials.Count; i++)
                {
                    listRawMaterials[i] = RawMaterials.None;
                }
                break;
            case 4:
                mat1 = (RawMaterials)EditorGUILayout.EnumPopup("Material 1:", mat1);
                listRawMaterials[0] = mat1;
                mat2 = (RawMaterials)EditorGUILayout.EnumPopup("Material 2:", mat2);
                listRawMaterials[1] = mat2;
                mat3 = (RawMaterials)EditorGUILayout.EnumPopup("Material 3:", mat3);
                listRawMaterials[2] = mat3;
                mat4 = (RawMaterials)EditorGUILayout.EnumPopup("Material 4:", mat4);
                listRawMaterials[3] = mat4;
                listRawMaterials[4] = RawMaterials.None;
                break;
            case 5:
                mat1 = (RawMaterials)EditorGUILayout.EnumPopup("Material 1:", mat1);
                listRawMaterials[0] = mat1;
                mat2 = (RawMaterials)EditorGUILayout.EnumPopup("Material 2:", mat2);
                listRawMaterials[1] = mat2;
                mat3 = (RawMaterials)EditorGUILayout.EnumPopup("Material 3:", mat3);
                listRawMaterials[2] = mat3;
                mat4 = (RawMaterials)EditorGUILayout.EnumPopup("Material 4:", mat4);
                listRawMaterials[3] = mat4;
                mat5 = (RawMaterials)EditorGUILayout.EnumPopup("Material 5:", mat5);
                listRawMaterials[4] = mat5;
                break;
            default:
                break;
        }
    }

    #region Limits
    private static void ProperytyLimits(Building build)
    {
        if (build.RawMaterial < 0)
        {
            build.RawMaterial = 0;
        }
        if (build.Civil < 0)
        {
            build.Civil = 0;
        }
        if (build.Tech < 0)
        {
            build.Tech = 0;
        }
        if (build.Military < 0)
        {
            build.Military = 0;
        }
        if (build.Size < 1)
        {
            build.Size = 1;
        }
        if(build.TimeToBuild < 0)
        {
            build.TimeToBuild = 0;
        }
        if (build.ElectricConsumption < 0)
        {
            build.ElectricConsumption = 0;
        }
    }
    #endregion 

}
