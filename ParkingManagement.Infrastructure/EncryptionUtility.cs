using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ParkingManagement.Infrastructure
{
    public class EncryptionUtility : IEncryptionUtility
    {
        public string ComputeHashWithSalt(string text)
        {
            byte[] passwordBytes = Encoding.Unicode.GetBytes(text);
            byte[] salt = new byte[16];

            RandomNumberGenerator.Create().GetBytes(salt);

            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] hash = CombineBytes(salt, sha512.ComputeHash(CombineBytes(salt, passwordBytes)));

                return Convert.ToBase64String(hash);
            }
        }

        public bool IsHashEqual(string hashWithSalt, string text)
        {
            byte[] passwordBytes = Encoding.Unicode.GetBytes(text);
            byte[] hashedBytes = Convert.FromBase64String(hashWithSalt);
            byte[] salt = ExtractSalt(hashedBytes);

            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] hash = CombineBytes(salt, sha512.ComputeHash(CombineBytes(salt, passwordBytes)));

                return hashedBytes.SequenceEqual(hash);
            }
        }

        private byte[] CombineBytes(byte[] buffer1, byte[] buffer2)
        {
            byte[] dst = new byte[buffer1.Length + buffer2.Length];

            Buffer.BlockCopy(buffer1, 0, dst, 0, buffer1.Length);
            Buffer.BlockCopy(buffer2, 0, dst, buffer1.Length, buffer2.Length);

            return dst;
        }

        private byte[] ExtractSalt(byte[] hash)
        {
            byte[] salt = new byte[16];

            Buffer.BlockCopy(hash, 0, salt, 0, 16);

            return salt;
        }
    }
}
