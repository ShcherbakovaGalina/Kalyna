using System;
using System.Collections.Generic;
using System.Numerics;

namespace Kalyna
{
    internal class Program
    {
        public Block D { get; set; } = new Block { Data = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } };
        public Block K { get; set; } = new Block { Data = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 } };

        private static void Main()
        {
            //var f = new FileEncoderDecoder
            //{
            //    PlainTextFileName = "Files\\P1.png",
            //    EncryptedTextFileName = "Files\\Encrypted2.png",
            //    DecryptedTextFileName = "Files\\Decrypted2.png",
            //    KeyFileName = "Files\\Key.txt"
            //};
            //f.Encode();
            //f.Decode();
            var D = new Block { Data = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } };
            D.Data = new List<byte>(new BigInteger(DateTime.UtcNow.Ticks).ToByteArray());
            for (var i = 0; i < 8; i++)
                D.Data.Add(0);
            var K = new Block { Data = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 } };
            var Kalyna = new Kalyna.Algorithm();
            var I = new Block(Kalyna.Encrypt(D, K));
        }
    }
}
