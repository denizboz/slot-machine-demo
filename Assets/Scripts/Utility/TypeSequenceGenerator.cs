using System.Collections.Generic;
using Slots;
using Random = UnityEngine.Random;

namespace Utility
{
    public class TypeSequenceGenerator
    {
        private readonly SymbolType[] m_possibleSymbolTypes;
        private readonly int m_varietySize;

        private readonly Dictionary<Wheel, SymbolType> m_lastRandomTypeDict;
        
        public TypeSequenceGenerator(SymbolType[] symbolTypes)
        {
            m_possibleSymbolTypes = symbolTypes;
            m_varietySize = symbolTypes.Length;

            m_lastRandomTypeDict = new Dictionary<Wheel, SymbolType>();
        }

        public void RegisterWheel(Wheel wheel)
        {
            var index = Random.Range(0, m_varietySize);
            var randomType = m_possibleSymbolTypes[index];

            if (m_lastRandomTypeDict.ContainsKey(wheel))
                m_lastRandomTypeDict[wheel] = randomType;
            else
                m_lastRandomTypeDict.Add(wheel, randomType);
        }
        
        public SymbolType[] GetSequence(Wheel wheel, SymbolType targetType, int size, int extraFill)
        {
            var symbolSequence = new SymbolType[size];

            var targetIndex = size - extraFill - 1;
            symbolSequence[targetIndex] = targetType;
            
            m_lastRandomTypeDict[wheel] = targetType;
            
            // fill below target type
            for (int i = targetIndex - 1; i >= 0; i--)
            {
                symbolSequence[i] = GetRandomSingle(wheel);
            }

            m_lastRandomTypeDict[wheel] = targetType;
            
            // fill above target type
            for (int i = targetIndex + 1; i < size; i++)
            {
                symbolSequence[i] = GetRandomSingle(wheel);
            }

            return symbolSequence;
        }

        // to ensure no subsequent identical symbols.
        public SymbolType GetRandomSingle(Wheel wheel)
        {
            while (true)
            {
                var index = Random.Range(0, m_varietySize);
                
                var randomType = m_possibleSymbolTypes[index];
                var lastRandomType = m_lastRandomTypeDict[wheel];
                
                if (randomType == lastRandomType)
                    continue;

                m_lastRandomTypeDict[wheel] = randomType;
                
                return randomType;
            }
        }
    }
}