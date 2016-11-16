using System;
using System.Collections.Generic;

namespace Kalyna
{
    internal static class Program
    {
        private static void Main()
        {
            var algorithm = new Algorithm
            {
                UseLog = true
            };
            var key = new Block
            {
                Data = new List<byte>
                {
                    15, 14, 13, 12, 11, 10, 9, 8,
                    7, 6, 5, 4, 3, 2, 1, 0
                }
            };

            var keys = algorithm.GenerateRoundsKeys(key);
            Console.WriteLine();
            for (var i = 0; i < keys.Count; i++)
            {
                algorithm.Log(i.ToString(), keys[i]);
            }

            var plainText = new Block
            {
                Data = new List<byte>
                {
                    31, 30, 29, 28, 27, 26, 25, 24,
                    23, 22, 21, 20, 19, 18, 17, 16
                }
            };
            algorithm.Encrypt(plainText, key);
        }
    }
}
