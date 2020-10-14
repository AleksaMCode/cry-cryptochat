using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CRY.AlgoLibrary;
using CRY.CryptedMessageParser;
using CRY.UserSQLManager;

namespace CRY
{
    public class Encryptor
    {
        private readonly User receiver;
        private readonly UserInformation currentUser;
        private readonly ChatCryptCombo combo;

        public Encryptor(User receiverUsername, UserInformation currentUser, ChatCryptCombo combo)
        {
            this.receiver = receiverUsername;
            this.currentUser = currentUser;
            this.combo = combo;
        }

        public void EncryptFile(MessageFile input, Stream output, byte[] key, byte[] iv)
        {
            var cert = new X509Certificate2(this.receiver.PublicCertificate);

            EncryptFile(cert, input, output, key, iv);
        }

        public void EncryptFile(X509Certificate2 cert, MessageFile input, Stream output, byte[] key, byte[] iv)
        {
            MessageCryptor cryptor = new MessageCryptor(this.currentUser.PrivateKey, ((RSACryptoServiceProvider)cert.PublicKey.Key).ExportParameters(false));
            cryptor.Encrypt(input, output, this.combo, key, iv);
        }
    }
}
