using UnityEngine;
using UnityEditor;
using System.Collections;
namespace Kalagaan
{
    namespace POFX
    {
        [CustomEditor(typeof(POFX_PixelateBase), true)]
        public class POFXPixelateEditor : POFXLayerEditor
        {
            
            public override void DrawUI()
            {
                POFX_PixelateBase l = target as POFX_PixelateBase;

                if (l == null)
                    return;

                IntensitySliders(l);
                l.m_params.pixelSnapNear = EditorGUILayout.FloatField("Pixel snap Near", l.m_params.pixelSnapNear);                
                l.m_params.pixelSnapFar = EditorGUILayout.FloatField("Pixel snap Far", l.m_params.pixelSnapFar);
                l.m_params.distanceCamFar = EditorGUILayout.FloatField("Distance cam Far", l.m_params.distanceCamFar);
                l.m_cParams.color = EditorGUILayout.ColorField("Color", l.m_cParams.color);
                l.m_cParams.outline = EditorGUILayout.FloatField("Outline", l.m_cParams.outline);
               
            }

            

        }
    }
}
