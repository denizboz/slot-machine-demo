using System.Collections.Generic;
using CommonTools.Runtime.DependencyInjection;
using DG.Tweening;
using Managers;
using UnityEngine;
using Utility;

namespace Slots
{
    public class Wheel : MonoBehaviour
    {
        [SerializeField] private WheelLocation m_location;
        [SerializeField] private RectTransform m_spinner;

        private RectTransform m_rect;
        private SymbolFactory m_symbolFactory;
        private Symbol m_topSymbol;

        private static bool isBottomYSet;

        private const int visibleSymbolCount = 3; // must be odd to start with a symbol in the middle
        private const float symbolDistance = 275f; // in terms of pixels

        private TypeSequenceGenerator m_sequenceGenerator;

        private Symbol[] m_visibleSymbols;
        private List<Symbol> m_invisibleSymbols;


        private void Awake()
        {
            m_rect = GetComponent<RectTransform>();
            m_sequenceGenerator = new TypeSequenceGenerator();
        }

        private void Start()
        {
            m_symbolFactory = DI.Resolve<SymbolFactory>();
            FillInitial();
        }

        public void Spin(float time, float speed)
        {
            var distance = time * speed;
            distance -= distance % symbolDistance;

            // debug purpose
            var testType = m_location switch
            {
                WheelLocation.Left => SymbolType.A,
                WheelLocation.Middle => SymbolType.Jackpot,
                _ => SymbolType.A
            };
            
            var sequenceSize = (int)distance / (int)symbolDistance;
            var extraFill = visibleSymbolCount / 2;
            var typeSequence = m_sequenceGenerator.GetSequence(testType, sequenceSize, extraFill);

            UnloadSymbols();
            ResetSpinner();
            LoadSymbols(typeSequence);
            
            m_spinner.DOAnchorPos(distance * Vector2.down, time).SetEase(Ease.OutQuad);
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
                
                var symbol = m_symbolFactory.Get(type);
                
                symbol.SetParent(m_rect);
                
                symbol.SetPosition(pos);
                symbol.SetParent(m_spinner);

                var visibleStart = count - visibleSymbolCount;
                
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

                var symbolType = m_sequenceGenerator.GetRandomSingle();
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