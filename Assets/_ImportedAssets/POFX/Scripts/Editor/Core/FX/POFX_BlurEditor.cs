using UnityEngine;
using UnityEditor;
using System.Collections;
namespace Kalagaan
{
    namespace POFX
    {
        [CustomEditor(typeof(POFX_BlurBase), true)]
        public class POFX_BlurEditor : POFXLayerEditor
        {

            public override void DrawUI()
            {
                POFX_BlurBase l = target as POFX_BlurBase;

                if (l == null)
                    return;

                IntensitySliders(l);
                l.m_params.blurNear = EditorGUILayout.FloatField("Blur Near", l.m_params.blurNear);
                l.m_params.blurFar = EditorGUILayout.FloatField("Blur Far", l.m_params.blurFar);                
                l.m_params.distanceCamFar = EditorGUILayout.FloatField("Distance cam Far", l.m_params.distanceCamFar);
                l.m_cParams.outline = EditorGUILayout.FloatField("Outline", l.m_cParams.outline);
            }
        }
    }
}
