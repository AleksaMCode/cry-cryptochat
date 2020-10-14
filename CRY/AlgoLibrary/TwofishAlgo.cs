using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace CRY.AlgoLibrary
{
    public class TwofishAlgo : IAlgo
    {
        public static readonly string signature = "2fh";
        public byte[] Key { get; set; }
        public byte[] Salt { get; set; }
        public int BlockSize { get; } = 16384;
        public byte[] AdditionalData { get => this.Salt; }

        /// <summary>
        /// Problem with c# func. Random(). Always returning same values.
        /// </summary>
        [Obsolete("TwofishAlgo() is deprecated, use TwofishAlgo(byte[], byte[]) instead.", true)]
        public TwofishAlgo()
        {
            this.Key = new byte[32];
            new Random().NextBytes(this.Key);
            this.Salt = new byte[16];
            new Random().NextBytes(this.Salt);
        }
        public TwofishAlgo(byte[] key, byte[] salt)
        {
            this.Key = key;
            this.Salt = salt;
        }
        public byte[] Decrypt(byte[] message)
        {
            Sha3Digest sha3Digest = new Sha3Digest();
            Pkcs5S2ParametersGenerator gen = new Pkcs5S2ParametersGenerator(sha3Digest);
            gen.Init(this.Key, this.Salt, 1000);
            KeyParameter param = (KeyParameter)gen.GenerateDerivedParameters(new TwofishEngine().AlgorithmName, 256);

            var blockCipher = new TwofishEngine();
            var padding = new Pkcs7Padding();
            try
            {
                var cipher = padding == null ?
                new PaddedBufferedBlockCipher(blockCipher) : new PaddedBufferedBlockCipher(blockCipher, padding);
                cipher.Init(false, param);
                return cipher.DoFinal(message);
            }
            catch (CryptoException)
            {
            }

            return null;
        }
        public byte[] Encrypt(byte[] message)
        {
            Sha3Digest sha3Digest = new Sha3Digest();
            Pkcs5S2ParametersGenerator gen = new Pkcs5S2ParametersGenerator(sha3Digest);
            gen.Init(this.Key, this.Salt, 1000);
            KeyParameter param = (KeyParameter)gen.GenerateDerivedParameters(new TwofishEngine().AlgorithmName, 256);
            var blockCipher = new TwofishEngine();
            var padding = new Pkcs7Padding();

            try
            {
                var cipher = padding == null ?
                new PaddedBufferedBlockCipher(blockCipher) : new PaddedBufferedBlockCipher(blockCipher, padding);
                cipher.Init(true, param);
                return cipher.DoFinal(message);
            }
            catch (CryptoException)
            {
            }

            return null;
        }
        public string GetSignatureString()
        {
            return signature;
        }
    }
}
