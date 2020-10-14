using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Linq;

namespace CRY.AlgoLibrary
{
    public class AesAlgo : IAlgo
    {
        public static readonly string signature = "aes";
        public byte[] Key { get; set; }
        public byte[] InitVec { get; set; }
        public int BlockSize { get; } = 16384;
        public byte[] AdditionalData { get => this.InitVec; }

        /// <summary>
        /// Problem with c# func. Random(). Always returning same values.
        /// </summary>
        [Obsolete("AesAlgo() is deprecated, use AesAlgo(byte[], byte[]) instead.", true)]
        public AesAlgo()
        {
            this.Key = new byte[32];
            new Random().NextBytes(this.Key);
            this.InitVec = new byte[16];
            new Random().NextBytes(this.InitVec);
        }
        public AesAlgo(byte[] key, byte[] initVec)
        {
            this.Key = key;
            this.InitVec = initVec;
        }
        public byte[] Encrypt(byte[] message)
        {
            using AesManaged aes = new AesManaged();
            using var encryptor = aes.CreateEncryptor(this.Key, this.InitVec);
            using MemoryStream ms = new MemoryStream();

            using CryptoStream writer = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            writer.Write(message, 0, message.Length);
            writer.FlushFinalBlock();
            return ms.ToArray();
        }
        public byte[] Decrypt(byte[] message)
        {
            using AesManaged aes = new AesManaged();
            using var decryptor = aes.CreateDecryptor(this.Key, this.InitVec);
            using MemoryStream ms = new MemoryStream(message);

            using CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            var decrypted = new byte[message.Length];
            var bytesRead = cs.Read(decrypted, 0, decrypted.Length);

            return decrypted.Take(bytesRead).ToArray();
        }
        public string GetSignatureString()
        {
            return signature;
        }
    }
}
