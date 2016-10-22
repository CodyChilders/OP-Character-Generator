using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OP_Character_Generator
{
    struct StatBlock
    {
        //stats is 8 bytes
        //least significant 6 will hold each stat
        //most significant 2 should always be 0
        private ulong stats;
        private int bytesStored;

        public void Reset()
        {
            stats = 0;
            bytesStored = 0;
        }

        public void AddStat(int stat)
        {
            if (bytesStored == 6)
                Reset();
            ulong newStat = (ulong) stat;
            newStat <<= (bytesStored << 3); //multiply by 8 to convert bytes to bits
            ++bytesStored;
            stats |= newStat;
        }

        public int GetTotal()
        {
            ulong statsCopy = stats;
            int total = 0;
            for (int i = 0; i < 6; i++)
            {
                byte thisStat = (byte) (statsCopy & (ulong) byte.MaxValue);
                total += thisStat;
                statsCopy >>= 8;
            }
            return total;
        }

        public override string ToString()
        {
            string returnVal;
            ulong statsCopy = stats;
            unsafe
            {
                byte* statArray = stackalloc byte[6];
                int writeIndex = 0;
                for (int i = 0; i < 6; i++)
                {
                    statArray[writeIndex++] = (byte) (statsCopy & (ulong) byte.MaxValue);
                    statsCopy >>= 8;
                }
                returnVal = string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\nTotal: {6}\n", 
                    statArray[0], statArray[1], statArray[2], statArray[3], statArray[4], statArray[5], GetTotal());
            }
            return returnVal;
        }

        public static bool operator <(StatBlock a, StatBlock b)
        {
            return a.GetTotal() < b.GetTotal();
        }

        public static bool operator >(StatBlock a, StatBlock b)
        {
            return a.GetTotal() > b.GetTotal();
        }

        public static bool operator ==(StatBlock a, StatBlock b)
        {
            return a.GetTotal() == b.GetTotal();
        }

        public static bool operator !=(StatBlock a, StatBlock b)
        {
            return !(a == b);
        }
    }
}
