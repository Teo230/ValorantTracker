using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ValorantTracker.Client.Utilities
{
    public class Crypter
    {

        private static byte[] key = Encoding.UTF8.GetBytes("qwertyuiopqwertyuiopqwertyuiopqw");

        public static string Encrypt(string plainText)
        {
            return Convert.ToBase64String(EncryptStringToBytes_Aes(plainText, key));
        }

        public static string Decrypt(string cipherText)
        {
            if (IsEncrypted(cipherText))
            {
                return DecryptStringFromBytes_Aes(Convert.FromBase64String(cipherText), key);
            }
            else
            {
                return cipherText;
            }
        }

        private static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key)
        {
            byte[] encrypted;
            byte[] IV;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;

                aesAlg.GenerateIV();
                IV = aesAlg.IV;

                aesAlg.Mode = CipherMode.CBC;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption. 
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            byte[] combinedIvCt = new byte[IV.Length + encrypted.Length];
            Array.Copy(IV, 0, combinedIvCt, 0, IV.Length);
            Array.Copy(encrypted, 0, combinedIvCt, IV.Length, encrypted.Length);

            // Return the encrypted bytes from the memory stream. 
            return combinedIvCt;

        }

        private static string DecryptStringFromBytes_Aes(byte[] cipherTextCombined, byte[] Key)
        {

            // Declare the string used to hold 
            // the decrypted text. 
            string plaintext = null;

            if (cipherTextCombined.Length != 0)
            {
                // Create an Aes object 
                // with the specified key and IV. 
                using (Aes aesAlg = Aes.Create())
                {
                    try
                    {
                        aesAlg.Key = Key;

                        byte[] IV = new byte[aesAlg.BlockSize / 8];
                        byte[] cipherText = new byte[cipherTextCombined.Length - IV.Length];

                        Array.Copy(cipherTextCombined, IV, IV.Length);
                        Array.Copy(cipherTextCombined, IV.Length, cipherText, 0, cipherText.Length);

                        aesAlg.IV = IV;

                        aesAlg.Mode = CipherMode.CBC;

                        // Create a decrytor to perform the stream transform.
                        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                        // Create the streams used for decryption. 
                        using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                        {
                            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                                {

                                    // Read the decrypted bytes from the decrypting stream
                                    // and place them in a string.
                                    plaintext = srDecrypt.ReadToEnd();
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //ManageException.SendException(ex);
                    }

                }
            }

            return plaintext;

        }

        private static bool IsEncrypted(string text)
        {
            try
            {
                var retVal = DecryptStringFromBytes_Aes(Convert.FromBase64String(text), key);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
