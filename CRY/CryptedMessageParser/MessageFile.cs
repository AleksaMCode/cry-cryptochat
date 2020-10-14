using System.IO;

namespace CRY.CryptedMessageParser
{
    public class MessageFile
    {
        public byte[] Message { get; internal set; }

        public MessageFile(byte[] message)
        {
            this.Message = message;
        }
    }
}
