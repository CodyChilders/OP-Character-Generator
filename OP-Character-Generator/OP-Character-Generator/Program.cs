using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace OP_Character_Generator
{
    class Program
    {
        static void GetNewBlock(ThreadSafeRandom tsrng, ref StatBlock block)
        {
            block.Reset();
            for (int i = 0; i < 6; i++)
            {
                int min = Int32.MaxValue;
                int stat = 0;
                for(int j = 0; j < 4; j++)
                {
                    int roll = (tsrng.Next() % 6) + 1;
                    if (roll < min)
                        min = roll;
                    stat += roll;
                }
                stat -= min;
                block.AddStat(stat);
            }
        }

        static void GenerateBatch(long total, out StatBlock highestFound, out StatBlock lowestFound)
        {
            ThreadSafeRandom tsrng = new ThreadSafeRandom();
            StatBlock highestSoFar = new StatBlock();
            StatBlock lowestSoFar = new StatBlock();
            highestSoFar.Reset();
            lowestSoFar.Reset();
            StatBlock currentCharacter = new StatBlock();
            for(long i = 0; i < total; i++)
            {
                GetNewBlock(tsrng, ref currentCharacter);
                if (currentCharacter > highestSoFar || i == 0)
                {
                    highestSoFar = currentCharacter;
                }
                if (currentCharacter < lowestSoFar || i == 0)
                {
                    lowestSoFar = currentCharacter;
                }
            }
            highestFound = highestSoFar;
            lowestFound = lowestSoFar;
        }

        static void Main(string[] args)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            long total = (long) Math.Pow(10, 10) * 5;
            const int threads = 10;
            StatBlock[] highestPerThread = new StatBlock[threads];
            StatBlock[] lowestPerThread = new StatBlock[threads];
            Parallel.For(0, threads, i =>
                {
                    Console.WriteLine("Starting thread {0}", i);
                    int saveIndex = i;
                    GenerateBatch(total / threads, out highestPerThread[saveIndex], out lowestPerThread[saveIndex]);
                });
            StatBlock highestBlockGenerated = new StatBlock();
            foreach(var sb in highestPerThread)
            {
                if(sb > highestBlockGenerated)
                {
                    highestBlockGenerated = sb;
                }
            }
            StatBlock lowestBlockGenerated = highestBlockGenerated; //start this at a high value so the < works, otherwise the default is 0 and it is never changed
            foreach(var sb in lowestPerThread)
            {
                if (sb < lowestBlockGenerated)
                {
                    lowestBlockGenerated = sb;
                }
            }
            timer.Stop();
            Console.WriteLine("\nThe best character is:\n{0}", highestBlockGenerated.ToString());
            Console.WriteLine("\nThe worst character is:\n{0}", lowestBlockGenerated.ToString());
            Console.WriteLine("Generated {0:N0} characters in {1,2} minutes", total, ((float)timer.ElapsedMilliseconds)/(60 * 1000) );
            Console.WriteLine("\n\n\nPress enter to continue...");
            Console.ReadLine();
        }
    }
}
