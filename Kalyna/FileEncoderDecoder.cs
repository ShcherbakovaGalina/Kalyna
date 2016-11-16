using System.Collections.Generic;
using System.IO;

namespace Kalyna
{
    public class FileEncoderDecoder
    {
        public string FileName { get; set; }

        public Block Key { get; set; }

        private static string GetFullFilePath(string fileName)
        {
            DirectoryInfo directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;
            return directoryInfo == null ? string.Empty : Path.Combine(directoryInfo.FullName, fileName);
        }

        public void Encode()
        {
            var filePath = GetFullFilePath(FileName);

            if (!File.Exists(filePath)) return;

            var algorithm = new Algorithm();
            algorithm.GenerateRoundsKeys(Key);
            using (var reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            using (var writer = new BinaryWriter(File.Open(filePath + "1", FileMode.Create)))
            {
                var plainText = reader.ReadBytes(16);
                while (plainText.Length != 0)
                {
                    var cipherText =algorithm.Encrypt(new Block
                    {
                        Data = new List<byte>(plainText)
                    }, Key);
                    writer.Write(cipherText.Data.ToArray());
                    plainText = reader.ReadBytes(16);
                }
            }
        }

        public void Decode()
        {
            var filePath = GetFullFilePath(FileName);

            if (!File.Exists(filePath)) return;

            var algorithm = new Algorithm();
            using (var reader = new BinaryReader(File.Open(filePath + "1", FileMode.Open)))
            using (var writer = new BinaryWriter(File.Open(filePath + "2", FileMode.Create)))
            {
                var cipherText = reader.ReadBytes(16);
                while (cipherText.Length != 0)
                {
                    var plainText = algorithm.Decrypt(new Block
                    {
                        Data = new List<byte>(cipherText)
                    }, Key);
                    writer.Write(plainText.Data.ToArray());
                    cipherText = reader.ReadBytes(16);
                }
            }
        }
    }
}
