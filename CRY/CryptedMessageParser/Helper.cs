using System.Security.Cryptography;
using CRY.CryptedMessageParser.Exceptions;
using CRY.AlgoLibrary;

namespace CRY.CryptedMessageParser
{
    internal static class Helper
    {
        public static readonly int cryptorCodenameSize = 3;

        public static readonly int hasherCodenameSize = 3;

        public static string GetCodeFromAlgo(IAlgo code)
        {
            if(code is AesAlgo)
            {
                return "aes";
            }
            else if(code is TripleDesAlgo)
            {
                return "3ds";
            }
            else if (code is TwofishAlgo)
            {
                return "2fh";
            }
            else
            {
                throw new UnknownCryptorCodeException(string.Empty);
            }
        }

        public static IAlgo GetAlgoFromCode(string code)
        {
            if(code.Length != cryptorCodenameSize)
            {
                throw new UnknownCryptorCodeException(code);
            }

            if (code.Equals("aes"))
            {
                using AesManaged aesMan = new AesManaged();
                aesMan.GenerateKey();
                aesMan.GenerateIV();
                return new AesAlgo(aesMan.Key, aesMan.IV);
            }
            else if (code.Equals("3ds"))
            {
                using TripleDESCryptoServiceProvider triple = new TripleDESCryptoServiceProvider();
                triple.GenerateKey();
                triple.GenerateIV();
                return new TripleDesAlgo(triple.Key, triple.IV);
            }
            else if (code.Equals("2fh"))
            {
                using AesManaged aesMan = new AesManaged();
                aesMan.GenerateKey();
                aesMan.GenerateIV();
                return new TwofishAlgo(aesMan.Key, aesMan.IV);
            }
            else
            {
                throw new UnknownCryptorCodeException(code);
            }
        }

        public static HashAlgorithm GetHasherFromCode(string code)
        {
            if (code.Length != hasherCodenameSize)
            {
                throw new UnknownHasherCodeException(code);
            }

            if (code.Equals("md5"))
            {
                return MD5.Create();
            }
            else if (code.Equals("sh2"))
            {
                return SHA256.Create();
            }
            else if (code.Equals("sh1"))
            {
                return SHA1.Create();
            }
            else
            {
                throw new UnknownHasherCodeException(code);
            }
        }

        public static string GetCodeFromHasher(HashAlgorithm hasher)
        {
            if (hasher is MD5)
            {
                return "md5";
            }
            else if (hasher is SHA256)
            {
                return "sh2";
            }
            else if (hasher is SHA1)
            {
                return "sh1";
            }
            else
            {
                throw new UnknownHasherCodeException(string.Empty);
            }
        }

        public static bool IsHasherCodeValid(string code)
        {
            return code.Equals("md5") || code.Equals("sh2") || code.Equals("sh1");
        }

        public static bool IsCryptoAlgoCodeValid(string code)
        {
            return code.Equals("aes") || code.Equals("3ds") || code.Equals("2fh");
        }
    }
}