using System;
using System.IO;
using System.Security.Cryptography;
using CRY.AlgoLibrary;

namespace CRY.CryptedMessageParser
{
    public class EncryptedMessageParser
    {
        public static EncryptedMessage Parse(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);

            reader.BaseStream.Position = 0;
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            byte[] encMesageByteLength = new byte[2];
            reader.Read(encMesageByteLength, 0, encMesageByteLength.Length);

            int encMesageLength = (Int32)BitConverter.ToInt16(encMesageByteLength, 0);

            byte[] encMesage = new byte[2 + encMesageLength];
            Array.Copy(encMesageByteLength, 0, encMesage, 0, 2);
            reader.Read(encMesage, 2, encMesage.Length - 2);

            EncryptedMessage message = new EncryptedMessage(encMesage);

            message.Signature = reader.ReadBytes((int)stream.Length - 2 - encMesageLength);

            return message;
        }

        public static bool VerifySignature(EncryptedMessage file, RSAParameters key, ChatCryptCombo algs)
        {
            byte[] header = new byte[file.HeaderLength];
            Array.Copy(file.encMessage, 0, header, 0, header.Length);
            int messageLen = (Int32)BitConverter.ToInt16(header, 0);

            byte[] headerPlusMessage = new byte[header.Length + messageLen];
            Array.Copy(header, 0, headerPlusMessage, 0, header.Length);
            Array.Copy(file.encMessage, header.Length, headerPlusMessage, header.Length, messageLen);
            var contentHashAggregate = algs.Hasher.ComputeHash(headerPlusMessage);

            return new RsaAlgo(key).CheckSignature(contentHashAggregate, algs.Hasher, file.Signature);
        }
    }
}