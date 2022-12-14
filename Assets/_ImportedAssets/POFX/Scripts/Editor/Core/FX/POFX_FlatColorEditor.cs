using UnityEngine;
using UnityEditor;
using System.Collections;
namespace Kalagaan
{
    namespace POFX
    {
        [CustomEditor(typeof(POFX_FlatColorBase), true)]
        public class POFXFlatColorEditor : POFXLayerEditor
        {
            
            public override void DrawUI()
            {
                POFX_FlatColorBase l = target as POFX_FlatColorBase;

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
