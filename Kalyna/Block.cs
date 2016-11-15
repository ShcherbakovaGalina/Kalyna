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
            bi = (bi >> i) | (bi << (128 - i));
            Data = new List<byte>(bi.ToByteArray().Where((t, idx) => idx < 16));
        }

        public void ShiftLeft(int i)
        {
            var bi = new BigInteger(Data.ToArray());
            bi = bi << i;
            Data = new List<byte>(bi.ToByteArray());
        }

        public void SubBytes()
        {
            var trump = 0;
            for (var i = Data.Count - 1; 0 < i; --i)
            {
                var d = Data[i];
                var upper = (d & 0xF0) >> 4;
                var lower = d & 0x0F;
                Data[i] = StaticTables.Π0[trump % 4][upper][lower];
                trump = ++trump % 4;
            }
        }

        public void Xor(Block key)
        {
            for (var i = 0; i < Data.Count; i++)
                Data[i] ^= key.Data[i];
        }
    }
}
