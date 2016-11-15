using System;
using System.Collections.Generic;
using System.Numerics;

namespace Kalyna
{
    public class Algorithm
    {

        public List<Block> GenerateRoundsKeys(Block key)
        {
            var keys = new List<Block>();
            for (var i = 0; i <= 10; i++)
                keys.Add(new Block());

            // Even keys

            var kt = new Block
            {
                Data = new List<byte>
                {
                    0x86, 0x2F, 0x1F, 0x65,
                    0x3B, 0x77, 0x5B, 0xA1,
                    0xD0, 0x5C, 0xBC, 0x2F,
                    0x38, 0xE2, 0xD8, 0x7D
                }
            };
            for (var i = 0; i <= 10; i++)
            {
                var roundKey = keys[i];
                roundKey.Data = new List<byte>(StaticTables.V);
                Console.WriteLine($"1. {new BigInteger(roundKey.Data.ToArray()).ToString("X")}");
                roundKey.ShiftLeft(i / 2);
                Console.WriteLine($"2. {new BigInteger(roundKey.Data.ToArray()).ToString("X")}");

                roundKey.RotateRight(32 * i);
                Console.WriteLine($"3. {new BigInteger(roundKey.Data.ToArray()).ToString("X")}");

                var tmv = new Block(roundKey);
                tmv.AddRoundKey(kt);
                Console.WriteLine($"4. {new BigInteger(tmv.Data.ToArray()).ToString("X")}");
                roundKey.AddRoundKey(kt);
                Console.WriteLine($"5. {new BigInteger(roundKey.Data.ToArray()).ToString("X")}");

                roundKey.AddRoundKey(tmv);
                Console.WriteLine($"6. {new BigInteger(roundKey.Data.ToArray()).ToString("X")}");
            }

            // Odd keys


            return null;
        }

    }
}
