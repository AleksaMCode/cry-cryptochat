namespace CRY.CryptedMessageParser
{
    public class EncryptedMessage
    {
        internal readonly byte[] encMessage;
        public byte[] Signature { get; set; }
        public int HeaderLength { get => 2; }
        internal EncryptedMessage(byte[] stream)
        {
            this.encMessage = stream;
        }
    }
}