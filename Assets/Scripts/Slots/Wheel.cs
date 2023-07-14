using System.Collections.Generic;
using CommonTools.Runtime.DependencyInjection;
using DG.Tweening;
using Events;
using Events.Implementations.Slots;
using Managers;
using UnityEngine;
using Utility;

namespace Slots
{
    public class Wheel : MonoBehaviour
    {
        [SerializeField] private WheelLocation m_location;
        [SerializeField] private RectTransform m_spinner;

        private readonly Queue<Symbol> m_symbolQueue = new Queue<Symbol>();
        
        private RectTransform m_rect;
        private SymbolFactory m_symbolFactory;
        private Symbol m_topSymbol;

        public static float BottomY;
        private static bool isBottomYSet;

        private const int initialSymbolCount = 5; // must be odd to start with a symbol in the middle
        private const float symbolDistance = 275f; // in terms of pixels

        private SymbolTypeSequenceGenerator m_sequenceGenerator;
        private SymbolType[] m_typeSequence;
        private int m_sequenceIndex;

        
        private void Awake()
        {
            m_rect = GetComponent<RectTransform>();
            m_sequenceGenerator = new SymbolTypeSequenceGenerator();
            
            GameEventSystem.AddListener<SymbolDisappearedEvent>(OnSymbolDisappeared);
        }

        private void Start()
        {
            m_symbolFactory = DI.Resolve<SymbolFactory>();
            LoadInitialSymbols();
        }

        public void Spin(float time, float speed)
        {
            var distance = time * speed;
            distance += distance % symbolDistance;

            // debug purpose
            var testType = m_location switch
            {
                WheelLocation.Left => SymbolType.A,
                WheelLocation.Middle => SymbolType.Jackpot,
                _ => SymbolType.A
            };
            
            var sequenceSize = (int)distance / (int)symbolDistance;
            m_typeSequence = m_sequenceGenerator.GetSequence(testType, sequenceSize);

            m_sequenceIndex = 0;
            
            m_spinner.DOAnchorPos(distance * Vector2.down, time).OnComplete(ResetSpinner);
        }

        private void OnSymbolDisappeared(object symbol)
        {
            RemoveSymbolFromBottom();
            AddSymbolToTop();
        }

        private void AddSymbolToTop()
        {
            Debug.Log($"{m_sequenceIndex} vs {m_typeSequence.Length}");
            
            var type = m_typeSequence[m_sequenceIndex];
            // var type = m_sequenceGenerator.GetRandomSingle();
            
            var symbol = m_symbolFactory.Get(type);
            symbol.SetWheel(m_location);
            symbol.SetParent(m_rect);

            var pos = m_topSymbol.AnchoredPos + symbolDistance * Vector2.up;

            symbol.SetPosition(pos);
            symbol.SetParent(m_spinner);

            m_topSymbol = symbol;
            m_symbolQueue.Enqueue(symbol);

            m_sequenceIndex++;
        }

        private void RemoveSymbolFromBottom()
        {
            var symbol = m_symbolQueue.Dequeue();
            m_symbolFactory.Return(symbol);
        }

        private void ResetSpinner()
        {
            var count = m_symbolQueue.Count;
            var symbols = new Symbol[count];

            for (int i = 0; i < count; i++)
            {
                var symbol = m_symbolQueue.Dequeue();
                symbols[i] = symbol;
                symbol.SetParent(m_rect, true);
            }

            m_spinner.anchoredPosition = Vector2.zero;
            
            for (int i = 0; i < count; i++)
            {
                var symbol = symbols[i];
                symbol.SetParent(m_spinner, true);
                m_symbolQueue.Enqueue(symbol);
            }
        }

        private void LoadInitialSymbols()
        {
            var startPos = -(initialSymbolCount / 2) * symbolDistance * Vector2.up;
            
            for (int i = 0; i < initialSymbolCount; i++)
            {
                var pos = startPos + (i * symbolDistance) * Vector2.up;

                if (i == 0 && !isBottomYSet)
                    SetBottomY(startPos - symbolDistance * Vector2.up);
                
                var symbolType = m_sequenceGenerator.GetRandomSingle();
                var symbol = m_symbolFactory.Get(symbolType);
                    
                symbol.SetParent(m_spinner);
                symbol.SetPosition(pos);
                symbol.SetWheel(m_location);
                
                m_symbolQueue.Enqueue(symbol);
                    
                if (i == initialSymbolCount - 1)
                    m_topSymbol = symbol;
            }
            
            if (!isBottomYSet)
                SetBottomY(startPos - symbolDistance * Vector2.up);
        }

        private void SetBottomY(Vector2 bottomPos)
        {
            if (isBottomYSet)
                return;
            
            var symbol = m_symbolFactory.Get(SymbolType.A);
            symbol.SetParent(m_rect);
            symbol.SetPosition(bottomPos);

            BottomY = symbol.transform.position.y;
            
            m_symbolFactory.Return(symbol);
            isBottomYSet = true;
        }
        
        private void OnDestroy()
        {
            GameEventSystem.RemoveListener<SymbolDisappearedEvent>(OnSymbolDisappeared);
        }
    }
}