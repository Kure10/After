using UnityEngine;
using UnityEditor;
using System.Collections;
namespace Kalagaan
{
    namespace POFX
    {
        [CustomEditor(typeof(POFX_RimBase), true)]
        public class POFX_RimEditor : POFXLayerEditor
        {

            public override void DrawUI()
            {
                POFX_RimBase l = target as POFX_RimBase;

                if (l == null)
                    return;

                IntensitySliders(l);
                l.m_cParams.color = EditorGUILayout.ColorField("Color", l.m_cParams.color);
                l.m_cParams.outline = EditorGUILayout.FloatField("Outline", l.m_cParams.outline);
                l.m_params.rimpower = EditorGUILayout.FloatField("Rim Power", l.m_params.rimpower);
                                
            }
        }
    }
}
