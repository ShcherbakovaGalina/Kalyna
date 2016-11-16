using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kalyna
{
    public class FileEncoderDecoder
    {
        public string PlainTextFileName { private get; set; }
        public string EncryptedTextFileName { private get; set; }
        public string DecryptedTextFileName { private get; set; }

        public Block Key { private get; set; }

        private Random Random { get; } = new Random();
        private int BlocksNumber { get; set; }

        private static string GetFullFilePath(string fileName)
        {
            DirectoryInfo directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;
            return directoryInfo == null ? string.Empty : Path.Combine(directoryInfo.FullName, fileName);
        }

        private void AddByteToBlock(ref byte[] block, byte data)
        {
            var newArray = new byte[block.Length + 1];
            block.CopyTo(newArray, 1);
            newArray[0] = data;
            block = newArray;
        }

        public void Encode()
        {
            var plainFilePath = GetFullFilePath(PlainTextFileName);
            var encryptedFilePath = GetFullFilePath(EncryptedTextFileName);

            if (!File.Exists(plainFilePath)) return;

            var areAddedRandomBytes = false;
            var algorithm = new Algorithm();
            algorithm.GenerateRoundsKeys(Key);
            using (var reader = new BinaryReader(File.Open(plainFilePath, FileMode.Open)))
            using (var writer = new BinaryWriter(File.Open(encryptedFilePath, FileMode.Create)))
            {
                var block = reader.ReadBytes(16);
                var previousBlock = block;
                while (block.Length != 0)
                {
                    BlocksNumber++;
                    if (block.Length < 16)
                    {
                        areAddedRandomBytes = true;
                        var numberOfAddedBytes = (byte)(16 - block.Length);
                        var len = block.Length;
                        for (var i = 0; i < 16 - len - 1; i++)
                            AddByteToBlock(ref block, (byte)Random.Next(255));
                        AddByteToBlock(ref block, numberOfAddedBytes);
                    }

                    var cipherText = algorithm.Encrypt(new Block
                    {
                        Data = new List<byte>(block)
                    }, Key);
                    writer.Write(cipherText.Data.ToArray());
                    previousBlock = block;
                    block = reader.ReadBytes(16);
                }

                if (!areAddedRandomBytes && 1 <= previousBlock[0] && previousBlock[0] <= 16)
                {
                    for (var i = 0; i < 15; i++)
                        AddByteToBlock(ref block, (byte)Random.Next(255));
                    AddByteToBlock(ref block, 16);
                    var cipherText = algorithm.Encrypt(new Block
                    {
                        Data = new List<byte>(block)
                    }, Key);
                    writer.Write(cipherText.Data.ToArray());
                }
            }
        }

        public void Decode()
        {
            var encryptedFilePath = GetFullFilePath(EncryptedTextFileName);
            var decryptedFilePath = GetFullFilePath(DecryptedTextFileName);

            if (!File.Exists(encryptedFilePath)) return;

            var algorithm = new Algorithm();
            algorithm.GenerateRoundsKeys(Key);
            using (var reader = new BinaryReader(File.Open(encryptedFilePath, FileMode.Open)))
            using (var writer = new BinaryWriter(File.Open(decryptedFilePath, FileMode.Create)))
            {
                var block = reader.ReadBytes(16);
                var nextBlock = reader.ReadBytes(16);
                while (block.Length != 0)
                {
                    var plainText = algorithm.Decrypt(new Block
                    {
                        Data = new List<byte>(block)
                    }, Key);
                    if (nextBlock.Length == 0 && plainText.Data[0] <= 16)
                    {
                        var numberOfAddedBytes = plainText.Data[0];
                        if (numberOfAddedBytes != 16)
                            writer.Write(plainText.Data.Where((d, idx) => numberOfAddedBytes <= idx).ToArray());
                    }
                    else
                        writer.Write(plainText.Data.ToArray());
                    block = nextBlock;
                    nextBlock = reader.ReadBytes(16);
                }
            }
        }
    }
}
