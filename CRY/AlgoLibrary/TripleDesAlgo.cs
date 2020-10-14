using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace CRY.AlgoLibrary
{
    public class TripleDesAlgo : IAlgo
    {
        public static readonly string signature = "3ds";
        public byte[] Key { get; set; }
        public byte[] InitVec { get; set; }
        public int BlockSize { get; } = 115;
        public byte[] AdditionalData { get => this.InitVec; }

        /// <summary>
        /// Problem with c# func. Random(). Always returning same values.
        /// </summary>
        [Obsolete("TripleDesAlgo() is deprecated, use TripleDesAlgo(byte[], byte[]) instead.", true)]
        public TripleDesAlgo()
        {
            this.Key = new byte[24];
            new Random().NextBytes(this.Key);
            this.InitVec = new byte[16];
            new Random().NextBytes(this.InitVec);
        }
        public TripleDesAlgo(byte[] key, byte[] initVec)
        {
            this.Key = key;
            this.InitVec = initVec;
        }
        public byte[] Decrypt(byte[] message)
        {
            using TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            using var decryptor = tdes.CreateDecryptor(this.Key, this.InitVec);
            using MemoryStream ms = new MemoryStream(message);

            using CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            var decrypted = new byte[message.Length];
            var bytesRead = cs.Read(decrypted, 0, message.Length);

            return decrypted.Take(bytesRead).ToArray();
        }
        public byte[] Encrypt(byte[] content)
        {
            using TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            using ICryptoTransform encryptor = tdes.CreateEncryptor(this.Key, this.InitVec);
            using MemoryStream ms = new MemoryStream();

            using CryptoStream writer = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            writer.Write(content, 0, content.Length);
            writer.FlushFinalBlock();
            return ms.ToArray();
        }
        public string GetSignatureString()
        {
            return signature;
        }
    }
}
