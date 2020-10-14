using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CRY.AlgoLibrary;
using CRY.CryptedMessageParser;
using CRY.UserSQLManager;

namespace CRY
{
    public class Decryptor
    {
        private readonly User sender;
        private readonly UserInformation currentUser;
        private readonly ChatCryptCombo combo;

        public Decryptor(User sender, UserInformation currentUser, ChatCryptCombo combo)
        {
            this.sender = sender;
            this.currentUser = currentUser;
            this.combo = combo;
        }

        public void DecryptFile(EncryptedMessage input, ref byte[] output, byte[] key, byte[] iv)
        {
            var cert = new X509Certificate2(this.sender.PublicCertificate);
           
            if (CertificateValidator.VerifyCertificate(cert) == false)
            {
                throw new Exception("Certificate is invalid.\nCan't verify the message signature.");
            }

            MessageDecryptor decryptor = new MessageDecryptor(((RSACryptoServiceProvider)(cert).PublicKey.Key).ExportParameters(false));
            try
            {
                decryptor.Decrypt(input, ref output, this.combo, key, iv);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
