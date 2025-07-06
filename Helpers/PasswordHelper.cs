using System;
using System.Security.Cryptography;
using System.Text;

namespace QuitSmartApp.Helpers
{
    /// <summary>
    /// Password hashing and verification utilities
    /// </summary>
    public static class PasswordHelper
    {
        /// <summary>
        /// Hash a password using SHA256 (basic implementation for demo)
        /// In production, use BCrypt or similar library
        /// </summary>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        /// <summary>
        /// Verify a password against its hash
        /// </summary>
        public static bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
                return false;

            string hashedPassword = HashPassword(password);
            return hashedPassword == hash;
        }
    }
}
