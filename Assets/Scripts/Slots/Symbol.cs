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
        public SymbolType Type;

        [SerializeField] private RectTransform m_rect;
        [SerializeField] private Image m_image;
        
        private WheelLocation m_wheelLocation;
        private static SymbolFactory symbolFactory;

        
        private void Start()
        {
            symbolFactory ??= DI.Resolve<SymbolFactory>();
        }

        private void Update()
        {
            if (m_rect.position.y < Wheel.BottomY)
            {   
                symbolFactory.Return(this);
                GameEventSystem.Invoke<SymbolDisappearedEvent>(m_wheelLocation);
            }
        }

        public void SetSprite(Sprite sprite)
        {
            m_image.sprite = sprite;
        }

        public void SetPosition(Vector2 pos)
        {
            m_rect.anchoredPosition = pos;
        }
        
        public void SetParent(RectTransform parent)
        {
            m_rect.parent = parent;
        }

        public void SetWheel(WheelLocation location)
        {
            m_wheelLocation = location;
        }
        
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}