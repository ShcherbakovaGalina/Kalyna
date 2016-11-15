using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Kalyna
{
    public class StaticTables
    {
        public static List<byte> V = new List<byte>
        {
            0, 1, 0, 1, 0, 1, 0, 1,
            0, 1, 0, 1, 0, 1, 0, 1,
            0, 1, 0, 1, 0, 1, 0, 1,
            0, 1, 0, 1, 0, 1, 0, 1
        };

        public static List<byte> Ff = new List<byte>
        {
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFE
        };

        public static BigInteger MaxBigInteger = new BigInteger(Ff.ToArray());

        public static List<byte> ShiftLeftV(int i)
        {
            return V.Select(v => (byte) (v << i)).ToList();
        }
    }
}
