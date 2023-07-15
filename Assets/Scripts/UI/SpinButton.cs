using Events;
using Events.Implementations;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SpinButton : MonoBehaviour
    {
        private Button m_button;

        private void Awake()
        {
            m_button = GetComponent<Button>();
            
            GameEventSystem.AddListener<FullSpinStartedEvent>(OnFullSpinStarted);
            GameEventSystem.AddListener<RewardingCompletedEvent>(OnRewardingComplete);
        }

        private void OnRewardingComplete(object obj)
        {
            m_button.interactable = true;
        }

        private void OnFullSpinStarted(object obj)
        {
            m_button.interactable = false;
        }
    }
}