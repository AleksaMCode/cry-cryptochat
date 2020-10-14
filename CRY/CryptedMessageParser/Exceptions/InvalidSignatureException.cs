using System;

namespace CRY.CryptedMessageParser.Exceptions
{
    public class InvalidSignatureException : Exception
    {
        public InvalidSignatureException(string msg) : base(msg + " Message Signature is invalid.")
        {
        }
    }
}