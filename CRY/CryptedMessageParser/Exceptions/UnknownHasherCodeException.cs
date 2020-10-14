using System;

namespace CRY.CryptedMessageParser.Exceptions
{
    public class UnknownHasherCodeException : Exception
    {
        public UnknownHasherCodeException(string code) : base("Unknown hasher with code '" + code + "'.")
        {
        }
    }
}