using UnityEngine;
using UnityEditor;
using System.Collections;
namespace Kalagaan
{
    namespace POFX
    {
        [CustomEditor(typeof(POFX_OutlineBase), true)]
        public class POFXOutlineEditor : POFXLayerEditor
        {
            
            public override void DrawUI()
            {
                POFX_OutlineBase l = target as POFX_OutlineBase;

                if (l == null)
                    return;

                IntensitySliders(l);
                l.m_cParams.color = EditorGUILayout.ColorField("Color", l.m_cParams.color);
                l.m_cParams.outline = EditorGUILayout.FloatField("Outline", l.m_cParams.outline);
                l.m_params.stencilRef = EditorGUILayout.FloatField("Stencil", l.m_params.stencilRef);
            }
            
        }
    }
}
