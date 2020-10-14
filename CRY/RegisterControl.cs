using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using CRY.UserSQLManager;

namespace CRY
{
    public class RegisterControl
    {
        private readonly UserDatabase data;

        public RegisterControl(UserDatabase db)
        {
            this.data = db;
        }

        internal void Register(string username, string password, string certificateFilePath)
        {
            X509Certificate2 cert = new X509Certificate2(certificateFilePath);

            if (CertificateValidator.VerifyCertificate(cert) == false)
            {
                throw new Exception("Certificate is invalid.");
            }
            else if (CertificateValidator.VerifyKeyUsage(cert) == false)
            {
                throw new Exception("Certificate must have 'digitalSignature' and 'keyEncipherment' set as it's key usage.");
            }

            this.data.AddUser(username, password, File.ReadAllBytes(certificateFilePath));
        }
    }
}
