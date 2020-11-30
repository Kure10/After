using UnityEngine;
using UnityEditor;
using System.Collections;
namespace Kalagaan
{
    namespace POFX
    {
        [CustomEditor(typeof(POFX_SobelBase), true)]
        public class POFXSobelEditor : POFXLayerEditor
        {
            
            public override void DrawUI()
            {
                POFX_SobelBase l = target as POFX_SobelBase;

                if (l == null)
                    return;

                IntensitySliders(l);
                l.m_cParams.color = EditorGUILayout.ColorField("Color", l.m_cParams.color);
                l.m_cParams.outline = EditorGUILayout.FloatField("Outline", l.m_cParams.outline);
                l.m_cParams.outline = Mathf.Clamp(l.m_cParams.outline, 0.0001f, float.MaxValue);

                l.m_params.invert = EditorGUILayout.Slider("Invert", l.m_params.invert, 0, 1);
                l.m_params.mixColor = EditorGUILayout.Slider("Mix color", l.m_params.mixColor, 0,1);
                

            }
            
        }
    }
}
