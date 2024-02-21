using Core.Runtime.DependencyInjection;
using UnityEngine;
using Utility;

namespace Slots
{
    public class Symbol : MonoBehaviour
    {
        public SymbolType Type { get; private set; }
        
        [SerializeField] private SpriteRenderer m_renderer;

        public Vector3 WorldPos => transform.position;
        public Vector3 LocalPos => transform.localPosition;
        

        public void SetType(SymbolType type)
        {
            Type = type;
        }
        
        public void SetSprite(Sprite sprite)
        {
            m_renderer.sprite = sprite;
        }

        public void SetPosition(Vector3 pos)
        {
            transform.localPosition = pos;
        }

        public void SetParent(Transform parent, bool worldPositionStays = false)
        {
            transform.SetParent(parent, worldPositionStays);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}