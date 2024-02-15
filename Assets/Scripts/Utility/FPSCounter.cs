using CommonTools.Runtime.DependencyInjection;
using TMPro;
using UnityEngine;

namespace Utility
{
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField] private float m_refreshPeriod = 1f;
        
        private TextMeshProUGUI m_tmp;
        private float m_timer;
        private int m_currentFPS;

        private void Awake()
        {
            m_tmp = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            m_currentFPS++;
            m_timer += Time.deltaTime;

            if (m_timer < m_refreshPeriod)
                return;
            
            m_tmp.text = $"{m_currentFPS.ToString()}FPS";
            m_currentFPS = 0;
            m_timer = 0f;
        }
    }
}