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

        static long GetLong(string prompt)
        {
            Console.Write(prompt);
            long returnVal = 0;
            do
            {
                string input = Console.ReadLine();
                try
                {
                    returnVal = Int64.Parse(input);
                }
                catch(FormatException)
                {
                    Console.WriteLine("Enter a positive integer");
                    continue;
                }
                catch(OverflowException)
                {
                    Console.WriteLine("Number is too large");
                    continue;
                }
                if(returnVal < 1)
                {
                    Console.WriteLine("Enter a positive integer");
                    continue;
                }
            } while (false);
            return returnVal;
        }

        static int GetInt(string prompt)
        {
            Console.Write(prompt);
            int returnVal = 0;
            do
            {
                string input = Console.ReadLine();
                try
                {
                    returnVal = Int32.Parse(input);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Enter a positive integer");
                    continue;
                }
                catch (OverflowException)
                {
                    Console.WriteLine("Number is too large");
                    continue;
                }
                if(returnVal < 1)
                {
                    Console.WriteLine("Enter a positive integer");
                    continue;
                }
            } while (false);
            return returnVal;
        }

        static void Main(string[] args)
        {
            Stopwatch timer = new Stopwatch();
            long total;
            int threads;
#if DEBUG
            total = (long) Math.Pow(10, 7);
            threads = 10;
#else
            total = GetLong("How many characters would you like to generate? ");
            if(total >= 1000000)
            {
                Console.WriteLine("Warning: generating large numbers of characters may take a while.");
            }
            threads = GetInt("How many threads would you like to use? ");
            if(threads > 5)
            {
                Console.WriteLine("Warning: starting many threads may impact performance of the rest of your system. " +
                                  "Performance will return once characters are done being generated." +
                                  "Close this window if performance becomes unacceptable on the rest of your system.");
            }
#endif
            timer.Start();
            StatBlock[] highestPerThread = new StatBlock[threads];
            StatBlock[] lowestPerThread = new StatBlock[threads];
            Parallel.For(0, threads, i =>
                {
                    Console.WriteLine("Starting thread {0}", i);
                    GenerateBatch(total / threads, out highestPerThread[i], out lowestPerThread[i]);
                    Console.WriteLine("Thread {0} complete", i);
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
