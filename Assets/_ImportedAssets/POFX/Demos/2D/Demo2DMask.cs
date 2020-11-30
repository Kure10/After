using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kalagaan.POFX;

public class Demo2DMask : MonoBehaviour {

    POFX m_pofx;
    public float m_speed = 1;
    float m_progress = 0;
    int m_currentLayerId = -1;
    bool m_ShowLayer = true;

	// Use this for initialization
	void Start () {
        m_pofx = GetComponent<POFX>();

        for (int i = 0; i < m_pofx.layerCount; ++i)
        {
            if(m_pofx.GetLayer(i)!=null)
                m_pofx.GetLayer(i).m_cParams.intensity = 0;
        }
    }
	
	// Update is called once per frame
	void Update () {

        POFXLayer l = m_pofx.GetLayer(m_currentLayerId);

        if (l == null)
        {
            m_progress += Time.deltaTime * m_speed;
            if (m_progress > 2)
            {
                m_progress = 0;
                m_currentLayerId++;
            }
            return;
        }

        l.m_enable = true;

        if (m_ShowLayer)
        {
            l.m_cParams.intensity += Time.deltaTime * m_speed;
            l.m_cParams.intensity = Mathf.Clamp01(l.m_cParams.intensity);
            if (l.m_cParams.intensity == 1)
                m_progress += Time.deltaTime * m_speed;

            if (m_progress >= 1)
                m_ShowLayer = false;
        }
        else
        {
            l.m_cParams.intensity -= Time.deltaTime * m_speed;
            l.m_cParams.intensity = Mathf.Clamp01(l.m_cParams.intensity);
            if (l.m_cParams.intensity == 0)
            {
                m_progress = 0;
                m_ShowLayer = true;
                m_currentLayerId++;
                if (m_currentLayerId == m_pofx.layerCount)
                    m_currentLayerId = -1;                
            }

        }

    }
}
