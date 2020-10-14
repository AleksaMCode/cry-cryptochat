using System;
using System.Security.Cryptography;
using CRY.AlgoLibrary;
using System.Linq;

namespace CRY.CryptedMessageParser
{
    public class MessageDecryptor
    {
        public RSAParameters Key { get; set; }

        public MessageDecryptor(RSAParameters publicKey)
        {
            this.Key = publicKey;
        }

        public void Decrypt(EncryptedMessage file, ref byte[] message, ChatCryptCombo algs, byte[] key, byte[] iv)
        {
            if (EncryptedMessageParser.VerifySignature(file, Key, algs))
            {
                byte[] header = new byte[2];
                Array.Copy(file.encMessage, 0, header, 0, 2);
                int messageLen = (Int32)BitConverter.ToInt16(header, 0);

                byte[] encMessage = new byte[messageLen];
                Array.Copy(file.encMessage, 2, encMessage, 0, encMessage.Length);

                switch (Helper.GetCodeFromAlgo(algs.Algorithm))
                {
                    case "aes":
                        {
                            AesAlgo cryptor = new AesAlgo(key, iv);
                            message = message.Concat(cryptor.Decrypt(encMessage)).ToArray();
                            break;
                        }
                    case "3ds":
                        {
                            TripleDesAlgo cryptor = new TripleDesAlgo(key, iv);
                            message = message.Concat(cryptor.Decrypt(encMessage)).ToArray();
                            break;
                        }
                    case "2fh":
                        {
                            TwofishAlgo cryptor = new TwofishAlgo(key, iv);
                            message = message.Concat(cryptor.Decrypt(encMessage)).ToArray();
                            break;
                        }
                }
            }
            else
            {
                throw new CryptedMessageParser.Exceptions.InvalidSignatureException("Message from sender is Corrupted.");
            }
        }
    }
}