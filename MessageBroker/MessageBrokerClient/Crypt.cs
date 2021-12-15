using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MessageBrokerClient
{
    public static class Crypt
    {
        public const int KEYSIZE = 256;
        public const int BLOCKSIZE = 128;
        public const int BLOCKLENGHT = BLOCKSIZE / 8;

        public const PaddingMode PADDING_MODE = PaddingMode.Zeros;
        public const CipherMode CYPHER_MODE = CipherMode.CBC;

        public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = KEYSIZE;
                aes.BlockSize = BLOCKSIZE;
                aes.Padding = PADDING_MODE;
                aes.Mode = CYPHER_MODE;

                aes.Key = key;
                aes.IV = iv;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    //return performCryptography(data, encryptor);
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
                aes.Padding = PADDING_MODE;
                aes.Mode = CYPHER_MODE;

                aes.Key = key;
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    //return performCryptography(data, decryptor);
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
