// <copyright file="RequestConfiguration.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.MessageQueue
{
    /// <summary>
    /// Settings for handling a specific route on the messagequeue comming in via the ms.request exchange
    /// </summary>
    public class RequestConfiguration : MessageQueueConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestConfiguration"/> class.
        /// </summary>
        /// <param name="queueName">Name of the queue onto where messages are to be placed</param>
        /// <param name="routingKey">Defines the routing of the messages that are to be placed in the queue (ex. user.#)</param>
        /// <param name="handler">A method for handling the messages / the business logic</param>
        public RequestConfiguration(string queueName, string routingKey, MessageFlow.MessageHandler handler)
            : base("ms.request", queueName, routingKey)
        {
            this.Handler = handler;
        }

        /// <summary>
        /// Gets or sets the method for handling the messages
        /// </summary>
        public MessageFlow.MessageHandler Handler { get; set; }
    }
}
