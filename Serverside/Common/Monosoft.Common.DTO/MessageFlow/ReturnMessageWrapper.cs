// <copyright file="ReturnMessageWrapper.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Monosoft.Common.DTO
{
    /// <summary>
    /// Wrapper for return messages
    /// </summary>
    public class ReturnMessageWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnMessageWrapper"/> class.
        /// </summary>
        /// <param name="responseToClientid">Which client is to receive the message</param>
        /// <param name="responseToMessageid">What is the clients message id</param>
        /// <param name="data">the return object as json (utf8 byte array)</param>
        public ReturnMessageWrapper(string responseToClientid, string responseToMessageid, byte[] data)
        {
            this.ResponseToClientid = responseToClientid;
            this.ResponseToMessageid = responseToMessageid;
            this.Data = data;
        }

        public static ReturnMessageWrapper CreateResult(bool success, Common.DTO.MessageWrapper wrapper, List<LocalizedString> message, object data)
        {
            string dataAsJson = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            byte[] res = System.Text.Encoding.UTF8.GetBytes(dataAsJson);

            Monosoft.Common.DTO.ReturnMessageWrapper resultobj = new ReturnMessageWrapper(wrapper.Clientid, wrapper.Messageid, res);
            resultobj.Message = message.ToArray();
            resultobj.Success = success;
            return resultobj;
        }

        /// <summary>
        /// Gets or sets ResponseToClientid
        /// </summary>
        public string ResponseToClientid { get; set; }

        /// <summary>
        /// Gets or sets ResponseToMessageid
        /// </summary>
        public string ResponseToMessageid { get; set; }

        /// <summary>
        /// Gets or sets Message
        /// </summary>
        public LocalizedString[] Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets Success
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets data (json serialized object, which have been converted to utf8 byte array)
        /// </summary>
        public byte[] Data { get; set; }
    }

    ///// <summary>
    ///// Message wrapper helper, for encoding/decoding data for transport
    ///// </summary>
    ///// <typeparam name="T">Type of the data</typeparam>
    //public class ReturnMessageWrapperHelper<T>
    //{
    //    /// <summary>
    //    /// Get decoded data from the message wrapper
    //    /// </summary>
    //    /// <param name="wrapper">message wrapper</param>
    //    /// <returns>decoded data</returns>
    //    public static T GetData(ReturnMessageWrapper wrapper)
    //    {
    //        T data = default(T);
    //        string json = System.Text.Encoding.UTF8.GetString(wrapper.);
    //        data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);

    //        return data;
    //    }

    //    /// <summary>
    //    /// Sets, and encode data into the message wrapper
    //    /// </summary>
    //    /// <param name="wrapper">message wrapper</param>
    //    /// <param name="data">encoding data</param>
    //    public static void SetData(ReturnMessageWrapper wrapper, T data)
    //    {
    //        string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
    //        byte[] res = System.Text.Encoding.UTF8.GetBytes(json);
    //        wrapper.MessageData = res;
    //    }
    //}
}