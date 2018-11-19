using System;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace InitSetup {
    class InitSetup {
        static string KEY = "qYZLZzmBaeTN6+7MxQhZMIQqTZHWjO60574O13QSYu0=";
        static string IV = "7zQ7iRVOR7dy9NKW28e/rA==";
        static string Path = Environment.CurrentDirectory + "\\Output.txt";

        static void Main(string[] args) {
            if (args.Length == 0) {
                while (true) {
                    string start = Console.ReadLine();
                    using (AesManaged myAes = new AesManaged())

                        if (start == "save") {
                            Console.WriteLine("Username:");
                            string UserName = Console.ReadLine();
                            Console.WriteLine("Password:");
                            string Password = enterPassword();
                            string Info = UserName + "," + Password;
                            if ((UserName == "" && Password == "") || (UserName == "" || Password == "")) {
                                Console.WriteLine("Username or Passward were blank");
                                break;
                            }
                            Encrypt enc = Encrypt(Info, myAes.Key, myAes.IV);
                            StreamWriter SW = new StreamWriter(Path);
                            SW.WriteLine(enc.GetEncStr());
                            SW.WriteLine("-");
                            SW.WriteLine("-");
                            SW.WriteLine(enc.GetKeyStr());
                            SW.WriteLine("-");
                            SW.WriteLine("-");
                            SW.WriteLine(enc.GetIVStr());
                            SW.Close();
                        } else if (start == "load") {
                            if (File.Exists(Path)) {
                                StreamReader SR = new StreamReader(Path);
                                List<string> lines = new List<string>();
                                while (!SR.EndOfStream) {
                                    lines.Add(SR.ReadLine());
                                }
                                Encrypt enc = new Encrypt();
                                enc.SetEncStr(lines[0]);
                                enc.SetKeyStr(lines[3]);
                                enc.SetIVStr(lines[6]);
                                string Out = Decrypt(enc);
                                string[] info = Out.Split(',');
                                Console.WriteLine(info[0]);
                                Console.WriteLine(info[1]);
                            } else {
                                Console.WriteLine("File does not exist.");
                                break;
                            }
                        } else if (start == "run") {
                            run();
                        } else if (start == "quit") {
                            break;
                        }
                }
            } else {
                if (args[0] == "-run") {
                    run();
                }
            }
            Console.ReadLine();
        }

        private static void run() {
            StreamReader SR = new StreamReader(Path);
            List<string> lines = new List<string>();
            while (!SR.EndOfStream) {
                lines.Add(SR.ReadLine());
            }
            Encrypt enc = new Encrypt();
            enc.SetEncStr(lines[0]);
            enc.SetKeyStr(KEY);
            enc.SetIVStr(IV);
            string Out = Decrypt(enc);
            string[] info = Out.Split(',');
            Console.WriteLine(info[0]);
            Console.WriteLine(info[1]);
            Console.ReadLine();
        }

        private static string Decrypt(Encrypt enc) {
            string str = "";


            using (AesManaged aesAlg = new AesManaged()) {
                aesAlg.Key = enc.GetKey();
                aesAlg.IV = enc.GetIV();

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(enc.GetEnc())) {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt)) {
                            str = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }
            return str;
        }

        private static string enterPassword() {
            string pass = "";
            do {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter) {
                    pass += key.KeyChar;
                    Console.Write("*");
                } else {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0) {
                        pass = pass.Substring(0, (pass.Length - 1));
                        Console.Write("\b \b");
                    } else if (key.Key == ConsoleKey.Enter) {
                        break;
                    }
                }
            } while (true);
            return pass;
        }

        private static Encrypt Encrypt(string bytes, byte[] Key, byte[] IV) {
            byte[] encrypted;

            using (AesManaged aesAlg = new AesManaged()) {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream()) {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt)) {
                            swEncrypt.Write(bytes);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return new Encrypt(encrypted, Key, IV); ;
        }
    }

    internal class Encrypt {
        byte[] encrypted { get; set; }
        byte[] key { get; set; }
        byte[] IV { get; set; }

        internal Encrypt() { }
        internal Encrypt(byte[] encryptedIn, byte[] keyIn, byte[] IVIn) {
            encrypted = encryptedIn;
            key = keyIn;
            IV = IVIn;
        }

        internal string GetEncStr() {
            return Convert.ToBase64String(encrypted);
        }
        internal string GetKeyStr() {
            return Convert.ToBase64String(key);
        }
        internal string GetIVStr() {
            return Convert.ToBase64String(IV);
        }

        internal void SetEncStr(string str) {
            encrypted = Convert.FromBase64String(str);
        }

        internal void SetKeyStr(string str) {
            key = Convert.FromBase64String(str);
        }

        internal void SetIVStr(string str) {
            IV = Convert.FromBase64String(str);
        }

        internal byte[] GetKey() {
            return key;
        }

        internal byte[] GetIV() {
            return IV;
        }

        internal byte[] GetEnc() {
            return encrypted;
        }
    }
}
