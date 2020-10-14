using CRY.UserSQLManager;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CRY.Tests
{
    [TestClass]
    public class DatabaseTests
    {
        [TestMethod]
        public void RegisterUsers()
        {
            UserDatabase db = new UserDatabase(@"C:\Users\Majkic\source\repos\Cryptography\CRY\Users.db");
            db.AddUser("aleksa", "aleksa", File.ReadAllBytes(@"C:\Users\Majkic\source\repos\Cryptography\CRY\openssl\certs\aleksa-cert.pem"));
            db.AddUser("majkic", "majkic", File.ReadAllBytes(@"C:\Users\Majkic\source\repos\Cryptography\CRY\openssl\certs\majkic-cert.pem"));
            db.AddUser("aleksamajkic", "aleksamajkic", File.ReadAllBytes(@"C:\Users\Majkic\source\repos\Cryptography\CRY\openssl\certs\aleksamajkic-cert.pem"));
        }
    }
}
