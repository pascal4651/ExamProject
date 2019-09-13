// <copyright file="EventConfiguration.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.MessageQueue
{
    /// <summary>
    /// Configuration for event messages
    /// </summary>
    public class EventConfiguration : MessageQueueConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventConfiguration"/> class.
        /// </summary>
        /// <param name="queueName">Name of the queue that the messages are to be placed in</param>
        /// <param name="routingKey">Name of the routing which messages are to be placed on the queue</param>
        /// <param name="handler">The message handler</param>
        public EventConfiguration(string queueName, string routingKey, MessageFlow.EventHandler handler)
            : base("ms.events", queueName, routingKey)
        {
            this.Handler = handler;
        }

        /// <summary>
        /// Gets or sets the handler, for handling events
        /// </summary>
        public MessageFlow.EventHandler Handler { get; set; }
    }
}
