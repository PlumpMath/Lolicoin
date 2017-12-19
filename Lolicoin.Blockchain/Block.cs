using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace Lolicoin.BlockChain
{
    [Serializable]
    public class Block
    {
        public Block(int index, TimeSpan timeSpan, BlockData data, string previousHash = "")
        {
            Index        = index;
            Timespan     = timeSpan;
            Data         = data;
            PreviousHash = previousHash;
            Hash         = CalculateHash();
            Nonce        = 0;
        }

        public int Index           { get; }
        public TimeSpan Timespan   { get; }
        public BlockData Data      { get; set; }
        public string PreviousHash { get; set; }
        public string Hash         { get; set; }
        public int Nonce           { get; set; }

        public string CalculateHash()
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] calculatedHash =
                    sha256.ComputeHash(
                        ObjectToByteArray(Index)
                            .Concat(ObjectToByteArray(Timespan))
                            .Concat(ObjectToByteArray(Data))
                            .Concat(ObjectToByteArray(Nonce))
                            .ToArray());

                var builder = new StringBuilder();

                for (int i = 0; i < calculatedHash.Length; i++)
                {
                    builder.Append(calculatedHash[i].ToString("x2").ToLower());
                }

                return builder.ToString();
            }
        }

        public string MineCoin(int difficulty)
        {
            var hash = new string(' ', difficulty);

            while (!hash.StartsWith(new string('0', difficulty + 1)))
            {
                Nonce++;
                hash = CalculateHash();

                Debug.WriteLine($"Calculating Hash: {hash}");
            }

            return hash;
        }

        private byte[] ObjectToByteArray(object obj)
        {
            if (obj == null) return null;

            var formatter = new BinaryFormatter();

            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, obj);

                return ms.ToArray();
            }
        }
    }
}