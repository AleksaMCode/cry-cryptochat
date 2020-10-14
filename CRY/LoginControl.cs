using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CRY.AlgoLibrary;
using CRY.PrivateKeyParsers;
using CRY.UserSQLManager;

namespace CRY
{
    public class LoginControl
    {
        private readonly string privateKeyPath;
        private readonly string userDatabasePath = @"C:\Users\Majkic\source\repos\Cryptography\CRY\Users.db";

        public LoginControl(string privateKey)
        {
            this.privateKeyPath = privateKey;
        }

        public UserInformation Login(string username, string password)
        {
            UserDatabase data = new UserDatabase(this.userDatabasePath);
            var user = data.GetUser(username);

            if (user != null && user.IsPasswordValid(password))
            {
                var userCert = new X509Certificate2(user.PublicCertificate);

                if (userCert == null)
                {
                    throw new Exception("Certificate error.");
                }

                if (CertificateValidator.VerifyCertificate(userCert) == false)
                {
                    throw new Exception("Certificate is invalid.");
                }

                byte[] keyRaw = File.ReadAllBytes(this.privateKeyPath);
                var privateParameters = new KeyFileParser(keyRaw).GetParameters();
                RSACryptoServiceProvider publicKeyProvider = (RSACryptoServiceProvider)userCert.PublicKey.Key;
                if (!RsaAlgo.AreKeysMatched(publicKeyProvider.ExportParameters(false), privateParameters))
                {
                    throw new Exception("The given private key does not match this user's certificate.");
                }

                return new UserInformation(user, privateParameters);
            }
            else
            {
                throw new Exception("Invalid username or password.");
            }
        }
    }
}
