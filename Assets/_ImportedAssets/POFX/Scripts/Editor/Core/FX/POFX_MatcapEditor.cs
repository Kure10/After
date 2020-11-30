using UnityEngine;
using UnityEditor;
using System.Collections;
namespace Kalagaan
{
    namespace POFX
    {
        [CustomEditor(typeof(POFX_MatcapBase), true)]
        public class POFXMatcapEditor : POFXLayerEditor
        {
            
            public override void DrawUI()
            {
                POFX_MatcapBase l = target as POFX_MatcapBase;

                if (l == null)
                    return;

                l.m_params.matcap = EditorGUILayout.ObjectField("Matcap", l.m_params.matcap, typeof(Texture2D),false) as Texture2D;
                IntensitySliders(l);
                l.m_cParams.color = EditorGUILayout.ColorField("Color", l.m_cParams.color);
                l.m_cParams.outline = EditorGUILayout.FloatField("Outline", l.m_cParams.outline);
                l.m_params.mix = EditorGUILayout.Slider("Mix", l.m_params.mix, 0f, 1f);
                l.m_params.addMul = EditorGUILayout.Slider("Add / Mul", l.m_params.addMul, 0f, 1f);
            }
            
        }
    }
}
