// <copyright file="MessageWrapper.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.DTO
{
    /// <summary>
    /// Message wrapper helper, for encoding/decoding data for transport
    /// </summary>
    /// <typeparam name="T">Type of the data</typeparam>
    public class MessageWrapperHelper<T>
    {
        /// <summary>
        /// Get decoded data from the message wrapper
        /// </summary>
        /// <param name="wrapper">message wrapper</param>
        /// <returns>decoded data</returns>
        public static T GetData(MessageWrapper wrapper)
        {
            T data = default(T);
            string json = System.Text.Encoding.UTF8.GetString(wrapper.MessageData);
            data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);

            return data;
        }

        /// <summary>
        /// Sets, and encode data into the message wrapper
        /// </summary>
        /// <param name="wrapper">message wrapper</param>
        /// <param name="data">encoding data</param>
        public static void SetData(MessageWrapper wrapper, T data)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            byte[] res = System.Text.Encoding.UTF8.GetBytes(json);
            wrapper.MessageData = res;
        }
    }
}