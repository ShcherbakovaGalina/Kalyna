using System;
using System.Collections.Generic;
using System.Numerics;

namespace Kalyna
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            //var cyka = new byte[] { 0x7F };

            //var c = new BigInteger(cyka);
            ////c = c << 1;
            //var h = BigInteger.Parse("127");

            //Console.WriteLine(c);

            var a = new Algorithm();
            var key = new Block
            {
                Data = new List<byte>
                {
                    15, 14, 13, 12, 11, 10, 9, 8,
                    7, 6, 5, 4, 3, 2, 1, 0
                }
            };
            a.GenerateRoundsKeys(key);
        }
    }
}
