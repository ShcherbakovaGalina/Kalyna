using System;
using System.Collections.Generic;
using System.Numerics;

namespace Kalyna
{
    public class Algorithm
    {
        public bool UseLog { get; set; } = false;

        private List<Block> RoundsKeys { get; set; } = new List<Block>();

        public void Log(string message, Block block)
        {
            Console.WriteLine($"{message,-30} {new BigInteger(block.Data.ToArray()).ToString("X32")}");
        }

        private Block GenerateKt(Block key)
        {
            var kt = new Block
            {
                Data = new List<byte>
                {
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5
                }
            };

            kt.AddRoundKey(key);
            if (UseLog)
                Log("state[0].add_rkey:", kt);

            kt.SubBytes();
            if (UseLog)
                Log("state[0].s_box:", kt);

            kt.ShiftRows();
            if (UseLog)
                Log("state[0].s_row:", kt);

            kt.MixColumns();
            if (UseLog)
                Log("state[0].m_col:", kt);

            kt.Xor(key);
            if (UseLog)
                Log("state[0].xor_rkey:", kt);

            kt.SubBytes();
            if (UseLog)
                Log("state[0].s_box:", kt);

            kt.ShiftRows();
            if (UseLog)
                Log("state[0].s_row:", kt);

            kt.MixColumns();
            if (UseLog)
                Log("state[0].m_col:", kt);

            kt.AddRoundKey(key);
            if (UseLog)
                Log("state[0].add_rkey:", kt);

            kt.SubBytes();
            if (UseLog)
                Log("state[0].s_box:", kt);

            kt.ShiftRows();
            if (UseLog)
                Log("state[0].s_row:", kt);

            kt.MixColumns();
            if (UseLog)
                Log("state[0].m_col:", kt);

            return kt;
        }

        public List<Block> GenerateRoundsKeys(Block key)
        {
            Log("Key", key);

            for (var i = 0; i <= 10; i++)
                RoundsKeys.Add(new Block());

            // Even keys

            //var kt = new Block
            //{
            //    Data = new List<byte>
            //    {
            //        0x7D, 0xD8, 0xE2, 0x38, 0x2F, 0xBC, 0x5C, 0xD0,
            //        0xA1, 0x5B, 0x77, 0x3B, 0x65, 0x1F, 0x2F, 0x86
            //    }
            //};
            var kt = GenerateKt(key);
            Log("KT", kt);

            for (var i = 0; i <= 10; i += 2)
            {
                if (UseLog)
                    Console.WriteLine();
                var roundKey = RoundsKeys[i];
                roundKey.Data = new List<byte>(StaticTables.V);
                roundKey.ShiftLeft(i / 2);
                if (UseLog)
                    Log($"state[{i}].ShiftLeft (tmv):", roundKey);

                var keyCopy = new Block(key);
                keyCopy.RotateRight(32 * i);
                if (UseLog)
                    Log($"state[{i}].Rotate (id):", keyCopy);

                roundKey.AddRoundKey(kt);
                if (UseLog)
                    Log($"state[{i}].add_rkey (tmv):", roundKey);
                var copy = new Block(roundKey);

                roundKey.AddRoundKey(keyCopy);
                if (UseLog)
                    Log($"state[{i}].add_rkey (kt_round):", roundKey);

                roundKey.SubBytes();
                if (UseLog)
                    Log($"state[{i}].s_box:", roundKey);

                roundKey.ShiftRows();
                if (UseLog)
                    Log($"state[{i}].s_row:", roundKey);

                roundKey.MixColumns();
                if (UseLog)
                    Log($"state[{i}].m_col:", roundKey);

                roundKey.Xor(copy);
                if (UseLog)
                    Log($"state[{i}].xor_rkey (kt_round):", roundKey);

                roundKey.SubBytes();
                if (UseLog)
                    Log($"state[{i}].s_box:", roundKey);

                roundKey.ShiftRows();
                if (UseLog)
                    Log($"state[{i}].s_row:", roundKey);

                roundKey.MixColumns();
                if (UseLog)
                    Log($"state[{i}].m_col:", roundKey);

                roundKey.AddRoundKey(copy);
                if (UseLog)
                {
                    Log($"state[{i}].add_rkey (tmv):", copy);
                    Log($"state[{i}].add_rkey (kt_round):", roundKey);
                }
                RoundsKeys[i] = roundKey;
            }

            // Odd keys
            for (var i = 1; i <= 10; i += 2)
            {
                RoundsKeys[i].Data = RoundsKeys[i - 1].Data;
                if (i == 7)
                {
                    
                }
                RoundsKeys[i].RotateLeft(56);
            }

            return RoundsKeys;
        }

        public Block Encrypt(Block plainText, Block key)
        {
            var encryptedText = new Block(RoundsKeys[0]);
            //encryptedText.AddRoundKey(RoundsKeys[0]);

            for (var i = 1; i <= 9; i++)
            {
                if (UseLog)
                    Console.WriteLine();

                encryptedText.SubBytes();
                if (UseLog)
                    Log($"round[{i}].s_box:", encryptedText);

                encryptedText.ShiftRows();
                if (UseLog)
                    Log($"round[{i}].s_row:", encryptedText);

                encryptedText.MixColumns();
                if (UseLog)
                    Log($"round[{i}].m_col:", encryptedText);

                encryptedText.Xor(RoundsKeys[i]);
                if (UseLog)
                    Log($"round[{i}].xor_rkey:", encryptedText);
            }

            encryptedText.SubBytes();
            if (UseLog)
                Log("round[10].s_box:", encryptedText);

            encryptedText.ShiftRows();
            if (UseLog)
                Log("round[10].s_row:", encryptedText);

            encryptedText.MixColumns();
            if (UseLog)
                Log("round[10].m_col:", encryptedText);

            encryptedText.AddRoundKey(RoundsKeys[9]);
            if (UseLog)
                Log("round[10].add_rkey:", encryptedText);

            return encryptedText;
        }
    }
}
