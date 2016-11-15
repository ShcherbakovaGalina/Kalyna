using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Kalyna
{
    public class Block
    {
        public List<byte> Data { get; set; } = new List<byte>();

        public Block() { }

        public Block(Block block)
        {
            Data = new List<byte>(block.Data);
        }

        public void AddRoundKey(Block key)
        {
            for (var i = 0; i < Data.Count; i++)
                Data[i] += key.Data[i];
        }

        public void RotateRight(int i)
        {
            var bi = new BigInteger(Data.ToArray());
            //Console.WriteLine($"1. {bi.ToString("X")}");
            //Console.WriteLine($"2. {((bi >> i) | (bi << (128 - i))).ToString("X")}");
            bi = (bi >> i) | (bi << (128 - i));
            //Console.WriteLine($"3. {bi.ToString("X")}");
            Data = new List<byte>(bi.ToByteArray().Where((t, idx) => idx < 16));
        }

        public void ShiftLeft(int i)
        {
            var bi = new BigInteger(Data.ToArray());
            bi = bi << i;
            Data = new List<byte>(bi.ToByteArray());
        }
    }
}
