namespace Kalyna
{
    internal static class Program
    {
        private static void Main()
        {
            var f = new FileEncoderDecoder
            {
                PlainTextFileName = "Files\\Plain.txt",
                EncryptedTextFileName = "Files\\Encrypted2.txt",
                DecryptedTextFileName = "Files\\Decrypted2.txt",
                KeyFileName = "Files\\Key.txt"
            };
            f.Encode();
            f.Decode();
        }
    }
}
