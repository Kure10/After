using UnityEngine;
using UnityEditor;
using System.Collections;
namespace Kalagaan
{
    namespace POFX
    {
        [CustomEditor(typeof(POFXLayer),true)]
        public class POFXLayerEditor : Editor
        {
            public enum eAction
            {
                NONE,
                REMOVE,
                UP,
                DOWN
            }

            public eAction OnInspector()
            {
                eAction a = eAction.NONE;
                POFXLayer l = target as POFXLayer;
                GUI.color = Color.grey;
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUI.color = Color.white;

                if (l.m_material == null)
                    l.Init();

                if (l.m_material == null)
                {
                    //GUILayout.Label("BUG");
                    return a;
                }

                POFXBase pofx = l.GetComponent<POFXBase>();
                int id = pofx.m_layers.IndexOf(l);


                GUILayout.BeginHorizontal();
                l.m_enable = GUILayout.Toggle(l.m_enable,"");
                GUILayout.Label( "" + l.GetName(), EditorStyles.boldLabel );
                GUILayout.FlexibleSpace();

                if (id != 0)
                {
                    if (GUILayout.Button("^", EditorStyles.helpBox))
                        a = eAction.UP;
                }

                if (id < pofx.m_layers.Count-1 )
                {
                    if (GUILayout.Button("v", EditorStyles.helpBox))
                        a = eAction.DOWN;
                }
                if (GUILayout.Button("X", EditorStyles.helpBox))
                    a = eAction.REMOVE;

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                
                GUILayout.BeginVertical(EditorStyles.helpBox);
                DrawUI();
                if (GUI.changed)
                    Undo.RecordObject(l,"POFX layer update");

                GUILayout.EndVertical();
                GUILayout.Space(20);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (l.GetComponent<SpriteRenderer>() != null)                                 
                    l.m_sortingOrderOffset = EditorGUILayout.IntField("Order in layer offset", l.m_sortingOrderOffset);                
                else
                    GUILayoutRenderQueue();                

                GUILayout.Space(20);
                GUILayout.EndHorizontal();

                if (l.m_enableMaterials.Count > 0)
                {
                    GUILayout.Label("Apply to materials :");
                    GUILayout.BeginHorizontal();
                    for (int m = 0; m < l.m_enableMaterials.Count; ++m)
                    //for (int m = 0; m < 20; ++m)
                    {
                        l.m_enableMaterials[m] = GUILayout.Toggle(l.m_enableMaterials[m], "" + (m+1), GUILayout.MaxWidth(30));
                        //GUILayout.Toggle(false, "" + m, GUILayout.MaxWidth(35));
                        if(m%5 == 4)                        
                        {
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                        }
                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
                GUILayout.Space(5);

                return a;
            }
            

            
            public void GUILayoutRenderQueue()
            {
                POFXLayer l = target as POFXLayer;

                if (l.m_renderQueue == 0)
                    l.m_renderQueue = l.m_material.shader.renderQueue;

                string[] choicesLabel = { "From Shader", "Geometry", "Alpha Test", "Transparent" };
                int[] choicesValues = { 0, 2000, 2450, 3000 };
                int idx = 0;                
                if (l.m_renderQueue <= 2450) idx = 2;
                if (l.m_renderQueue >= 3000) idx = 3;
                if (l.m_renderQueue <= 2000) idx = 1;
                if (l.m_renderQueue == l.m_material.shader.renderQueue) idx = 0;

                              
                int newIdx = EditorGUILayout.Popup("Render queue ", idx, choicesLabel);
                l.m_renderQueue = EditorGUILayout.IntField(l.m_renderQueue, GUILayout.MaxWidth(50));
                
                if(idx!= newIdx)
                {
                    switch (newIdx)
                    {
                        case 0:
                            l.m_renderQueue = l.m_material.shader.renderQueue;
                            l.m_material = l.GetMaterial(l.m_material.shader.name);
                            break;
                        default:
                            l.m_renderQueue = choicesValues[newIdx];
                            l.m_material = new Material(l.m_material);
                            l.m_material.renderQueue = l.m_renderQueue;
                            break;
                    }
                }
                
            }


            public void IntensitySliders(POFXLayer l)
            {
                l.m_cParams.intensity = EditorGUILayout.Slider("Intensity", l.m_cParams.intensity, 0, 1);
                l.m_cParams.intensityEdges = EditorGUILayout.Slider("Inside/Edges", l.m_cParams.intensityEdges, -1, 1);
            }



            public virtual void DrawUI()
            {
                POFXLayer l = target as POFXLayer;

                if (l == null)
                    return;

                l.m_cParams.intensity = EditorGUILayout.Slider("Intensity", l.m_cParams.intensity, 0, 1);
                l.m_cParams.color = EditorGUILayout.ColorField("Color", l.m_cParams.color);
                l.m_cParams.outline = EditorGUILayout.FloatField("Outline", l.m_cParams.outline);

                //DrawDefaultInspector();
            }




            public void GUILayoutTextureSlot(string label, ref Texture2D tex, ref Vector2 scale, ref Vector2 offset)
            {

                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                GUILayout.Label(label);
                scale = EditorGUILayout.Vector2Field("Tiling", scale);
                offset = EditorGUILayout.Vector2Field("Offset", offset);
                GUILayout.EndVertical();

                tex = EditorGUILayout.ObjectField("", tex, typeof(Texture2D), false, GUILayout.MaxWidth(70)) as Texture2D;
                GUILayout.EndHorizontal();
            }

        }
    }
}
