using System.Collections.Generic;
using CommonTools.Runtime.DependencyInjection;
using Slots;
using UnityEngine;
using Utility;

namespace Managers
{
    public class SymbolFactory : Manager
    {
        [SerializeField] private SpriteContainerSO m_spriteContainer;
        
        [SerializeField] private Symbol m_symbolPrefab;
        [SerializeField] private RectTransform m_poolParent;
        
        private Queue<Symbol> m_symbolPool;

        private int poolSize = 128;

        protected override void Bind()
        {
            DI.Bind<SymbolFactory>(this);
        }

        protected override void OnAwake()
        {
            CreatePool();
        }
        
        public Symbol Get(SymbolType type, bool blurred = false)
        {
            EnsurePoolCapacity();
            
            var symbol = m_symbolPool.Dequeue();
            var sprite = m_spriteContainer.GetSprite(type, blurred);

            symbol.SetActive(true);
            symbol.SetType(type);
            symbol.SetSprite(sprite);
            
            return symbol;
        }

        public void Return(Symbol symbol)
        {
            symbol.SetActive(false);
            symbol.SetParent(m_poolParent);
            
            m_symbolPool.Enqueue(symbol);
        }

        public void SwitchSharp(Symbol symbol)
        {
            var sharpSprite = m_spriteContainer.GetSprite(symbol.Type, blurred: false);
            symbol.SetSprite(sharpSprite);
        }
        
        public void SwitchBlurred(Symbol symbol)
        {
            var blurredSprite = m_spriteContainer.GetSprite(symbol.Type, blurred: true);
            symbol.SetSprite(blurredSprite);
        }
        
        private void CreatePool()
        {
            m_symbolPool = new Queue<Symbol>(poolSize);
            
            for (int i = 0; i < poolSize; i++)
            {
                AddSymbolToPool();
            }
        }

        private void AddSymbolToPool()
        {
            var symbol = Instantiate(m_symbolPrefab, m_poolParent);
                
            symbol.SetActive(false);
            m_symbolPool.Enqueue(symbol);
        }
        
        private void EnsurePoolCapacity()
        {
            if (m_symbolPool.Count > 0)
                return;

            poolSize *= 2;

            for (int i = poolSize / 2; i < poolSize; i++)
            {
                AddSymbolToPool();
            }
            
            Debug.LogWarning($"Symbol pool capacity increased from {(poolSize / 2).ToString()} to {poolSize.ToString()}");
        }
    }
}