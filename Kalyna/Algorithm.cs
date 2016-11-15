using System;
using System.Collections.Generic;
using System.Numerics;

namespace Kalyna
{
    public class Algorithm
    {
        public void Log(string message, Block block)
        {
            Console.WriteLine($"{message,-30} {new BigInteger(block.Data.ToArray()).ToString("X32")}");
        }

        public List<Block> GenerateRoundsKeys(Block key)
        {
            Log("Key", key);

            var keys = new List<Block>();
            for (var i = 0; i <= 10; i++)
                keys.Add(new Block());

            // Even keys

            var kt = new Block
            {
                Data = new List<byte>
                {
                    0x7D, 0xD8, 0xE2, 0x38, 0x2F, 0xBC, 0x5C, 0xD0,
                    0xA1, 0x5B, 0x77, 0x3B, 0x65, 0x1F, 0x2F, 0x86
                }
            };
            Log("KT", kt);

            for (var i = 2; i <= 10; i += 2)
            {
                Console.WriteLine();
                var roundKey = keys[i];
                roundKey.Data = new List<byte>(StaticTables.V);
                roundKey.ShiftLeft(i / 2);
                Log($"state[{i}].ShiftLeft (tmv):", roundKey);

                var keyCopy = new Block(key);
                keyCopy.RotateRight(32 * i);
                Log($"state[{i}].Rotate (id):", keyCopy);

                roundKey.AddRoundKey(kt);
                Log($"state[{i}].add_rkey (tmv):", roundKey);
                var copy = new Block(roundKey);

                roundKey.AddRoundKey(keyCopy);
                Log($"state[{i}].add_rkey (kt_round):", roundKey);

                roundKey.SubBytes();
                Log($"state[{i}].s_box:", roundKey);

                roundKey.ShiftRows();
                Log($"state[{i}].s_row:", roundKey);

                roundKey.MixColumns();
                Log($"state[{i}].m_col:", roundKey);

                roundKey.Xor(copy);
                Log($"state[{i}].xor_rkey (kt_round):", roundKey);

                roundKey.SubBytes();
                Log($"state[{i}].s_box:", roundKey);

                roundKey.ShiftRows();
                Log($"state[{i}].s_row:", roundKey);

                roundKey.MixColumns();
                Log($"state[{i}].m_col:", roundKey);

                roundKey.AddRoundKey(copy);
                Log($"state[{i}].add_rkey (tmv):", copy);
                Log($"state[{i}].add_rkey (kt_round):", roundKey);
                keys[i] = roundKey;
            }

            // Odd keys
            for (var i = 1; i <= 9; i += 2)
            {
                keys[i].Data = keys[i - 1].Data;
                keys[i].RotateLeft(64);
            }

            return keys;
        }

    }
}
