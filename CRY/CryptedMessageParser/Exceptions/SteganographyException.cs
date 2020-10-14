using System;

namespace CRY.CryptedMessageParser.Exceptions
{
    public class SteganographyException : Exception
    {
        public SteganographyException(string reason) : base(reason)
        {

        }
    }
}
