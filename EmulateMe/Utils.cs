using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmulateMe
{
    public static class Utils
    {
        private static Random rng;

        static Utils()
        {
            rng = new Random(Guid.NewGuid().GetHashCode());
        }

        public static uint NextUInt32()
        {
            var buf = new byte[4];
            rng.NextBytes(buf);
            return BitConverter.ToUInt32(buf, 0);
        }

        public static byte[] GetBytes(uint i)
        {
            return BitConverter.GetBytes(i);
        }

        public static List<uint> GetRandomValuesDistinct(int count)
        {
            var hs = new HashSet<uint>();
            var arr = new uint[count];

            while (count != 0)
            {
                if (hs.Add(NextUInt32()))
                {
                    count--;
                }
            }

            hs.CopyTo(arr);
            return arr.ToList();
        }

        public static string BytesToSourceString(byte[] buffer)
        {
            var sb = new StringBuilder();
            sb.Append("new byte[] {");

            for (int i = 0; i < buffer.Length - 1; i++)
                sb.Append($"0x{buffer[i]:X2}, ");

            sb.Append($"0x{buffer[buffer.Length - 1]:X2}");
            sb.Append("}");

            return sb.ToString();
        }
    }
}