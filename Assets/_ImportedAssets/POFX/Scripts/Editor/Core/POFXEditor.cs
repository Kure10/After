using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace Kalagaan
{
    namespace POFX
    {

        [CustomEditor(typeof(POFXBase),true)]
        public class POFXEditor : Editor
        {

            public static string selectedCategory = "";

            void AddLayer(POFXLayer layer)
            {
                POFXBase pofx = target as POFXBase;                
                pofx.AddLayer(layer);                
            }

            /*
            void AddLayer(string layerType)
            {
                POFXBase pofx = target as POFXBase;
                pofx.AddLayer(UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(pofx.gameObject, "Assets/POFX/Scripts/Editor/Core/POFXEditor.cs", layerType) as POFXLayer);

            }*/



            List<UnityEditor.MonoScript> m_types = null;

            static Texture2D m_logo;
            public override void OnInspectorGUI()
            {
                POFXBase pofx = target as POFXBase;

                
                if (m_logo == null)
                    m_logo = Resources.Load("POFX_banner") as Texture2D;
                GUILayout.BeginHorizontal(EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                GUILayout.Label(m_logo);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(-26);
                GUILayout.BeginHorizontal(EditorStyles.centeredGreyMiniLabel);
                GUILayout.FlexibleSpace();
                GUILayout.Label("" + POFXBase.version);
                GUILayout.EndHorizontal();

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Settings",EditorStyles.boldLabel);
                pofx.m_hideRenderer = EditorGUILayout.Toggle("Hide renderer", pofx.m_hideRenderer);
                pofx.m_hideEditorSelection = EditorGUILayout.Toggle("Hide editor selection", pofx.m_hideEditorSelection);
                if (pofx.m_hideEditorSelection)
                    EditorUtility.SetSelectedRenderState(pofx.GetComponent<Renderer>(), EditorSelectedRenderState.Hidden );
                else
                    EditorUtility.SetSelectedRenderState(pofx.GetComponent<Renderer>(), EditorSelectedRenderState.Highlight);
                GUILayout.EndVertical();


                int deleteId = -1;
                for (int i = 0; i < pofx.m_layers.Count; ++i)
                {
                    pofx.m_layers[i].hideFlags = HideFlags.HideInInspector;
                    POFXLayerEditor e = CreateEditor(pofx.m_layers[i]) as POFXLayerEditor;
                    if (e != null)
                    {
                        POFXLayer tmp;

                        switch (e.OnInspector())
                        {
                            case POFXLayerEditor.eAction.REMOVE:
                            if (EditorUtility.DisplayDialog("Delete layer", "Delete layer "+ pofx.m_layers[i].GetName() + " ?", "OK", "CANCEL"))
                                deleteId = i;
                                break;

                            case POFXLayerEditor.eAction.UP:
                                tmp = pofx.m_layers[i - 1];
                                pofx.m_layers[i - 1] = pofx.m_layers[i];
                                pofx.m_layers[i] = tmp;
                                break;

                            case POFXLayerEditor.eAction.DOWN:
                                tmp = pofx.m_layers[i + 1];
                                pofx.m_layers[i + 1] = pofx.m_layers[i];
                                pofx.m_layers[i] = tmp;
                                break;
                        }
                    }                       
                }

                if(deleteId!=-1)
                    pofx.RemoveLayer(pofx.m_layers[deleteId]);


                //if (GUI.changed)
                {
                    
                    pofx.Refresh();
                    SceneView.RepaintAll();
                }

                bool enableSprite = pofx.m_spriteRenderer != null;


                if (m_types == null)
                {
                    m_types = Resources
                        .FindObjectsOfTypeAll(typeof(MonoScript))
                        .Cast<MonoScript>()
                        .Where(x => x.GetClass() != null)
                        .Where(x => x.GetClass().BaseType != null)
                        .Where(x => (x.GetClass().BaseType.BaseType == typeof(Kalagaan.POFX.POFXLayer)))
                        .Where(x => x.GetClass().BaseType.GetField("path", BindingFlags.Public | BindingFlags.Static) != null)
                        .Where(x => (x.GetClass().BaseType.GetField("spriteCompatible", BindingFlags.Public | BindingFlags.Static) != null) || !enableSprite)
                        .OrderBy(x => "" + x.GetClass().BaseType.GetField("path", BindingFlags.Public | BindingFlags.Static).GetValue(null) + "" + x.GetClass())
                        .ToList();
                }

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Add FX", EditorStyles.boldLabel);

                string category = "";
                GUILayout.BeginHorizontal();
                for (int i=0; i< m_types.Count; ++i)
                {
                    string name = "" + m_types[i].GetClass();
                    name = name.Replace("Kalagaan.POFX.POFX_", "");
                    //name = name.Replace("Base", "");
                    //name = types[i].GetClass().GetProperties().Where(p => p.Name == "path").ToList()[0].GetValue(null);

                    FieldInfo field = m_types[i].GetClass().BaseType.GetField("path", BindingFlags.Public | BindingFlags.Static);
                    string FXCategory = "";
                    if(field!=null)
                        FXCategory = (string)field.GetValue(null);

                    if(FXCategory != category)
                    {
                        GUILayout.EndHorizontal();
                        category = FXCategory;
                        if (GUILayout.Button(category, EditorStyles.toolbarDropDown))
                        {
                            selectedCategory = selectedCategory!= category ? category : "";
                        }
                        GUILayout.BeginHorizontal();
                    }


                    if (selectedCategory == category)
                    {
                        if (GUILayout.Button(name))
                        {
                            //Debug.Log("Add " + types[i].GetClass());
                            //AddLayer("" + types[i].GetClass().Name);
                            //AddLayer( (types[i].GetClass().BaseType.BaseType) as POFXLayer);
                            AddLayer( pofx.gameObject.AddComponent(m_types[i].GetClass()) as POFXLayer);
                        }
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();

                
                

                GUILayout.Space(20);
                //DrawDefaultInspector();
            }






            public void OnSceneGUI()
            {
                Camera svcam = SceneView.currentDrawingSceneView.camera;
                //svcam.depthTextureMode = DepthTextureMode.None;
                svcam.ResetTransparencySortSettings();
                svcam.clearStencilAfterLightingPass = true;
            }


            /*
            static Camera m_cam;
            static RenderTexture m_rt;
            public void OnSceneGUI()
            {
                //SceneView.currentDrawingSceneView.camera.nearClipPlane = .001f;


                //return;
                if (m_cam == null)
                {
                    GameObject go = new GameObject("POFX_EditorCam");
                    go.hideFlags = HideFlags.DontSave;
                    m_cam = go.AddComponent<Camera>();
                    m_rt = new RenderTexture(300, 300,24);
                    m_cam.targetTexture = m_rt;
                }

                Camera svcam = SceneView.currentDrawingSceneView.camera;

                if (svcam != null)
                {
                    m_cam.transform.parent = svcam.transform;
                    m_cam.transform.localPosition = Vector3.zero;
                    m_cam.transform.localRotation = Quaternion.identity;

                    m_cam.fieldOfView = svcam.fieldOfView;

                    m_cam.Render();

                    Handles.BeginGUI();
                    GUILayout.Label(m_rt);
                    Handles.EndGUI();
                }
            }
            */

        }
        }
}
