using System;

namespace CRY.CryptedMessageParser.Exceptions
{
    public class UnknownCryptorCodeException : Exception
    {
        public UnknownCryptorCodeException(string code) : base("Unknown cryptor with code '" + code + "'.")
        {
        }
    }
}