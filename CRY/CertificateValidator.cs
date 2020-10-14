using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace CRY
{
    public static class CertificateValidator
    {
        public static bool VerifyCertificate(X509Certificate2 certificateToValidate)
        {
            using (X509Chain chain = new X509Chain())
            {
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EndCertificateOnly;
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                chain.ChainPolicy.VerificationTime = DateTime.Now;
                //chain.ChainPolicy.ExtraStore.Add(new X509Certificate2(@"C:\Users\Majkic\source\repos\Cryptography\CRY\openssl\rootCA.pem"));

                bool isChainValid = chain.Build(certificateToValidate);

                if (!isChainValid)
                {
                    return false;
                }

                //// Using PKI Library - using PKI.OCSP;
                //X509CRL2 crl = new X509CRL2(crlRawData);
                //X509CRLEntry entry = crl.RevokedCertificates[cert.SerialNumber];
                //if (entry != null) // NULL would mean that the certificate is not revoked.
                //{                       

                return true;
            }
        }

        public static bool VerifyKeyUsage(X509Certificate2 cert)
        {
            List<X509KeyUsageExtension> extensions = cert.Extensions.OfType<X509KeyUsageExtension>().ToList();
            if (!extensions.Any())
            {
                return cert.Version < 3;
            }

            List<X509KeyUsageFlags> keyUsageFlags = extensions.Select((ext) => ext.KeyUsages).ToList();
            return keyUsageFlags.Contains(X509KeyUsageFlags.KeyEncipherment) && keyUsageFlags.Contains(X509KeyUsageFlags.DigitalSignature);
        }
    }
}
