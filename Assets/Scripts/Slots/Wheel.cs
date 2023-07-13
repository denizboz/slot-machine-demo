using CommonTools.Runtime.DependencyInjection;
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

        private Vector2[] m_symbolPositions;

        private SymbolFactory m_symbolFactory;
        
        private RectTransform m_rect;
        
        private const int symbolQueueSize = 5; // must be odd to start with a symbol in the middle
        private const float symbolDistance = 275f; // in terms of pixels

        public static float BottomY;
        private static bool isBottomYSet;
        
        
        private void Awake()
        {
            m_rect = GetComponent<RectTransform>();
            SetSymbolLocations();
            
            GameEventSystem.AddListener<SymbolDisappearedEvent>(OnSymbolDisappeared);
        }

        private void Update()
        {
            // debug purposes
            if (Input.GetKey(KeyCode.Return))
            {
                m_spinner.anchoredPosition += 5000f * Time.deltaTime * Vector2.down;
            }
        }

        private void Start()
        {
            m_symbolFactory = DI.Resolve<SymbolFactory>();
            
            LoadInitialSymbols();
            SetBottomY();
        }

        private void OnSymbolDisappeared(object location)
        {
            if ((WheelLocation)location == m_location)
                AddSymbol();
        }
        
        private void AddSymbol()
        {
            var symbol = m_symbolFactory.Get(SymbolType.A);
            symbol.SetParent(m_rect);
            symbol.SetPosition(m_symbolPositions[symbolQueueSize - 1]);
            symbol.SetParent(m_spinner);
            symbol.SetWheel(m_location);
        }

        private void SetSymbolLocations()
        {
            m_symbolPositions = new Vector2[symbolQueueSize];

            var startPos = -(symbolQueueSize / 2) * symbolDistance * Vector2.up;
            
            for (int i = 0; i < symbolQueueSize; i++)
            {
                m_symbolPositions[i] = startPos + (i * symbolDistance) * Vector2.up;
            }
        }

        private void LoadInitialSymbols()
        {
            for (int i = 0; i < symbolQueueSize - 1; i++)
            {
                var symbol = m_symbolFactory.Get(SymbolType.Bonus);
                var pos = m_symbolPositions[i];

                symbol.SetParent(m_spinner);
                symbol.SetPosition(pos);
                symbol.SetWheel(m_location);
            }
        }

        private void SetBottomY()
        {
            if (isBottomYSet)
                return;
            
            var symbol = m_symbolFactory.Get(SymbolType.A);
            symbol.SetParent(m_spinner);
            symbol.SetPosition(m_symbolPositions[0]);

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