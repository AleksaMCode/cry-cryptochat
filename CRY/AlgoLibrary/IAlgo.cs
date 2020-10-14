using System;
using System.Collections.Generic;
using System.Text;

namespace CRY.AlgoLibrary
{
    public  interface IAlgo
    {
        int BlockSize { get; }
        byte[] Key { get; }
        byte[] AdditionalData { get; }
        byte[] Encrypt(byte[] message);
        byte[] Decrypt(byte[] message);
        string GetSignatureString();
    }
}
