using System;
using System.Linq;
using Random = UnityEngine.Random;

namespace Utility
{
    public class TypeSequenceGenerator
    {
        private static readonly SymbolType[] possibleSymbolTypes;
        private static readonly int varietySize;

        private SymbolType m_lastRandomType;
        
        static TypeSequenceGenerator()
        {
            possibleSymbolTypes = Enum.GetValues(typeof(SymbolType)) as SymbolType[];
            possibleSymbolTypes = possibleSymbolTypes.OrderBy(type => (int)type).ToArray();

            varietySize = possibleSymbolTypes.Length;
        }

        public SymbolType[] GetSequence(SymbolType targetType, int size, int extraFill)
        {
            var symbolSequence = new SymbolType[size];

            var targetIndex = size - extraFill - 1;
            
            symbolSequence[targetIndex] = targetType;
            m_lastRandomType = targetType;
            
            // fill below target type
            for (int i = targetIndex - 1; i >= 0; i--)
            {
                symbolSequence[i] = GetRandomSingle();
            }

            m_lastRandomType = targetType;
            
            // fill after target type
            for (int i = targetIndex + 1; i < size; i++)
            {
                symbolSequence[i] = GetRandomSingle();
            }

            return symbolSequence;
        }

        // to ensure no subsequent identical symbols.
        public SymbolType GetRandomSingle()
        {
            while (true)
            {
                var index = Random.Range(0, varietySize);
                var randomType = possibleSymbolTypes[index];

                if (randomType == m_lastRandomType)
                    continue;
                
                return randomType;
            }
        }
    }
}