using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "SpriteContainer", menuName = "Data/New Sprite Container")]
    public class SpriteContainerSO : ScriptableObject
    {
        [SerializeField] private Sprite[] m_sharpSprites;
        [SerializeField] private Sprite[] m_blurredSprites;

        private Dictionary<SymbolType, int> m_enumToIndexDictionary;

        private bool m_initialized;
        

        public Sprite GetSprite(SymbolType symbolType, bool blurred)
        {
            TryInitialize();

            var index = m_enumToIndexDictionary[symbolType];
            return !blurred ? m_sharpSprites[index] : m_blurredSprites[index];
        }
        
        
        // using TryInitialize method instead of static constructor since static constructor
        // of a scriptable object class is unexpectedly called while working in the editor.
        private bool TryInitialize()
        {
            if (m_initialized)
                return false;

            var symbols = Enum.GetValues(typeof(SymbolType)) as SymbolType[];

            if (m_sharpSprites.Length != symbols.Length || m_blurredSprites.Length != symbols.Length)
                throw new Exception("Sprite arrays must be of size symbol type variety.");
            
            symbols = symbols.OrderBy(symbol => (int)symbol).ToArray();
            
            m_enumToIndexDictionary = new Dictionary<SymbolType, int>(symbols.Length);

            for (var i = 0; i < symbols.Length; i++)
            {
                m_enumToIndexDictionary.Add(symbols[i], i);
            }
            
            return true;
        }
    }
}