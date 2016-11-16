namespace Kalyna
{
    internal static class Program
    {
        private static void Main()
        {
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
