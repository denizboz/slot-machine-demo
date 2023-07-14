using System;
using System.Linq;
using Random = UnityEngine.Random;

namespace Utility
{
    public class SymbolTypeSequenceGenerator
    {
        private static readonly SymbolType[] possibleSymbolTypes;
        private static readonly int varietySize;

        private SymbolType m_lastRandomType;
        
        static SymbolTypeSequenceGenerator()
        {
            possibleSymbolTypes = Enum.GetValues(typeof(SymbolType)) as SymbolType[];
            possibleSymbolTypes = possibleSymbolTypes.OrderBy(type => (int)type).ToArray();

            varietySize = possibleSymbolTypes.Length;
        }

        public SymbolType[] GetSequence(SymbolType targetType, int size)
        {
            var symbolSequence = new SymbolType[size];

            symbolSequence[size - 1] = targetType;
            m_lastRandomType = targetType;
            
            for (int i = size - 2; i >= 0; i--)
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