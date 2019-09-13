// <copyright file="MessageDataHelper.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.Utils
{
    /// <summary>
    /// Class containing methods for helping deserializing and serializing objects and bytearrays.
    /// </summary>
    public class MessageDataHelper
    {
        /// <summary>
        /// Takes the object and returns the bytearray of the json
        /// </summary>
        /// <param name="obj">The object parameter to be converted</param>
        /// <returns>Json as utf8 bytearray</returns>
        public static byte[] ToMessageData(object obj)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            return System.Text.Encoding.UTF8.GetBytes(json);
        }

        /// <summary>
        /// Takes the bytearray and returns the object as a typed object
        /// </summary>
        /// <typeparam name="T">The Type of the returned object</typeparam>
        /// <param name="bytes">The bytearray that will be returned as a typed object</param>
        /// <returns>Object of specified generic type from the bytearray parameter</returns>
        public static T FromMessageData<T>(byte[] bytes)
        {
            var jsonStr = System.Text.Encoding.UTF8.GetString(bytes);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonStr);
        }
    }
}
