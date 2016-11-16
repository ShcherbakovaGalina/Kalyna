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

            kt.SubBytes(StaticTables.Π);
            if (UseLog)
                Log("state[0].s_box:", kt);

            kt.ShiftRows();
            if (UseLog)
                Log("state[0].s_row:", kt);

            kt.MixColumns(StaticTables.Mds);
            if (UseLog)
                Log("state[0].m_col:", kt);

            kt.Xor(key);
            if (UseLog)
                Log("state[0].xor_rkey:", kt);

            kt.SubBytes(StaticTables.Π);
            if (UseLog)
                Log("state[0].s_box:", kt);

            kt.ShiftRows();
            if (UseLog)
                Log("state[0].s_row:", kt);

            kt.MixColumns(StaticTables.Mds);
            if (UseLog)
                Log("state[0].m_col:", kt);

            kt.AddRoundKey(key);
            if (UseLog)
                Log("state[0].add_rkey:", kt);

            kt.SubBytes(StaticTables.Π);
            if (UseLog)
                Log("state[0].s_box:", kt);

            kt.ShiftRows();
            if (UseLog)
                Log("state[0].s_row:", kt);

            kt.MixColumns(StaticTables.Mds);
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

                roundKey.SubBytes(StaticTables.Π);
                if (UseLog)
                    Log($"state[{i}].s_box:", roundKey);

                roundKey.ShiftRows();
                if (UseLog)
                    Log($"state[{i}].s_row:", roundKey);

                roundKey.MixColumns(StaticTables.Mds);
                if (UseLog)
                    Log($"state[{i}].m_col:", roundKey);

                roundKey.Xor(copy);
                if (UseLog)
                    Log($"state[{i}].xor_rkey (kt_round):", roundKey);

                roundKey.SubBytes(StaticTables.Π);
                if (UseLog)
                    Log($"state[{i}].s_box:", roundKey);

                roundKey.ShiftRows();
                if (UseLog)
                    Log($"state[{i}].s_row:", roundKey);

                roundKey.MixColumns(StaticTables.Mds);
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
            var cipherText = new Block(plainText);
            cipherText.AddRoundKey(RoundsKeys[0]);

            for (var i = 1; i <= 9; i++)
            {
                if (UseLog)
                    Console.WriteLine();

                cipherText.SubBytes(StaticTables.Π);
                if (UseLog)
                    Log($"round[{i}].s_box:", cipherText);

                cipherText.ShiftRows();
                if (UseLog)
                    Log($"round[{i}].s_row:", cipherText);

                cipherText.MixColumns(StaticTables.Mds);
                if (UseLog)
                    Log($"round[{i}].m_col:", cipherText);

                cipherText.Xor(RoundsKeys[i]);
                if (UseLog)
                    Log($"round[{i}].xor_rkey:", cipherText);
            }

            if (UseLog)
                Console.WriteLine();

            cipherText.SubBytes(StaticTables.Π);
            if (UseLog)
                Log("round[10].s_box:", cipherText);

            cipherText.ShiftRows();
            if (UseLog)
                Log("round[10].s_row:", cipherText);

            cipherText.MixColumns(StaticTables.Mds);
            if (UseLog)
                Log("round[10].m_col:", cipherText);

            cipherText.AddRoundKey(RoundsKeys[10]);
            if (UseLog)
                Log("round[10].add_rkey:", cipherText);

            return cipherText;
        }

        public Block Decrypt(Block cipherText, Block key)
        {
            var plainText = new Block(cipherText);

            plainText.SubRoundKey(RoundsKeys[10]);
            if (UseLog)
                Log("round[10].sub_rkey:", plainText);

            plainText.MixColumns(StaticTables.MdsRev);
            if (UseLog)
                Log("round[10].m_col:", plainText);

            plainText.ShiftRowsRev();
            if (UseLog)
                Log("round[10].s_row:", plainText);

            plainText.SubBytes(StaticTables.ΠRev);
            if (UseLog)
                Log("round[10].s_box:", plainText);

            for (var i = 9; 1 <= i; --i)
            {
                if (UseLog)
                    Console.WriteLine();

                plainText.Xor(RoundsKeys[i]);
                if (UseLog)
                    Log($"round[{i}].xor_rkey:", plainText);

                plainText.MixColumns(StaticTables.MdsRev);
                if (UseLog)
                    Log($"round[{i}].m_col:", plainText);

                plainText.ShiftRowsRev();
                if (UseLog)
                    Log($"round[{i}].s_row:", plainText);

                plainText.SubBytes(StaticTables.ΠRev);
                if (UseLog)
                    Log($"round[{i}].s_box:", plainText);
            }

            plainText.SubRoundKey(RoundsKeys[0]);
            if (UseLog)
                Log("round[0].sub_rkey:", plainText);

            return plainText;
        }
    }
}
