using System;
using System.Collections.Generic;
using System.Numerics;

namespace Kalyna
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var a = new Algorithm();
            var key = new Block
            {
                Data = new List<byte>
                {
                    15, 14, 13, 12, 11, 10, 9, 8,
                    7, 6, 5, 4, 3, 2, 1, 0
                }
            };

            var keys = a.GenerateRoundsKeys(key);
            Console.WriteLine();
            for (var i = 0; i < keys.Count; i++)
            {
                a.Log(i.ToString(), keys[i]);
            }
        }
    }
}
