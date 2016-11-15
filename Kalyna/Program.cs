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
                    0, 1, 2, 3,
                    4, 5, 6, 7,
                    8, 9, 10, 11,
                    12, 13, 14, 15
                }
            };
            a.GenerateRoundsKeys(key);
        }
    }
}
