using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OP_Character_Generator
{
    class ThreadSafeRandom
    {
        static readonly Random globalRNG = new Random();
        [ThreadStatic]
        static Random local;

        public ThreadSafeRandom()
        {
            if(local == null)
            {
                int seed;
                lock(globalRNG)
                {
                    seed = globalRNG.Next();
                }
                local = new Random(seed);
            }
        }

        public int Next()
        {
            return local.Next();
        }
    }
}
