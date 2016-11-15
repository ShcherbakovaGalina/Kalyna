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
            const int n = 8;
            for (var i = 0; i < 16; i += n)
            {
                var dataBi = new BigInteger(Data.Where((d, idx) => i <= idx && idx < i + n).ToArray());
                dataBi += new BigInteger(key.Data.Where((d, idx) => i <= idx && idx < i + n).ToArray());
                var newData = dataBi.ToByteArray();
                for (var j = 0; j < n; j++)
                    Data[i + j] = newData[j];
            }
            //var dataBi = new BigInteger(Data.ToArray());
            ////var keyBi = new BigInteger(key.Data.ToArray());
            //dataBi += new BigInteger(key.Data.ToArray());
            //Data = new List<byte>(dataBi.ToByteArray().Where((t, idx) => idx < 16));
            //for (var i = 0; i < Data.Count; i++)
            //    Data[i] += key.Data[i];
        }

        public void RotateRight(int i)
        {
            var bi = new BigInteger(Data.ToArray());
            bi = (bi >> i % 128) | (bi << (128 - i % 128));
            Data = new List<byte>(bi.ToByteArray().Where((t, idx) => idx < 16));
        }

        public void RotateLeft(int i)
        {
            var bi = new BigInteger(Data.ToArray());
            bi = (bi << i % 128) | (bi >> (128 - i % 128));
            Data = new List<byte>(bi.ToByteArray().Where((t, idx) => idx < 16));
        }

        public void ShiftLeft(int i)
        {
            var bi = new BigInteger(Data.ToArray());
            bi <<= i;
            Data = new List<byte>(bi.ToByteArray());
        }

        public void SubBytes()
        {
            var trump = 0;
            for (var i = Data.Count - 1; 0 <= i; --i)
            {
                var d = Data[i];
                var upper = (d & 0xF0) >> 4;
                var lower = d & 0x0F;
                Data[i] = StaticTables.Π0[trump % 4][upper][lower];
                trump = ++trump % 4;
            }
        }

        private void ShiftBytesPair(int i1, int i2)
        {
            var buf = Data[i1];
            Data[i1] = Data[i2];
            Data[i2] = buf;

            //var bi = new BigInteger(new[] { Data[i1], Data[i2] });
            //var rightBit = bi & 1;
            //bi >>= 1;
            //bi |= rightBit << 15;
            //var newBytes = bi.ToByteArray();
            //Data[i1] = newBytes[0];
            //Data[i2] = newBytes[1];
        }

        public void ShiftRows()
        {
            for (var i = 11; 8 <= i; --i)
                ShiftBytesPair(i, i - 8);
        }

        private static byte Gmul(int a, int b)
        {
            byte p = 0; /* the product of the multiplication */
            while (b != 0)
            {
                if ((b & 1) == 1) /* if b is odd, then add the corresponding a to p (final product = sum of all a's corresponding to odd b's) */
                    p ^= (byte)a; /* since we're in GF(2^m), addition is an XOR */

                if ((a & 0x80) == 0x80) /* GF modulo: if a >= 128, then it will overflow when shifted left, so reduce */
                    a = (a << 1) ^ 0x11d; /* XOR with the primitive polynomial x^8 + x^4 + x^3 + x + 1 (0b1_0001_1011) -- you can change it but it must be irreducible */
                else
                    a <<= 1; /* equivalent to a*2 */
                b >>= 1; /* equivalent to b // 2 */
            }
            return p;
        }

        public void MixColumns()
        {
            var dataCopy = new List<byte>(Data);

            // First column
            var trump = Data.Count - 1;
            for (var row = 0; row < 8; row++)
            {
                byte sum = 0;
                var hillary = Data.Count - 1;
                for (var h = 0; h < 8; h++)
                {
                    sum ^= Gmul(dataCopy[hillary], StaticTables.Mds[row][h]);
                    hillary--;
                }
                Data[trump] = sum;
                trump--;
            }

            // Second column
            trump = Data.Count - 1 - 8;
            for (var row = 0; row < 8; row++)
            {
                byte sum = 0;
                var hillary = Data.Count - 1 - 8;
                for (var h = 0; h < 8; h++)
                {
                    sum ^= Gmul(dataCopy[hillary], StaticTables.Mds[row][h]);
                    hillary--;
                }
                Data[trump] = sum;
                trump--;
            }
        }

        public void Xor(Block key)
        {
            for (var i = 0; i < Data.Count; i++)
                Data[i] ^= key.Data[i];
        }
    }
}
