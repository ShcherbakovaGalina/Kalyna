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

        private Block GenerateKt(Block key, bool useLog = false)
        {
            var kt = new Block
            {
                Data = new List<byte>
                {
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5
                }
            };

            kt.AddRoundKey(key);
            if (useLog)
                Log("state[0].add_rkey:", kt);

            kt.SubBytes();
            if (useLog)
                Log("state[0].s_box:", kt);

            kt.ShiftRows();
            if (useLog)
                Log("state[0].s_row:", kt);

            kt.MixColumns();
            if (useLog)
                Log("state[0].m_col:", kt);

            kt.Xor(key);
            if (useLog)
                Log("state[0].xor_rkey:", kt);

            kt.SubBytes();
            if (useLog)
                Log("state[0].s_box:", kt);

            kt.ShiftRows();
            if (useLog)
                Log("state[0].s_row:", kt);

            kt.MixColumns();
            if (useLog)
                Log("state[0].m_col:", kt);

            kt.AddRoundKey(key);
            if (useLog)
                Log("state[0].add_rkey:", kt);

            kt.SubBytes();
            if (useLog)
                Log("state[0].s_box:", kt);

            kt.ShiftRows();
            if (useLog)
                Log("state[0].s_row:", kt);

            kt.MixColumns();
            if (useLog)
                Log("state[0].m_col:", kt);

            return kt;
        }

        public List<Block> GenerateRoundsKeys(Block key, bool useLog = false)
        {
            Log("Key", key);

            var keys = new List<Block>();
            for (var i = 0; i <= 10; i++)
                keys.Add(new Block());

            // Even keys

            //var kt = new Block
            //{
            //    Data = new List<byte>
            //    {
            //        0x7D, 0xD8, 0xE2, 0x38, 0x2F, 0xBC, 0x5C, 0xD0,
            //        0xA1, 0x5B, 0x77, 0x3B, 0x65, 0x1F, 0x2F, 0x86
            //    }
            //};
            var kt = GenerateKt(key, useLog);
            Log("KT", kt);

            for (var i = 0; i <= 10; i += 2)
            {
                if (useLog)
                    Console.WriteLine();
                var roundKey = keys[i];
                roundKey.Data = new List<byte>(StaticTables.V);
                roundKey.ShiftLeft(i / 2);
                if (useLog)
                    Log($"state[{i}].ShiftLeft (tmv):", roundKey);

                var keyCopy = new Block(key);
                keyCopy.RotateRight(32 * i);
                if (useLog)
                    Log($"state[{i}].Rotate (id):", keyCopy);

                roundKey.AddRoundKey(kt);
                if (useLog)
                    Log($"state[{i}].add_rkey (tmv):", roundKey);
                var copy = new Block(roundKey);

                roundKey.AddRoundKey(keyCopy);
                if (useLog)
                    Log($"state[{i}].add_rkey (kt_round):", roundKey);

                roundKey.SubBytes();
                if (useLog)
                    Log($"state[{i}].s_box:", roundKey);

                roundKey.ShiftRows();
                if (useLog)
                    Log($"state[{i}].s_row:", roundKey);

                roundKey.MixColumns();
                if (useLog)
                    Log($"state[{i}].m_col:", roundKey);

                roundKey.Xor(copy);
                if (useLog)
                    Log($"state[{i}].xor_rkey (kt_round):", roundKey);

                roundKey.SubBytes();
                if (useLog)
                    Log($"state[{i}].s_box:", roundKey);

                roundKey.ShiftRows();
                if (useLog)
                    Log($"state[{i}].s_row:", roundKey);

                roundKey.MixColumns();
                if (useLog)
                    Log($"state[{i}].m_col:", roundKey);

                roundKey.AddRoundKey(copy);
                if (useLog)
                {
                    Log($"state[{i}].add_rkey (tmv):", copy);
                    Log($"state[{i}].add_rkey (kt_round):", roundKey);
                }
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
