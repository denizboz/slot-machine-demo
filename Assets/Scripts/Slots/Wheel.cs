using System.Collections;
using System.Collections.Generic;
using CommonTools.Runtime.DependencyInjection;
using DG.Tweening;
using Events;
using Events.Implementations;
using UnityEngine;
using Utility;

namespace Slots
{
    public class Wheel : MonoBehaviour
    {
        [SerializeField] private Symbol m_symbolPrefab;
        [SerializeField] private SpriteContainerSO m_spriteContainer;
        [SerializeField] private Transform m_spinner;

        private TypeSequenceGenerator m_sequenceGenerator;
        
        private Symbol m_topSymbol;
        private Symbol m_bottomSymbol;

        private const int visibleSymbolCount = 3; // must be odd to start with a symbol in the middle
        private const int extraSymbolCount = 4; // must be even to add half to bottom & half to top
        
        private const int symbolCount = visibleSymbolCount + extraSymbolCount;
        private const int visibleAndUp = visibleSymbolCount + extraSymbolCount / 2;
        
        private const float symbolDistance = 3.75f; 

        private readonly List<Symbol> m_symbolPool = new List<Symbol>(symbolCount);
        private SymbolType[] m_sequence;
        private int m_sequenceIndex;

        private float m_bottomY;
        private const string bottomPosCheckRoutine = nameof(CheckBottomPosition);

        private void Awake()
        {
            m_sequenceGenerator = DI.Resolve<TypeSequenceGenerator>();
            m_sequenceGenerator.RegisterWheel(this);
            
            FillInitial();
            StartCoroutine(bottomPosCheckRoutine);
        }

        private void UpdateSymbolPositions()
        {
            m_symbolPool.Remove(m_bottomSymbol);

            var blurred = m_sequenceIndex < m_sequence.Length - visibleAndUp;
            var symbolType = m_sequence[m_sequenceIndex];
            var sprite = m_spriteContainer.GetSprite(symbolType, blurred);
            
            m_bottomSymbol.SetSprite(sprite);
            m_bottomSymbol.SetPosition(m_topSymbol.LocalPos + symbolDistance * Vector3.up);

            m_topSymbol = m_bottomSymbol;
            m_bottomSymbol = m_symbolPool[0];
            m_symbolPool.Add(m_topSymbol);

            m_sequenceIndex++;

            StartCoroutine(bottomPosCheckRoutine);
        }
        
        private IEnumerator CheckBottomPosition()
        {
            while (m_bottomSymbol.WorldPos.y > m_bottomY)
            {
                yield return null;
            }
            
            UpdateSymbolPositions();
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
                .OnComplete(() => EventManager.Invoke(WheelSpinCompletedEvent.New(targetType)));
        }
        
        private void ResetSpinner()
        {
            foreach (var symbol in m_symbolPool)
            {
                symbol.SetParent(transform, true);
            }
            
            m_spinner.localPosition = Vector3.zero;
            
            foreach (var symbol in m_symbolPool)
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
                var sprite = m_spriteContainer.GetSprite(symbolType);

                var symbol = Instantiate(m_symbolPrefab);
                    
                symbol.SetParent(m_spinner);
                symbol.SetPosition(pos);
                symbol.SetSprite(sprite);

                m_symbolPool.Add(symbol);

                if (i == 0)
                    m_bottomSymbol = symbol;
                else if (i == symbolCount - 1)
                    m_topSymbol = symbol;
            }
        }
    }
}