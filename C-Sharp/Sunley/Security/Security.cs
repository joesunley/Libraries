using Sunley.Miscellaneous;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Sunley.Security
{
    public class CredentialStore
    {
        private Dictionary<string, UserCredentials> store = new Dictionary<string, UserCredentials>();

        public CredentialStore AddCredential(UserCredentials c)
        {
            store.Add(c.GetUsername(), c);
            return this;
        }
        public bool CheckCredentials(LoginAttempt login)
        {
            try
            {
                UserCredentials c = store[login.Username];

                return c.CheckCredentials(login);
            } catch { return false; }

            
        }
    }

    public struct UserCredentials
    {

        // Fields //
        private string username;
        private string passwordHash;
        private string salt;

        
        // Methods //
        public bool CheckCredentials(LoginAttempt login)
        {
            string hash = Encryption.Encrypt(login.Password, salt, HashType.SHA256);
            return login.Username == username && passwordHash == hash;
        }

        public bool CreateCredentials(string uName, string pWord)
        {
            if (username == string.Empty && passwordHash == string.Empty && salt == string.Empty)
                if (ValidateUsername(uName) && ValidatePassword(pWord))
                {
                    string[] outs = Encryption.Encrypt(pWord, HashType.SHA256);

                    username = uName;
                    passwordHash = outs[0];
                    salt = outs[1];

                    return true;
                }
            return false;
        }
        public bool ChangeCredentials(string uName, string pWord)
        {
            if (ValidateUsername(uName) && ValidatePassword(pWord))
            {
                string[] outs = Encryption.Encrypt(pWord, HashType.SHA256);

                username = uName;
                passwordHash = outs[0];
                salt = outs[1];

                return true;
            } else 
                return false; 
        }

        public bool ChangeUsername(string e)
        {
            if (ValidateUsername(e))
            {
                username = e;
                return true;
            }
            return false;
        }
        public bool ChangePassword(string h)
        {
            if (ValidatePassword(h))
            {
                passwordHash = h;
                return true;
            }
            return false;
        }

        private bool ValidateUsername(string input)
        {
            throw new NotImplementedException();
        }
        private bool ValidatePassword(string input)
        {
            throw new NotImplementedException();
        }

        public string GetSalt() { return salt; }
        public string GetUsername() { return username; }
    }

    public struct LoginAttempt
    {

        // Consts //
        private const HashType hashType = HashType.SHA256;


        // Properties //
        public DateTime Time { get; }
        public IPAddress IPAddress { get; }

        public string Username { get; }
        public string Password { get; }


        // Constructors //
        public LoginAttempt(DateTime time, IPAddress iP, string uName, string pWord)
        {
            Time = time;
            IPAddress = iP;
            Username = uName;
            Password = pWord;
        }
        public LoginAttempt NewAttempt(string uName, string pWord)
        {
            DateTime t = DateTime.Now;
            IPAddress ip = Misc.GetPublicIPAddress();
            
            this = new LoginAttempt(t, ip, uName, pWord);
            return this;
        }
    }

    public static class Encryption
    {
        public static string Hash(string input, HashType type)
        {
            switch (type)
            {
                case HashType.SHA256:
                    return Algorithms.Sha256(input);
                case HashType.SHA1:
                    return Algorithms.Sha1(input);
                case HashType.MD5:
                    return Algorithms.Md5(input);
                case HashType.RIPEMD:
                    return Algorithms.RipeMd(input);
                default:
                    throw new Exception("Invalid Hash Type");
            }
            
            throw new NotImplementedException();
        }

        public static string[] Encrypt(string input, HashType type)
        {
            string salt = CreateSalt(256);
            string toHash = input + salt;
            string hash = Hash(toHash, type);
            return new string[] { hash, salt };
        }
        public static string Encrypt(string input, string salt, HashType type)
        {
            return Hash(input + salt, type);
        }

        private static string CreateSalt(int size)
        {
            //Generate a cryptographic random number.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number.
            return Convert.ToBase64String(buff);
        }

        private static class Algorithms
        {
            public static string Sha256(string rawData)
            {

                // Create a SHA256   
                using (SHA256 hash = SHA256.Create())
                {
                    // ComputeHash - returns byte array  
                    byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                    // Convert byte array to a string   
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        builder.Append(bytes[i].ToString("x2"));
                    }
                    return builder.ToString();
                }
            }
            public static string Sha1(string rawData)
            {

                // Create a SHA256   
                using (SHA1 hash = SHA1.Create())
                {
                    // ComputeHash - returns byte array  
                    byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                    // Convert byte array to a string   
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        builder.Append(bytes[i].ToString("x2"));
                    }
                    return builder.ToString();
                }
            }
            public static string Md5(string rawData)
            {

                // Create a SHA256   
                using (MD5 hash = MD5.Create())
                {
                    // ComputeHash - returns byte array  
                    byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                    // Convert byte array to a string   
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        builder.Append(bytes[i].ToString("x2"));
                    }
                    return builder.ToString();
                }
            }
            public static string RipeMd(string rawData)
            {
                throw new NotImplementedException();
                // Create a SHA256   
                //using (RIPEMD160 hash = RIPEMD160.Create())
                //{
                //    // ComputeHash - returns byte array  
                //    byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                //    // Convert byte array to a string   
                //    StringBuilder builder = new StringBuilder();
                //    for (int i = 0; i < bytes.Length; i++)
                //    {
                //        builder.Append(bytes[i].ToString("x2"));
                //    }
                //    return builder.ToString();
                //}
            }
        }


    }

    public enum HashType
    {
        SHA256,
        SHA1,
        MD5,
        RIPEMD
    }

    public struct LoginDetails
    {
        [DefaultValue(false)]
        public bool DateOfBirth { get; set; }

        [DefaultValue(false)]
        public bool PhoneNumber { get; set; }

        [DefaultValue(true)]
        public bool Username { get; set; }

        [DefaultValue(false)]
        public bool Email { get; set; }
    }


}
