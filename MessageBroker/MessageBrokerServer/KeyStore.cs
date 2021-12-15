using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MessageBrokerServer
{
    public static class KeyStore
    {
        public const int KEYSIZE = 256;
        public const int BLOCKSIZE = 128;

        public static string KeysDirectory { get; set; }
        public static void Init()
        {
            KeyStore.KeysDirectory = Path.Combine(Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName), "keys");
            if (!Directory.Exists(KeyStore.KeysDirectory))
                Directory.CreateDirectory(KeyStore.KeysDirectory);
            KeyStore.GetEncryptionKey("broker");

            Console.WriteLine("Key store intialized: " + KeysDirectory);
        }


        private static Dictionary<string, byte[]> keys = new Dictionary<string, byte[]>();

        public static byte[] GetEncryptionKey(string id)
        {
            if (keys.ContainsKey(id))
                return keys[id];

            string keyFile = Path.Combine(KeysDirectory, id + ".key");
            if (File.Exists(keyFile))
            {
                byte[] key = Convert.FromBase64String(File.ReadAllText(keyFile));
                keys.Add(id, key);
                return key;
            }
            else
            {
                using (Aes aes = Aes.Create())
                {
                    aes.KeySize = KEYSIZE;
                    aes.BlockSize = BLOCKSIZE;
                    aes.Padding = PaddingMode.Zeros;
                    aes.GenerateKey();
                    byte[] key = aes.Key;
                    File.WriteAllText(keyFile, Convert.ToBase64String(key));
                    keys.Add(id, key);
                    return key;
                }
            }
        }

        public static byte[] GenerateIV()
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = KEYSIZE;
                aes.BlockSize = BLOCKSIZE;
                aes.Padding = PaddingMode.Zeros;
                aes.GenerateIV();
                return aes.IV;
            }
        }


        public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = KEYSIZE;
                aes.BlockSize = BLOCKSIZE;
                aes.Padding = PaddingMode.Zeros;

                aes.Key = key;
                aes.IV = iv;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    return performCryptography(data, encryptor);
                }
            }
        }

        public static byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = KEYSIZE;
                aes.BlockSize = BLOCKSIZE;
                aes.Padding = PaddingMode.Zeros;

                aes.Key = key;
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    return performCryptography(data, decryptor);
                }
            }
        }

        private static byte[] performCryptography(byte[] data, ICryptoTransform cryptoTransform)
        {
            using (var ms = new MemoryStream())
            using (var cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();

                return ms.ToArray();
            }
        }

    }
}
