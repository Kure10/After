using UnityEngine;
using UnityEditor;
using System.Collections;
namespace Kalagaan
{
    namespace POFX
    {
        [CustomEditor(typeof(POFX_DesaturateBase), true)]
        public class POFXDesaturateEditor : POFXLayerEditor
        {
            
            public override void DrawUI()
            {
                POFX_DesaturateBase l = target as POFX_DesaturateBase;

                if (l == null)
                    return;

                IntensitySliders(l);
                l.m_cParams.color = EditorGUILayout.ColorField("Color", l.m_cParams.color);
                l.m_cParams.outline = EditorGUILayout.FloatField("Outline", l.m_cParams.outline);
                l.m_cParams.outline = Mathf.Clamp(l.m_cParams.outline, 0.0001f, float.MaxValue);
                

            }
            
        }
    }
}
