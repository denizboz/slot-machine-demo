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
        [SerializeField] private Transform m_spinner;

        private SymbolFactory m_symbolFactory;
        private TypeSequenceGenerator m_sequenceGenerator;
        
        private Symbol m_topSymbol;
        private Symbol m_bottomSymbol;

        private const int visibleSymbolCount = 3; // must be odd to start with a symbol in the middle
        private const int extraSymbolCount = 4; // must be even to add half bottom half top
        
        private const int symbolCount = visibleSymbolCount + extraSymbolCount;
        private const float symbolDistance = 3.75f; 

        private readonly List<Symbol> m_symbolList = new List<Symbol>();
        private SymbolType[] m_sequence;
        private int m_sequenceIndex;

        private float m_bottomY;


        private void Start()
        {
            m_symbolFactory = DI.Resolve<SymbolFactory>();
            m_sequenceGenerator = DI.Resolve<TypeSequenceGenerator>();
            
            m_sequenceGenerator.RegisterWheel(this);
            
            FillInitial();
        }

        private void Update()
        {
            if (m_bottomSymbol.WorldPos.y < m_bottomY)
                TryUpdateSymbols();
        }

        private void TryUpdateSymbols()
        {
            var symbol = m_bottomSymbol;
            m_symbolList.Remove(m_bottomSymbol);
            m_symbolFactory.Return(symbol);

            var visibleAndUp = visibleSymbolCount + extraSymbolCount / 2;
            var blurred = m_sequenceIndex < m_sequence.Length - visibleAndUp;
            var symbolType = m_sequence[m_sequenceIndex];
            
            symbol = m_symbolFactory.Get(symbolType, blurred);
            symbol.SetParent(m_spinner);
            symbol.SetPosition(m_topSymbol.LocalPos + symbolDistance * Vector3.up);
            
            m_symbolList.Add(symbol);

            m_bottomSymbol = m_symbolList[0];
            m_topSymbol = symbol;

            m_sequenceIndex++;
        }
        
        public void Spin(SymbolType targetType, float time, float speed, Ease ease = Ease.Linear)
        {
            var distance = time * speed;
            distance -= distance % symbolDistance;
            
            var sequenceSize = (int)(distance / symbolDistance);
            var extraFill = symbolCount / 2;

            m_sequenceIndex = 0;
            m_sequence = m_sequenceGenerator.GetSequence(this, targetType, sequenceSize, extraFill);

            ResetSpinner();
            
            m_spinner.DOLocalMove(distance * Vector3.down, time).SetEase(Ease.OutQuad)
                .OnComplete(() => GameEventSystem.Invoke<WheelSpinCompletedEvent>(targetType));
        }
        
        private void ResetSpinner()
        {
            foreach (var symbol in m_symbolList)
            {
                symbol.SetParent(transform, true);
            }
            
            m_spinner.localPosition = Vector3.zero;
            
            foreach (var symbol in m_symbolList)
            {
                symbol.SetParent(m_spinner, true);
            }
        }

        private void FillInitial()
        {
            var startPos = -(symbolCount / 2) * symbolDistance * Vector3.up;

            m_bottomY = startPos.y - symbolDistance / 2f;
            
            for (int i = 0; i < symbolCount; i++)
            {
                var pos = startPos + (i * symbolDistance) * Vector3.up;

                var symbolType = m_sequenceGenerator.GetRandomSingle(this);
                var symbol = m_symbolFactory.Get(symbolType);
                    
                symbol.SetParent(m_spinner);
                symbol.SetPosition(pos);

                m_symbolList.Add(symbol);

                if (i == 0)
                    m_bottomSymbol = symbol;
                else if (i == symbolCount - 1)
                    m_topSymbol = symbol;
            }
        }
    }
}