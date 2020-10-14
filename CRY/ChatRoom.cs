using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using CRY.AlgoLibrary;
using CRY.CryptedMessageParser;
using CRY.UserSQLManager;

namespace CRY
{
    public class ChatRoom
    {
        /// <summary>
        /// Sender
        /// </summary>
        public string User1 { get; set; }

        /// <summary>
        /// Receiver
        /// </summary>
        public string User2 { get; set; }

        private readonly int nameOffset = 40;
        public ChatCryptCombo CryptoTypes { get; set; }

        public byte[] Key { get; set; } = null;

        /// <summary>
        /// IV for AES and DES3
        /// Salt for TwofishAlgo
        /// </summary>
        public byte[] IVorSalt { get; set; } = null;
        public string EncAlgo { get; set; }
        public string HashAlgo { get; set; }

        public readonly string chatFilePath = @"C:\Users\Majkic\source\repos\Cryptography\CRY\chat\chat.bin";

        private byte[] chatFile = null;

        //     0        1         2        3        4    
        // ┏━━━━━━━━┳━━━━━━━━┳━━━━━━━━┳━━━━━━━━┳━━━━━━━━┓
        // ┃ HAND-  ┃  ACK   ┃   ON   ┃  FIN   ┃  OFF   ┃
        // ┃ SHAK   ┃        ┃        ┃        ┃        ┃
        // ┗━━━━━━━━┻━━━━━━━━┻━━━━━━━━┻━━━━━━━━┻━━━━━━━━┛
        private byte[] ConnectionFlagsArray { get; set; } = new byte[1];

        /// <summary>
        /// Initially all bits set to false.
        /// </summary>
        private BitArray ConnectionFlags { get; set; } = new BitArray(8);

        public ChatRoom(string user1, string user2, string encAlgo, string hashAlgo)
        {
            this.User1 = user1;
            this.User2 = user2;
            this.EncAlgo = encAlgo;
            this.HashAlgo = hashAlgo;
            Array.Clear(ConnectionFlagsArray, 0, ConnectionFlagsArray.Length);

            CryptoTypes = new ChatCryptCombo(Helper.GetHasherFromCode(hashAlgo), Helper.GetAlgoFromCode(encAlgo));

            if (Helper.GetCodeFromAlgo(CryptoTypes.Algorithm) == "aes")
            {
                Key = new byte[((AesAlgo)CryptoTypes.Algorithm).Key.Length];
                Array.Copy(((AesAlgo)CryptoTypes.Algorithm).Key, Key, Key.Length);

                IVorSalt = new byte[((AesAlgo)CryptoTypes.Algorithm).InitVec.Length];
                Array.Copy(((AesAlgo)CryptoTypes.Algorithm).InitVec, IVorSalt, IVorSalt.Length);
            }
            else if (Helper.GetCodeFromAlgo(CryptoTypes.Algorithm) == "3ds")
            {
                Key = new byte[((TripleDesAlgo)CryptoTypes.Algorithm).Key.Length];
                Array.Copy(((TripleDesAlgo)CryptoTypes.Algorithm).Key, Key, Key.Length);

                IVorSalt = new byte[((TripleDesAlgo)CryptoTypes.Algorithm).InitVec.Length];
                Array.Copy(((TripleDesAlgo)CryptoTypes.Algorithm).InitVec, IVorSalt, IVorSalt.Length);
            }
            else // TwofishAlgo
            {
                Key = new byte[((TwofishAlgo)CryptoTypes.Algorithm).Key.Length];
                Array.Copy(((TwofishAlgo)CryptoTypes.Algorithm).Key, Key, Key.Length);

                IVorSalt = new byte[((TwofishAlgo)CryptoTypes.Algorithm).Salt.Length];
                Array.Copy(((TwofishAlgo)CryptoTypes.Algorithm).Salt, IVorSalt, IVorSalt.Length);
            }
            WriteChatFile();
        }

        public ChatRoom(RSAParameters privKey)
        {
            Array.Clear(ConnectionFlagsArray, 0, ConnectionFlagsArray.Length);
            ParseChatFile(privKey);
        }

        public ChatRoom()
        {
            Array.Clear(ConnectionFlagsArray, 0, ConnectionFlagsArray.Length);
            byte[] file = File.ReadAllBytes(chatFilePath);
            ParseUsersNames(file);
        }

        public void BitsToByte()
        {
            ConnectionFlags.CopyTo(ConnectionFlagsArray, 0);
        }

        public void FlushConnectionFlags()
        {
            using Mutex mutex = new Mutex(false, "FlushConnectionFlags() Mutex");
            BitsToByte();
            
            mutex.WaitOne();
            Array.Copy(ConnectionFlagsArray, 0, chatFile, nameOffset, ConnectionFlagsArray.Length);
            using FileStream stream = new FileStream(chatFilePath, FileMode.Create);
            using BinaryWriter writter = new BinaryWriter(stream);
            writter.Write(chatFile);
            mutex.ReleaseMutex();
        }

        public void SetHandshakeFlag()
        {
            ConnectionFlags[0] = true; // ConnectionFlags.Set(0, true);
            FlushConnectionFlags();
        }

        public void SetAckFlag()
        {
            ConnectionFlags[1] = true;
            FlushConnectionFlags();
        }

        public void SetOnFlag()
        {
            ConnectionFlags[2] = true;
            FlushConnectionFlags();
        }

        public void SetFinFlag()
        {
            ConnectionFlags[3] = true;
            FlushConnectionFlags();
        }

        public void SetOffFlag()
        {
            ConnectionFlags[4] = true;
            FlushConnectionFlags();
        }

        public Boolean GetHandshakeFlag()
        {
            return ConnectionFlags[0];
        }

        public Boolean GetAckFlag()
        {
            return ConnectionFlags[1];
        }

        public Boolean GetOnFlag()
        {
            return ConnectionFlags[2];
        }

        public Boolean GetFinFlag()
        {
            return ConnectionFlags[3];
        }

        public Boolean GetOffFlag()
        {
            return ConnectionFlags[4];
        }

        public void RemoveConnectionFlag(int flagNum)
        {
            ConnectionFlags.Set(flagNum, false);
            FlushConnectionFlags();
        }

        public static X509Certificate2 GetPublicCertFromReceiver(string usr)
        {
            UserDatabase db = new UserDatabase(@"C:\Users\Majkic\source\repos\Cryptography\CRY\Users.db");
            IEnumerable<User> users = db.GetAllUsers();

            User receiver = null;
            foreach (var user in users)
            {
                if (user.Username == usr)
                {
                    receiver = user;
                    break;
                }
            }

            return new X509Certificate2(receiver.PublicCertificate);
        }

        private RSAParameters GetPublicKeyFromReceiver(string usr)
        {
            UserDatabase db = new UserDatabase(@"C:\Users\Majkic\source\repos\Cryptography\CRY\Users.db");
            IEnumerable<User> users = db.GetAllUsers();
            User receiver = null;
            foreach (var user in users)
            {
                if (user.Username == usr)
                {
                    receiver = user;
                    break;
                }
            }

            var userCert = new X509Certificate2(receiver.PublicCertificate);

            if (CertificateValidator.VerifyCertificate(userCert) == false)
            {
                throw new Exception("Certificate is invalid.\nchat.bin file can't be created.");
            }

            RSACryptoServiceProvider publicKeyProvider = (RSACryptoServiceProvider)userCert.PublicKey.Key;

            return publicKeyProvider.ExportParameters(false);
        }

        public void ParseConnectionBits()
        {
            using (BinaryReader reader = new BinaryReader(new FileStream(chatFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                reader.BaseStream.Seek(nameOffset, SeekOrigin.Begin);
                reader.Read(ConnectionFlagsArray, 0, ConnectionFlagsArray.Length);
            }
            ConnectionFlags = new BitArray(ConnectionFlagsArray);
        }

        public void ParseUsersNames(byte[] file)
        {
            byte[] usr1 = new byte[20];
            byte[] usr2 = new byte[20];
            Array.Copy(file, 0, usr1, 0, 20);
            Array.Copy(file, 20, usr2, 0, 20);
            User1 = Encoding.ASCII.GetString(usr1);
            User2 = Encoding.ASCII.GetString(usr2);
            User1 = User1.Trim('\0');
            User2 = User2.Trim('\0');
        }

        public void ParseChatFile(RSAParameters privKey)
        {
            byte[] file = File.ReadAllBytes(chatFilePath);
            chatFile = new byte[20 + 20 + 1 + 3 + 256 + 256 + 3]; // max.size for name is 20
            Array.Copy(file, 0, chatFile, 0, file.Length);

            ParseConnectionBits();

            if (String.IsNullOrEmpty(User1) && String.IsNullOrEmpty(User2))
            {
                ParseUsersNames(file);
            }

            if (Key == null && IVorSalt == null) // not really necessary if, it's always true
            {
                int offset = nameOffset + 1;
                byte[] encAlg = new byte[3];
                byte[] hashAlg = new byte[3];
                Array.Copy(file, offset, encAlg, 0, 3);
                EncAlgo = Encoding.ASCII.GetString(encAlg);
                offset += 3;

                if (!Helper.IsCryptoAlgoCodeValid(EncAlgo))
                {
                    throw new CryptedMessageParser.Exceptions.UnknownCryptorCodeException(EncAlgo);
                }

                int keyLen = 0, ivLen = 0;
                switch (EncAlgo)
                {
                    case "aes":
                        keyLen = 32;
                        ivLen = 16;
                        break;
                    case "3ds":
                        keyLen = 24;
                        ivLen = 8;
                        break;
                    case "2fh":
                        keyLen = 32;
                        ivLen = 16;
                        break;
                }

                var cryptor = new RsaAlgo(privKey);
                byte[] encKey = new byte[256];
                byte[] encIV = new byte[256];

                Key = new byte[keyLen];
                IVorSalt = new byte[ivLen];

                Array.Copy(file, offset, encKey, 0, 256);
                offset += 256;

                Array.Copy(file, offset, encIV, 0, 256);
                offset += 256;

                Array.Copy(cryptor.Decrypt(encKey), 0, Key, 0, keyLen);
                Array.Copy(cryptor.Decrypt(encIV), 0, IVorSalt, 0, ivLen);

                Array.Copy(file, offset, hashAlg, 0, 3);
                HashAlgo = Encoding.ASCII.GetString(hashAlg);

                if (!Helper.IsHasherCodeValid(HashAlgo))
                {
                    throw new CryptedMessageParser.Exceptions.UnknownHasherCodeException(HashAlgo);
                }
            }

            if (EncAlgo == "aes")
            {
                CryptoTypes = new ChatCryptCombo(Helper.GetHasherFromCode(HashAlgo), new AesAlgo(Key, IVorSalt));
            }
            else if (EncAlgo == "3ds")
            {
                CryptoTypes = new ChatCryptCombo(Helper.GetHasherFromCode(HashAlgo), new TripleDesAlgo(Key, IVorSalt));
            }
            else/* if (EncAlgo == "2fh")*/
            {
                CryptoTypes = new ChatCryptCombo(Helper.GetHasherFromCode(HashAlgo), new TwofishAlgo(Key, IVorSalt));
            }
        }

        public void WriteChatFile()
        {
            byte[] usr1 = Encoding.ASCII.GetBytes(User1);
            byte[] usr2 = Encoding.ASCII.GetBytes(User2);
            chatFile = new byte[20 + 20 + 1 + 3 + 256 + 256 + 3]; // max.size for name is 20

            Array.Copy(usr1, 0, chatFile, 0, usr1.Length);
            Array.Copy(usr2, 0, chatFile, 20, usr2.Length);

            BitsToByte();
            Array.Copy(ConnectionFlagsArray, 0, chatFile, nameOffset, ConnectionFlagsArray.Length);

            int offset = nameOffset + 1;
            byte[] encAlg = Encoding.ASCII.GetBytes(EncAlgo);
            byte[] hashAlg = Encoding.ASCII.GetBytes(HashAlgo);

            var cryptor = new RsaAlgo(GetPublicKeyFromReceiver(User2));
            byte[] encKey = cryptor.Encrypt(Key);
            byte[] encIV = cryptor.Encrypt(IVorSalt);

            Array.Copy(encAlg, 0, chatFile, offset, encAlg.Length); // encAlg.Length = 3 -> e.q. aes, 3ds, 2fh
            offset += encAlg.Length;
            Array.Copy(encKey, 0, chatFile, offset, encKey.Length); // 256B = 2048b len
            offset += encKey.Length;
            Array.Copy(encIV, 0, chatFile, offset, encIV.Length);
            offset += encIV.Length;
            Array.Copy(hashAlg, 0, chatFile, offset, hashAlg.Length); // hashAlg.Length = 3 -> e.q. sh1, sh2, md5

            using FileStream stream = new FileStream(chatFilePath, FileMode.Create);
            using BinaryWriter writter = new BinaryWriter(stream);
            writter.Write(chatFile);
        }
    }
}