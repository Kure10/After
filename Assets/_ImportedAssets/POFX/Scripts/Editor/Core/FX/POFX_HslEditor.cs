using UnityEngine;
using UnityEditor;
using System.Collections;
namespace Kalagaan
{
    namespace POFX
    {
        [CustomEditor(typeof(POFX_HslBase), true)]
        public class POFXHslEditor : POFXLayerEditor
        {
            
            public override void DrawUI()
            {
                POFX_HslBase l = target as POFX_HslBase;

                if (l == null)
                    return;

                IntensitySliders(l);
                //l.m_cParams.color = EditorGUILayout.ColorField("Color", l.m_cParams.color);
                l.m_cParams.outline = EditorGUILayout.FloatField("Outline", l.m_cParams.outline);
                l.m_cParams.outline = Mathf.Clamp(l.m_cParams.outline, 0.0001f, float.MaxValue);

                l.m_params.hue = EditorGUILayout.Slider("Hue", l.m_params.hue, 0f, 1f);
                l.m_params.brightness = EditorGUILayout.Slider("Brightness", l.m_params.brightness, -1f, 1f);
                l.m_params.contrast = EditorGUILayout.Slider("Contrast", l.m_params.contrast, 0f, 1f);
                l.m_params.saturation = EditorGUILayout.Slider("Saturation", l.m_params.saturation, 0f, 1f);
                l.m_params.invert = EditorGUILayout.Slider("Invert", l.m_params.invert, 0f, 1f);
            }
            
        }
    }
}
