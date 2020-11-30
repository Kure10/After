using UnityEngine;
using UnityEditor;
using System.Collections;
namespace Kalagaan
{
    namespace POFX
    {
        [CustomEditor(typeof(POFX_ColorSwapBase), true)]
        public class POFXColorSwapEditor : POFXLayerEditor
        {
            
            public override void DrawUI()
            {
                POFX_ColorSwapBase l = target as POFX_ColorSwapBase;

                if (l == null)
                    return;

                IntensitySliders(l);
                l.m_params.mainTex = EditorGUILayout.ObjectField("Color texture", l.m_params.mainTex, typeof(Texture2D), false) as Texture2D;
                l.m_cParams.color = EditorGUILayout.ColorField("Color", l.m_cParams.color);
                l.m_params.color2 = EditorGUILayout.ColorField("Color2", l.m_params.color2);
                l.m_params.threshold = EditorGUILayout.Slider("Threshold", l.m_params.threshold, 0f, 1f);
                l.m_params.smooth = EditorGUILayout.Slider("Smooth", l.m_params.smooth, 0f, 1f);
                l.m_cParams.outline = EditorGUILayout.FloatField("Outline", l.m_cParams.outline);
                l.m_cParams.outline = Mathf.Clamp(l.m_cParams.outline, 0.0001f, float.MaxValue);

               
            }
            
        }
    }
}
