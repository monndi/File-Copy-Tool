using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace File_Copy_Tool
{
    public sealed class FileCopyToolUtils
    {
        private FileCopyToolUtils() { }

        /// <summary>
        /// Compute the MD5 hash of the input byte array.
        /// </summary>
        /// <param name="data">An array of bytes to be hashed.</param>
        /// <returns>Return the hashed value as a hexadecimal string. </returns>
        public static string GetMD5Hash(byte[] data)
        {
            byte[] hashedData = new MD5CryptoServiceProvider().ComputeHash(data);
            StringBuilder sOut = new StringBuilder(hashedData.Length);

            return BitConverter.ToString(hashedData).Replace("-", "").ToLowerInvariant();
        }
        /// <summary>
        /// Compute the SHA1 hash of the input stream of bytes.
        /// </summary>
        /// <param name="dataStream">A stram of bytes to be hashed.</param>
        /// <returns>Return the hashed value as a hexadecimal string. </returns>
        public static string GetSHA1Hash(FileStream dataStream)
        {
            byte[] hashedData = new SHA1Managed().ComputeHash(dataStream);

            return BitConverter.ToString(hashedData).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// Compute the SHA1 hash of the input byte array.
        /// </summary>
        /// <param name="data">An array of bytes to be hashed.</param>
        /// <returns>Return the hashed value as a hexadecimal string. </returns>
        public static string GetSHA1Hash(byte[] data)
        {
            byte[] hashedData = new SHA1Managed().ComputeHash(data);
            return BitConverter.ToString(hashedData).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// Returns a value indicating if the MD5 hashed input bytes array are equal.
        /// </summary>
        /// <param name="sourceData">First byte array to be hashed and compared.</param>
        /// <param name="destinationData">Second byte array to be hashed and compared.</param>
        /// <returns>true if the hashed input arrays are equal,  otherwise, false.</returns>
        public static bool CompareMD5Hash(byte[] sourceData, byte[] destinationData)
        {
            string sourcheHash = GetMD5Hash(sourceData);
            string destinationHash = GetMD5Hash(destinationData);

            return sourcheHash.CompareTo(destinationHash) == 0;
        }
    }
}
