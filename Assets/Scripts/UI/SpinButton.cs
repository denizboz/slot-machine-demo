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

            m_button.onClick.AddListener(OnButtonClicked);
            GameEventSystem.AddListener<RewardingCompletedEvent>(OnRewardingComplete);
        }

        private void OnButtonClicked()
        {
            m_button.interactable = false;
            GameEventSystem.Invoke<SpinButtonClickedEvent>();
        }
        
        private void OnRewardingComplete(object obj)
        {
            m_button.interactable = true;
        }

        private void OnDestroy()
        {
            m_button.onClick.RemoveListener(OnButtonClicked);
            GameEventSystem.RemoveListener<RewardingCompletedEvent>(OnRewardingComplete);
        }
    }
}