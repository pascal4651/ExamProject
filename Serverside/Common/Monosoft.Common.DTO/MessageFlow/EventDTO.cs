// <copyright file="EventDTO{T}.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.DTO
{
    /// <summary>
    /// Event DTO definition
    /// </summary>
    public class EventDTO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventDTO"/> class.
        /// </summary>
        public EventDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventDTO"/> class.
        /// </summary>
        /// <param name="json">event data as json</param>
        /// <param name="clientid">caller clientid</param>
        /// <param name="messageid">caller messageid</param>
        public EventDTO(string json, string clientid, string messageid)
        {
            this.Data = System.Text.Encoding.UTF8.GetBytes(json);
            this.ClientId = clientid;
            this.MessageId = messageid;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventDTO"/> class.
        /// </summary>
        /// <param name="obj">event data</param>
        /// <param name="clientid">caller clientid</param>
        /// <param name="messageid">caller messageid</param>
        public EventDTO(object obj, string clientid, string messageid)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            this.Data = System.Text.Encoding.UTF8.GetBytes(json);
            this.ClientId = clientid;
            this.MessageId = messageid;
        }

        /// <summary>
        /// Gets or sets ClientId
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets MessageId
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// Gets or sets Data
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Get event data as json
        /// </summary>
        /// <returns>json string</returns>
        public string Json()
        {
            return System.Text.Encoding.UTF8.GetString(this.Data);
        }
    }
}
