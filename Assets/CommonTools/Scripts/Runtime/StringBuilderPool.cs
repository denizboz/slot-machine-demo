using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CommonTools.Runtime
{
    public static class StringBuilderPool
    {
        private static int poolSize = 128;
        private static readonly Queue<StringBuilder> pool = new Queue<StringBuilder>(poolSize);


        static StringBuilderPool()
        {
            for (int i = 0; i < poolSize; i++)
            {
                var builder = new StringBuilder();
                pool.Enqueue(builder);
            }
        }
        
        public static StringBuilder Get()
        {
            EnsurePoolCapacity();
            
            var builder = pool.Dequeue();
            pool.Enqueue(builder);
            builder.Clear();
            return builder;
        }
        
        private static void EnsurePoolCapacity()
        {
            if (pool.Count > 0)
                return;

            poolSize *= 2;

            for (int i = poolSize / 2; i < poolSize; i++)
            {
                var builder = new StringBuilder();
                pool.Enqueue(builder);
            }
            
            Debug.LogWarning($"StringBuilder pool capacity increased from {(poolSize / 2).ToString()} to {poolSize.ToString()}");
        }
    }
}
