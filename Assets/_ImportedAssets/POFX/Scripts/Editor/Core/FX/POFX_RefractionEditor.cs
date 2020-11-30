using UnityEngine;
using UnityEditor;
using System.Collections;
namespace Kalagaan
{
    namespace POFX
    {
        [CustomEditor(typeof(POFX_RefractionBase), true)]
        public class POFXRefractionEditor : POFXLayerEditor
        {
            
            public override void DrawUI()
            {
                POFX_RefractionBase l = target as POFX_RefractionBase;

                if (l == null)
                    return;

                IntensitySliders(l);
                //l.m_params.distorsionTex = EditorGUILayout.ObjectField("Distorsion tex", l.m_params.distorsionTex, typeof(Texture2D),false) as Texture2D;
                GUILayoutTextureSlot("Distorsion tex",ref l.m_params.distorsionTex, ref l.m_params.distorsionTexScale, ref l.m_params.distorsionTexOffset);
                l.m_params.distorsion = EditorGUILayout.Slider("Distorsion", l.m_params.distorsion, 0f, 1f);
                
                l.m_cParams.color = EditorGUILayout.ColorField("Color", l.m_cParams.color);
                l.m_cParams.outline = EditorGUILayout.FloatField("Outline", l.m_cParams.outline);
                l.m_cParams.outline = Mathf.Clamp(l.m_cParams.outline, 0, float.MaxValue);
                l.m_params.refraction = EditorGUILayout.FloatField("Refraction", l.m_params.refraction);
                l.m_params.fresnel = EditorGUILayout.Slider("Fresnel", l.m_params.fresnel,0,1);
                

            }
            
        }
    }
}
