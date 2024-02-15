using TMPro;
using UnityEngine;

namespace Utility
{
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField] private float m_refreshPeriod = 1f;

        [SerializeField, Tooltip("Uses Update function.")]
        private bool m_showFPS = true;
        
        private TextMeshProUGUI m_tmp;
        private float m_timer;
        private int m_currentFPS;

        private void Awake()
        {
            if (!m_showFPS)
                gameObject.SetActive(false);
        }

        private void Start()
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