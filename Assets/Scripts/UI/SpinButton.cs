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
        }

        private void OnEnable()
        {
            m_button.onClick.AddListener(OnButtonClicked);
            EventManager.AddListener<RewardingCompletedEvent>(OnRewardingComplete);
        }

        private void OnDisable()
        {
            m_button.onClick.RemoveListener(OnButtonClicked);
            EventManager.RemoveListener<RewardingCompletedEvent>(OnRewardingComplete);
        }
        
        private void OnButtonClicked()
        {
            m_button.interactable = false;
            EventManager.Invoke(SpinButtonClickedEvent.New());
        }
        
        private void OnRewardingComplete(RewardingCompletedEvent _)
        {
            m_button.interactable = true;
        }
    }
}