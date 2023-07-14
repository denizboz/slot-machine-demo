using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations.Slots;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Slots
{
    public class Symbol : MonoBehaviour
    {
        [SerializeField] private RectTransform m_rect;
        [SerializeField] private Image m_image;
        
        public SymbolType Type { get; private set; }
        public WheelLocation ParentWheel { get; private set; }
        public Vector2 AnchoredPos => m_rect.anchoredPosition;
        
        private void Update()
        {
            if (m_rect.position.y < Wheel.BottomY)
                GameEventSystem.Invoke<SymbolDisappearedEvent>(this);
        }
        
        public void SetSprite(Sprite sprite)
        {
            m_image.sprite = sprite;
        }

        public void SetPosition(Vector2 pos)
        {
            m_rect.anchoredPosition = pos;
        }

        public void SetParent(RectTransform parent, bool worldPositionStays = false)
        {
            m_rect.SetParent(parent, worldPositionStays);
        }

        public void SetWheel(WheelLocation location)
        {
            ParentWheel = location;
        }
        
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}