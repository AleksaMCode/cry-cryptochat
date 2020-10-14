using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using CRY.AlgoLibrary;

namespace CRY.CryptedMessageParser
{
    public class MessageCryptor
    {
        public RSAParameters SenderPrivateKey { get; set; }
        public RSAParameters ReceiverPublicKey { get; set; }

        public MessageCryptor(RSAParameters senderPrivateKey, RSAParameters receiverPublicKey)
        {
            this.SenderPrivateKey = senderPrivateKey;
            this.ReceiverPublicKey = receiverPublicKey;
        }

        public void Encrypt(MessageFile message, Stream output, ChatCryptCombo algs, byte[] key, byte[] iv)
        {
            BinaryWriter writer = new BinaryWriter(output); // to write to output
            writer.Seek(0, SeekOrigin.Begin); // write to beginning, this will override if there is something there

            byte[] encMessage = new byte[] { };
            switch (Helper.GetCodeFromAlgo(algs.Algorithm))
            {
                case "aes":
                    {
                        AesAlgo cryptor = new AesAlgo(key, iv);
                        encMessage = encMessage.Concat(cryptor.Encrypt(message.Message)).ToArray();
                        break;
                    }
                case "3ds":
                    {
                        TripleDesAlgo cryptor = new TripleDesAlgo(key, iv);
                        encMessage = encMessage.Concat(cryptor.Encrypt(message.Message)).ToArray();
                        break;
                    }
                case "2fh":
                    {
                        TwofishAlgo cryptor = new TwofishAlgo(key, iv);
                        encMessage = encMessage.Concat(cryptor.Encrypt(message.Message)).ToArray();
                        break;
                    }
            }

            byte[] header = new byte[2];
            byte[] lenByte = BitConverter.GetBytes(encMessage.Length);
            Array.Copy(lenByte, 0, header, 0, 2); // convert length to byte array
            byte[] headerPlusMessage = new byte[2 + encMessage.Length];
            Array.Copy(header, 0, headerPlusMessage, 0, 2);
            Array.Copy(encMessage, 0, headerPlusMessage, 2, encMessage.Length);

            var contentHashAggregate = algs.Hasher.ComputeHash(headerPlusMessage);

            writer.Write(headerPlusMessage);

            byte[] rsaSignature = new RsaAlgo(this.SenderPrivateKey).Sign(contentHashAggregate, algs.Hasher);

            writer.Write(rsaSignature);
        }
    }
}
