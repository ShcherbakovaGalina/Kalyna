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
        public string KeyFileName { private get; set; }

        private Random Random { get; } = new Random();
        private int BlocksNumber { get; set; }

        private static string GetFullFilePath(string fileName)
        {
            DirectoryInfo directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;
            return directoryInfo == null ? string.Empty : Path.Combine(directoryInfo.FullName, fileName);
        }

        private static void AddByteToBlock(ref byte[] block, byte data)
        {
            var newArray = new byte[block.Length + 1];
            block.CopyTo(newArray, 1);
            newArray[0] = data;
            block = newArray;
        }

        private Block GetKey()
        {
            //var key = new Block
            //{
            //    Data = new List<byte>
            //    {
            //        15, 14, 13, 12, 11, 10, 9, 8,
            //        7, 6, 5, 4, 3, 2, 1, 0
            //    }
            //};

            var keyFilePath = GetFullFilePath(KeyFileName);

            if (!File.Exists(keyFilePath)) return null;

            using (var reader = new BinaryReader(File.Open(keyFilePath, FileMode.Open)))
            {
                var key = reader.ReadBytes(16);
                return key.Length != 16 ? null : new Block { Data = new List<byte>(key) };
            }
        }

        public void Encode()
        {
            var plainFilePath = GetFullFilePath(PlainTextFileName);
            var encryptedFilePath = GetFullFilePath(EncryptedTextFileName);

            if (!File.Exists(plainFilePath)) return;

            var algorithm = new Algorithm();
            var key = GetKey();
            algorithm.GenerateRoundsKeys(key);

            var areAddedRandomBytes = false;
            using (var reader = new BinaryReader(File.Open(plainFilePath, FileMode.Open)))
            using (var writer = new BinaryWriter(File.Open(encryptedFilePath, FileMode.Create)))
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var fileSize = 0;
                var block = reader.ReadBytes(16);
                var previousBlock = block;
                while (block.Length != 0)
                {
                    fileSize += block.Length;
                    BlocksNumber++;
                    if (block.Length < 16)
                    {
                        areAddedRandomBytes = true;
                        var numberOfAddedBytes = (byte)(16 - block.Length);
                        var len = block.Length;
                        for (var i = 0; i < 16 - len - 1; i++)
                            AddByteToBlock(ref block, (byte)Random.Next(255));
                        AddByteToBlock(ref block, numberOfAddedBytes);
                        fileSize += numberOfAddedBytes;
                    }

                    var cipherText = algorithm.Encrypt(new Block
                    {
                        Data = new List<byte>(block)
                    }, key);
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
                    }, key);
                    writer.Write(cipherText.Data.ToArray());
                }

                watch.Stop();
                Console.WriteLine($"Encryption completed\nTime: {watch.Elapsed}\nFile size: {fileSize} Bytes" +
                                  $"\nSpeed: {(int)(fileSize / watch.Elapsed.TotalSeconds)} Bytes per second");
            }
        }

        public void Decode()
        {
            var encryptedFilePath = GetFullFilePath(EncryptedTextFileName);
            var decryptedFilePath = GetFullFilePath(DecryptedTextFileName);

            if (!File.Exists(encryptedFilePath)) return;

            var algorithm = new Algorithm();
            var key = GetKey();
            algorithm.GenerateRoundsKeys(key);
            using (var reader = new BinaryReader(File.Open(encryptedFilePath, FileMode.Open)))
            using (var writer = new BinaryWriter(File.Open(decryptedFilePath, FileMode.Create)))
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var fileSize = 0;
                var block = reader.ReadBytes(16);
                var nextBlock = reader.ReadBytes(16);
                while (block.Length != 0)
                {
                    fileSize += block.Length;
                    var plainText = algorithm.Decrypt(new Block
                    {
                        Data = new List<byte>(block)
                    }, key);
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

                watch.Stop();
                Console.WriteLine($"\nDecryption completed\nTime: {watch.Elapsed}\nFile size: {fileSize} Bytes" +
                                  $"\nSpeed: {(int)(fileSize / watch.Elapsed.TotalSeconds)} Bytes per second");
            }
        }
    }
}
