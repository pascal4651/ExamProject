// <copyright file="HashHelper.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.Utils
{
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Helper for hashing
    /// </summary>
    public class HashHelper
    {
        /// <summary>
        /// Calculate MD5 hash from text
        /// </summary>
        /// <param name="input">Text to hash</param>
        /// <returns>MD5 hash string</returns>
        public static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }
}