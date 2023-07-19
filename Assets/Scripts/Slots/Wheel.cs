using System.Collections.Generic;
using CommonTools.Runtime.DependencyInjection;
using DG.Tweening;
using Events;
using Events.Implementations;
using Managers;
using UnityEngine;
using Utility;

namespace Slots
{
    public class Wheel : MonoBehaviour
    {
        public WheelLocation Location;
        
        [SerializeField] private RectTransform m_rect;
        [SerializeField] private RectTransform m_spinner;

        private SymbolFactory m_symbolFactory;
        private Symbol m_topSymbol;

        private static bool isBottomYSet;

        private const int visibleSymbolCount = 3; // must be odd to start with a symbol in the middle
        private const float symbolDistance = 275f; // in terms of pixels

        private TypeSequenceGenerator m_sequenceGenerator;

        private Symbol[] m_visibleSymbols;
        private List<Symbol> m_invisibleSymbols;

        
        private void Start()
        {
            m_symbolFactory = DI.Resolve<SymbolFactory>();
            m_sequenceGenerator = DI.Resolve<TypeSequenceGenerator>();
            
            m_sequenceGenerator.RegisterWheel(this);
            
            FillInitial();
        }

        public void Spin(SymbolType targetType, float time, float speed, Ease ease = Ease.Linear)
        {
            var distance = time * speed;
            distance -= distance % symbolDistance;

            var sequenceSize = (int)distance / (int)symbolDistance;
            var extraFill = visibleSymbolCount / 2;
            var typeSequence = m_sequenceGenerator.GetSequence(this, targetType, sequenceSize, extraFill);

            UnloadSymbols();
            ResetSpinner();
            LoadSymbols(typeSequence);

            m_spinner.DOAnchorPos(distance * Vector2.down, time).SetEase(Ease.OutQuad)
                .OnComplete(() => GameEventSystem.Invoke<WheelSpinCompletedEvent>(targetType));
        }
        
        private void LoadSymbols(SymbolType[] typeSequence)
        {
            // add visibles to invisibles
            foreach (var symbol in m_visibleSymbols)
            {
                m_invisibleSymbols.Add(symbol);
            }
            
            var count = typeSequence.Length;
            
            for (var i = 0; i < count; i++)
            {
                var type = typeSequence[i];
                var pos = m_topSymbol.AnchoredPos + (i + 1) * symbolDistance * Vector2.up;

                var visibleStart = count - visibleSymbolCount;
                
                var symbol = m_symbolFactory.Get(type, blurred: i < visibleStart);
                
                symbol.SetParent(m_rect);
                
                symbol.SetPosition(pos);
                symbol.SetParent(m_spinner);

                // register new visible symbols
                if (i > visibleStart - 1)
                    m_visibleSymbols[i - visibleStart] = symbol;
                else
                    m_invisibleSymbols.Add(symbol);
                
                if (i == typeSequence.Length - 1)
                    m_topSymbol = symbol;
            }
        }

        private void UnloadSymbols()
        {
            foreach (var symbol in m_invisibleSymbols)
            {
                m_symbolFactory.Return(symbol);
            }

            m_invisibleSymbols.Clear();
        }

        private void ResetSpinner()
        {
            for (int i = 0; i <visibleSymbolCount; i++)
            {
                m_visibleSymbols[i].SetParent(m_rect, true);
            }

            m_spinner.anchoredPosition = Vector2.zero;
            
            for (int i = 0; i < visibleSymbolCount; i++)
            {
                m_visibleSymbols[i].SetParent(m_spinner, true);
            }
        }

        private void FillInitial()
        {
            m_visibleSymbols = new Symbol[visibleSymbolCount];
            m_invisibleSymbols = new List<Symbol>();
            
            var startPos = -(visibleSymbolCount / 2) * symbolDistance * Vector2.up;
            
            for (int i = 0; i < visibleSymbolCount; i++)
            {
                var pos = startPos + (i * symbolDistance) * Vector2.up;

                var symbolType = m_sequenceGenerator.GetRandomSingle(this);
                var symbol = m_symbolFactory.Get(symbolType);
                    
                symbol.SetParent(m_spinner);
                symbol.SetPosition(pos);
                
                m_visibleSymbols[i] = symbol;
                
                if (i == visibleSymbolCount - 1)
                    m_topSymbol = symbol;
            }
        }
    }
}