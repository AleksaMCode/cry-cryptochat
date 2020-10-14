using System;
using System.IO;
using CRY.UserSQLManager;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using CRY.CryptedMessageParser;
using CRY.AlgoLibrary;
using System.Linq;

namespace CRY
{
    public class Program
    {
        private readonly static string[] commands = File.ReadAllLines("command_list.txt");
        private readonly static string prefix = ">> ";
        private readonly static string mirrorPrefix = " <<";
        private readonly static string wrongCommand = "Wrong command!";
        private static bool startChat = false;
        private static string steganographyImgLocation = @"C:\Users\Majkic\source\repos\Cryptography\CRY\hello_friend.jpg";
        private enum Choice { log, reg, logoff, usr, chat, help, wrong_command };
        private static readonly List<Tuple<string, int>> choices = new List<Tuple<string, int>> // <command name, arg.num.> 
        {
            Tuple.Create("log", 6),
            Tuple.Create("reg", 6),
            Tuple.Create("logoff", 0),
            Tuple.Create("usr", 1),
            Tuple.Create("chat", 6),
            Tuple.Create("help",0)
        };
        private static UserInformation user = null;
        private static UserDatabase dataBase = new UserDatabase(@"C:\Users\Majkic\source\repos\Cryptography\CRY\Users.db");
        private static ChatRoom ChRoom;

        public static void ChatNewMessage(object s, FileSystemEventArgs e)
        {
            Thread.Sleep(3_000);

            string sender;

            if (user.Username == ChRoom.User1)
            {
                sender = ChRoom.User2;
            }
            else
            {
                sender = ChRoom.User1;
            }

            if (ChRoom.GetAckFlag() == true || ChRoom.GetFinFlag() == true)
            {
                byte[] encByteArray = SteganographyAlgo.Decode(@"C:\Users\Majkic\source\repos\Cryptography\CRY\chat\online\" + user.Username + @"\message.png");
                Stream enc = new MemoryStream(encByteArray);
                byte[] decMsg = new byte[] { };

                Decryptor dec = new Decryptor(dataBase.GetUser(sender), user, ChRoom.CryptoTypes);
                dec.DecryptFile(EncryptedMessageParser.Parse(enc), ref decMsg, ChRoom.Key, ChRoom.IVorSalt);

                string msg = Encoding.ASCII.GetString(decMsg);
                Console.WriteLine(String.Format("\n\t\t\t\t{0,50}{1,40}", msg, String.Format("{0} [{1:d} {1:t}]", mirrorPrefix + sender, DateTime.Now)));
                if (ChRoom.GetAckFlag() == true)
                {
                    Console.Write("{0}\t\t", user.Username + prefix);
                }
                else
                {
                    Console.WriteLine("Chat with {0} is over. CRY - chat app is now closing...", sender);
                }

                GC.Collect();

                if (ChRoom.GetFinFlag() == true)
                {
                    Thread.Sleep(3_000);
                    ChRoom.SetOffFlag();
                    ChRoom.RemoveConnectionFlag(3);
                    ChRoom.RemoveConnectionFlag(2);
                    Thread.Sleep(5_000);
                    Directory.Delete(@"C:\Users\Majkic\source\repos\Cryptography\CRY\chat\online\" + user.Username, true);
                    //File.Delete(ChRoom.chatFilePath);
                    Environment.Exit(1);
                }

                ChRoom.RemoveConnectionFlag(1); // Remove Ack flag
            }
            else
            {
                byte[] decMsg = new byte[] { };
                Decryptor dec = new Decryptor(dataBase.GetUser(sender), user, ChRoom.CryptoTypes);
                {
                    MemoryStream enc = new MemoryStream();
                    using FileStream file = new FileStream(@"C:\Users\Majkic\source\repos\Cryptography\CRY\chat\online\" + user.Username + @"\message.msg",
                        FileMode.Open, FileAccess.Read, FileShare.Read);
                    file.CopyTo(enc);
                    enc.Position = 0;

                    dec.DecryptFile(EncryptedMessageParser.Parse(enc), ref decMsg, ChRoom.Key, ChRoom.IVorSalt);
                    file.Close();
                    ;
                }

                string msg = Encoding.ASCII.GetString(decMsg);
                Console.WriteLine(String.Format("\n\t\t\t\t{0,50}{1,40}", msg, String.Format("{0} [{1:d} {1:t}]", mirrorPrefix + sender, DateTime.Now)));
                Console.Write("{0}\t\t", user.Username + prefix);

                GC.Collect();
            }

            if (File.Exists(@"C:\Users\Majkic\source\repos\Cryptography\CRY\chat\online\" + user.Username + @"\message.png"))
            {
                while (true)
                {
                    try
                    {
                        Thread.Sleep(1_000);
                        File.Delete(@"C:\Users\Majkic\source\repos\Cryptography\CRY\chat\online\" + user.Username + @"\message.png");
                        break;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            else
            {
                while (true)
                {
                    try
                    {
                        Thread.Sleep(1_000);
                        File.Delete(@"C:\Users\Majkic\source\repos\Cryptography\CRY\chat\online\" + user.Username + @"\message.msg");
                        break;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        private static bool first = false;
        public static void EncChatRoom()
        {
            if (first == false)
            {
                Console.Clear();
                Task.Run(() =>
                {
                    var watcher1 = new FileSystemWatcher();
                    watcher1.Path = @"C:\Users\Majkic\source\repos\Cryptography\CRY\chat\online\" + user.Username + @"\";
                    watcher1.Created += ChatNewMessage;
                    watcher1.Filter = "message.msg";
                    watcher1.EnableRaisingEvents = true;
                });

                Task.Run(() =>
                {
                    var watcher2 = new FileSystemWatcher();
                    watcher2.Path = @"C:\Users\Majkic\source\repos\Cryptography\CRY\chat\online\" + user.Username + @"\";
                    watcher2.Created += ChatNewMessage;
                    watcher2.Filter = "message.png";
                    watcher2.EnableRaisingEvents = true;
                });

                first = true;
            }

            bool lstMsg = false;
            while (lstMsg == false)
            {
                string message = "";
                Console.Write("{0}\t\t", user.Username + prefix);
                message = Console.ReadLine()/*.Split('$')*/;
                int messageSplitNum = message.Where(x => x == '|').Count();

                if (messageSplitNum == 2) // lastmsg | message | steganographyImgLocation
                {
                    string[] messageArray = message.Split('|');

                    if (messageArray[0].Trim() == "lastmsg")
                    {
                        message = messageArray[1];

                        if (File.Exists(messageArray[2].Trim())) // if file doesn't exist, default location will be used
                        {
                            steganographyImgLocation = messageArray[2].Trim();
                        }

                        lstMsg = true;
                        ChRoom.SetFinFlag();
                    }
                }
                else if (ChRoom.GetAckFlag() == true && messageSplitNum == 1) // default location is only changed if there is other input that is correct
                {
                    string[] messageArray = message.Split('|');
                    message = messageArray[0];

                    if (File.Exists(messageArray[1].Trim())) // if file doesn't exist, default location will be used
                    {
                        steganographyImgLocation = messageArray[1].Trim();
                    }
                }

                message = message.Trim();

                string receiver;

                if (user.Username == ChRoom.User1)
                {
                    receiver = ChRoom.User2;
                }
                else
                {
                    receiver = ChRoom.User1;
                }

                if (File.Exists(@"C:\Users\Majkic\source\repos\Cryptography\CRY\chat\online\" + receiver + @"\message.msg")
                    || File.Exists(@"C:\Users\Majkic\source\repos\Cryptography\CRY\chat\online\" + receiver + @"\message.png")) // used to prevent message congestion!
                {
                    Console.WriteLine("Server congestion -> Message can't be sent. Wait for the timeout to end.");
                    Thread.Sleep(5_000);
                }
                else
                {
                    byte[] msg = Encoding.ASCII.GetBytes(message);
                    MessageFile msgFile = new MessageFile(msg);

                    Encryptor enc = new Encryptor(dataBase.GetUser(receiver), user, ChRoom.CryptoTypes);

                    {
                        Stream encStream = new MemoryStream();
                        enc.EncryptFile(msgFile, encStream, ChRoom.Key, ChRoom.IVorSalt);
                        encStream.Position = 0;

                        if (ChRoom.GetAckFlag() == true || ChRoom.GetFinFlag() == true) // first/last message - enc. + steganography enc.
                        {
                            byte[] encArray;

                            using var memoryStream = new MemoryStream();
                            encStream.CopyTo(memoryStream);
                            encArray = memoryStream.ToArray();

                            SteganographyAlgo.Encode(encArray, steganographyImgLocation,
                                @"C:\Users\Majkic\source\repos\Cryptography\CRY\chat\online\" + receiver + @"\message.png");
                        }
                        else
                        {
                            using FileStream fs = new FileStream(@"C:\Users\Majkic\source\repos\Cryptography\CRY\chat\online\" + receiver +
                                @"\message.msg", FileMode.Create, System.IO.FileAccess.Write);
                            encStream.CopyTo(fs);
                        }
                    }
                }
            }
            Directory.Delete(@"C:\Users\Majkic\source\repos\Cryptography\CRY\chat\online\" + user.Username, true);
            Environment.Exit(1);
        }

        static public void Loading(string loadingMessage)
        {
            Console.Write(loadingMessage);
            using (var progress = new ProgressBar())
            {
                for (int i = 0; i <= 100; i++)
                {
                    progress.Report((double)i / 100);
                    Thread.Sleep(250);
                    if (ChRoom.GetHandshakeFlag() == false && ChRoom.GetOnFlag() == true && ChRoom.GetAckFlag() == true)
                    {
                        break;
                    }
                }
            }
            Console.WriteLine("Done.");
        }

        public static void StartProgram()
        {
            Console.WriteLine(prefix + "Welcome to CRY - crypto chat app.");
            Console.WriteLine("\nPress Enter to continue.");
            Console.ReadLine();

            while (true)
            {
                string command;
                if (startChat)
                {
                    EncChatRoom();
                }

                Console.Clear();
                if (user != null)
                {
                    Console.WriteLine(prefix + "You are logged in as {0}.", user.Username);
                }
                Console.WriteLine(prefix + "Type <help> for more info.");
                Console.Write(prefix);

                string[] tokens = Console.ReadLine().Split(' ');

                command = tokens[0];
                Choice selected = Choice.wrong_command;
                {
                    int i = 0;
                    foreach (var ch in choices)
                    {
                        if (command.Equals(choices[i++].Item1))
                            selected = (Choice)(--i);

                        if (ch.Item1.Equals(command) && tokens.Length - 1 == ch.Item2)
                        {
                            break;
                        }
                        else if (ch.Item1.Equals(command) && tokens.Length - 1 != ch.Item2)
                        {
                            selected = Choice.wrong_command;
                            break;
                        }
                    }
                }
                if (selected == Choice.wrong_command)
                {
                    Console.WriteLine(wrongCommand + "\nPress Enter to continue.");
                    Console.ReadLine();
                    continue;
                }

                if (selected != Choice.wrong_command)
                {
                    switch (selected)
                    {
                        case Choice.log:
                            {
                                if (user == null)
                                {
                                    if (tokens[1] == "-name" && tokens[3] == "-pass" && tokens[5] == "-key")
                                    {
                                        if (File.Exists(tokens[6]))
                                        {
                                            IEnumerable<User> users = dataBase.GetAllUsers();
                                            bool usrExist = false;
                                            foreach (var us in users)
                                            {
                                                if (us.Username == tokens[4])
                                                {
                                                    usrExist = true;
                                                    break;
                                                }
                                            }
                                            if (usrExist)
                                            {
                                                LoginControl login = new LoginControl(tokens[6]);
                                                try
                                                {
                                                    user = login.Login(tokens[2], tokens[4]);
                                                    Directory.CreateDirectory(@"C:\Users\Majkic\source\repos\Cryptography\CRY\chat\online\" + tokens[2]);
                                                    Console.WriteLine("Successful login.");
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.WriteLine(e);
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("User '{0}' doesn't exist.", tokens[6]);
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Key '{0}' doesn't exist.", tokens[6]);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine(wrongCommand);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("You are already logged-in.");
                                }
                                Console.WriteLine("\nPress Enter to continue.");
                                Console.ReadLine();
                                break;
                            }
                        case Choice.reg:
                            {
                                if (tokens[1] == "-name" && tokens[3] == "-pass" && tokens[5] == "-cert")
                                {
                                    if (File.Exists(tokens[6]))
                                    {
                                        RegisterControl register = new RegisterControl(dataBase);
                                        try
                                        {
                                            register.Register(tokens[2], tokens[4], tokens[6]);
                                            Console.WriteLine("New user successfully added.");
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Certificate '{0}' doesn't exist.", tokens[6]);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine(wrongCommand);
                                }
                                Console.WriteLine("\nPress Enter to continue.");
                                Console.ReadLine();
                                break;
                            }
                        case Choice.logoff:
                            {
                                if (user != null)
                                {
                                    Directory.Delete(@"C:\Users\Majkic\source\repos\Cryptography\CRY\chat\online\" + user.Username, true);
                                    user = null;
                                    Console.WriteLine("You have been successfully logged out.");
                                }
                                else
                                {
                                    Console.WriteLine("You aren't logged-in.");
                                }
                                Console.WriteLine("\nPress Enter to continue.");
                                Console.ReadLine();
                                break;
                            }
                        case Choice.usr:
                            {
                                if (tokens[1] == "-a" || tokens[1] == "-o")
                                {
                                    IEnumerable<User> users = dataBase.GetAllUsers();
                                    if (tokens[1].Equals("-a")) // list all the users
                                    {
                                        int i = 1;
                                        foreach (var us in users)
                                        {
                                            Console.WriteLine(i++ + ". " + us.Username);
                                        }
                                    }
                                    if (tokens[1].Equals("-o")) // list only the online users
                                    {
                                        int i = 1;
                                        if (Directory.GetDirectories(@"C:\Users\Majkic\source\repos\Cryptography\CRY\chat\online\").Length > 0)
                                        {
                                            foreach (var us in users)
                                            {
                                                if (Directory.Exists(@"C:\Users\Majkic\source\repos\Cryptography\CRY\chat\online\" + us.Username))
                                                {
                                                    Console.WriteLine(i++ + ". " + us.Username);
                                                }
                                            }
                                        }
                                        if (i == 1)
                                        {
                                            Console.WriteLine("There is no other users online.");
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine(wrongCommand);
                                }
                                Console.WriteLine("\nPress Enter to continue.");
                                Console.ReadLine();
                                break;
                            }
                        case Choice.help:
                            {
                                int i = 1;
                                foreach (var line in commands)
                                {
                                    Console.WriteLine("\t{0,3}: {1}", i++, line);
                                }
                                Console.WriteLine("\nPress Enter to continue.");
                                Console.ReadLine();
                                break;
                            }
                        case Choice.chat:
                            {
                                if (user != null)
                                {
                                    if (tokens[2] != user.Username)
                                    {
                                        bool isUserOnline = false;
                                        IEnumerable<User> users = dataBase.GetAllUsers();
                                        foreach (var us in users)
                                        {
                                            if (us.Username == tokens[2] && Directory.Exists(@"C:\Users\Majkic\source\repos\Cryptography\CRY\chat\online\" + us.Username))
                                            {
                                                isUserOnline = true;
                                                break;
                                            }
                                        }
                                        if (isUserOnline)
                                        {
                                            if (Helper.IsCryptoAlgoCodeValid(tokens[4]))
                                            {
                                                if (Helper.IsHasherCodeValid(tokens[6]))
                                                {
                                                    ChRoom = new ChatRoom(user.Username, tokens[2], tokens[4], tokens[6]);
                                                    ChRoom.SetHandshakeFlag();

                                                    var task = Task.Factory.StartNew(() => Loading(String.Format("Waiting for confirmation from {0}... ", tokens[2])));
                                                    Task.WaitAll(task);

                                                    if (ChRoom.GetHandshakeFlag() == false && ChRoom.GetOnFlag() == true && ChRoom.GetAckFlag() == true)
                                                    {
                                                        Console.WriteLine("Connection with {0} successfully established.", tokens[2]);
                                                        Thread.Sleep(200);
                                                        Console.WriteLine("\nPress Enter to continue.");
                                                        Console.ReadLine();
                                                        Console.Clear();
                                                        EncChatRoom();
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Connection with {0} has successfully failed.", tokens[2]);
                                                        File.Delete(ChRoom.chatFilePath); //delete chat file
                                                    }
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Hash algorithm isn't valid.");
                                                    throw new CryptedMessageParser.Exceptions.UnknownHasherCodeException(tokens[4]);
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("Encryption algorithm isn't valid.");
                                                throw new CryptedMessageParser.Exceptions.UnknownCryptorCodeException(tokens[4]);
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("{0} user is not online.", tokens[2]);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("You can't chat with yourself.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("You need to login before you start the chat.");
                                }
                                Console.WriteLine("\nPress Enter to continue.");
                                Console.ReadLine();
                                break;
                            }
                    }
                }
            }
        }

        public static void ChatHandshake(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(3_000);

            if (user != null)
            {
                ChatRoom cRoom = new ChatRoom();
                if (user.Username != cRoom.User1)
                {
                    Console.WriteLine();
                    string answer = "n";
                    do
                    {
                        Console.WriteLine(prefix + "User {0} wants to chat with you? [y|n]", cRoom.User1);
                        Console.Write(prefix);
                        answer = Console.ReadLine();
                        answer = answer.ToLower();

                        if (!answer.StartsWith("y") && !answer.StartsWith("n"))
                        {
                            Console.WriteLine(prefix + "Wrong answer, try again.");
                        }
                        else
                        {
                            continue;
                        }

                    } while (!answer.StartsWith("y") && !answer.StartsWith("n"));

                    if (answer == "y")
                    {
                        cRoom.ParseChatFile(user.PrivateKey);
                        cRoom.SetAckFlag();
                        cRoom.SetOnFlag();
                        cRoom.RemoveConnectionFlag(0);
                        ChRoom = cRoom;
                        Console.Clear();
                        startChat = true;
                        return;
                    }
                }
            }
        }

        public static void ChatFileChange(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(3_000);
            ChRoom.ParseConnectionBits();
        }

        static void Main(string[] args)
        {
            Task.Run(() =>
            {
                var watcher = new FileSystemWatcher();
                watcher.Path = @"C:\Users\Majkic\source\repos\Cryptography\CRY\chat";
                watcher.Created += ChatHandshake;
                watcher.Changed += ChatFileChange;
                watcher.Filter = "chat.bin";
                watcher.EnableRaisingEvents = true;
            });

            StartProgram();
        }
    }
}