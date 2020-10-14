using System;
using System.Linq;
using System.Security.Cryptography;

namespace CRY.AlgoLibrary
{
    public class RsaAlgo : IAlgo
    {
        public static readonly string signature = "rsa";
        public RSAParameters Key { get; set; }
        byte[] IAlgo.Key => null;
        public int BlockSize { get; } = 115;
        public byte[] AdditionalData { get => null; }

        public RsaAlgo(RSAParameters rsaKey)
        {
            this.Key = rsaKey;
        }
        public static bool AreKeysMatched(RSAParameters publicKey, RSAParameters privateKey)
        {
            byte[] data = new byte[10];
            new Random().NextBytes(data);

            using RSACryptoServiceProvider decryptRSA = new RSACryptoServiceProvider();
            using RSACryptoServiceProvider encryptRSA = new RSACryptoServiceProvider();
            encryptRSA.ImportParameters(publicKey);
            decryptRSA.ImportParameters(privateKey);

            var decrypted = decryptRSA.Decrypt(encryptRSA.Encrypt(data, false), false);

            if (data.SequenceEqual(decrypted))
            {
                return true;
            }

            return false;
        }
        public byte[] Decrypt(byte[] message)
        {
            byte[] decryptedData;

            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();
            rsaProvider.ImportParameters(this.Key);
            decryptedData = rsaProvider.Decrypt(message, false);

            return decryptedData;
        }
        public byte[] Encrypt(byte[] message)
        {
            byte[] encryptedData;

            using RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();
            rsaProvider.ImportParameters(this.Key);
            encryptedData = rsaProvider.Encrypt(message, false);

            return encryptedData;
        }
        public byte[] Sign(byte[] data, HashAlgorithm hasher)
        {
            using var rsaProvider = new RSACryptoServiceProvider();
            rsaProvider.ImportParameters(this.Key);
            return rsaProvider.SignData(data, hasher);
        }
        public bool CheckSignature(byte[] data, HashAlgorithm hasher, byte[] signature)
        {
            using var rsaProvider = new RSACryptoServiceProvider();
            rsaProvider.ImportParameters(this.Key);
            return rsaProvider.VerifyData(data, hasher, signature);
        }
        public string GetSignatureString()
        {
            return signature;
        }
    }
}
