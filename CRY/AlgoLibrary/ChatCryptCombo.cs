using System.Security.Cryptography;

namespace CRY.AlgoLibrary
{
    public class ChatCryptCombo
    {
        public HashAlgorithm Hasher { get; set; }
        public IAlgo Algorithm { get; set; }
        public ChatCryptCombo(HashAlgorithm hash, IAlgo algorithm)
        {
            this.Hasher = hash;
            this.Algorithm = algorithm;
        }
    }
}
