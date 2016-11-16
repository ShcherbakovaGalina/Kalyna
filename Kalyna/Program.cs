using System.Collections.Generic;

namespace Kalyna
{
    internal static class Program
    {
        private static void Main()
        {
            var key = new Block
            {
                Data = new List<byte>
                {
                    15, 14, 13, 12, 11, 10, 9, 8,
                    7, 6, 5, 4, 3, 2, 1, 0
                }
            };

            var f = new FileEncoderDecoder
            {
                PlainTextFileName = "Files\\Plain.txt",
                EncryptedTextFileName = "Files\\Encrypted.txt",
                DecryptedTextFileName = "Files\\Decrypted.txt",
                KeyFileName = "Files\\Key.txt"
            };
            f.Encode();
            f.Decode();
        }
    }
}
