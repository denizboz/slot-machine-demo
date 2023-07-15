using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Slots
{
    public class Symbol : MonoBehaviour
    {
        public SymbolType Type { get; private set; }
        
        [SerializeField] private RectTransform m_rect;
        [SerializeField] private Image m_image;
        
        public Vector2 AnchoredPos => m_rect.anchoredPosition;
        

        public void SetType(SymbolType type)
        {
            Type = type;
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

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}