using System;

namespace Utility
{
    public enum SymbolType
    {
        A = 10,
        Bonus = 20,
        Seven = 30,
        Wild = 40,
        Jackpot = 50
    }
        
    [Serializable]
    public struct Lineup
    {
        public SymbolType Left;
        public SymbolType Middle;
        public SymbolType Right;
    }
}