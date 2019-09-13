// <copyright file="MessageQueueConfiguration.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.MessageQueue
{
    /// <summary>
    /// An abstract class for messagequeueconfiguration - please us a concrete class such as EventConfiguration or RequestConfiguration
    /// </summary>
    public abstract class MessageQueueConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageQueueConfiguration"/> class.
        /// </summary>
        /// <param name="exchangeName">Name of the exchange from where messages are received</param>
        /// <param name="queueName">Name of the queue onto where messages are to be placed</param>
        /// <param name="routingKey">Define the route that describes the messages to be placed in the queue</param>
        protected MessageQueueConfiguration(string exchangeName, string queueName, string routingKey)
        {
            this.ExchangeName = exchangeName;
            this.QueueName = queueName;
            this.RoutingKey = routingKey;
        }

        /// <summary>
        /// Gets or sets ExchangeName
        /// </summary>
        public string ExchangeName { get; set; }

        /// <summary>
        /// Gets or sets QueueName
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// Gets or sets RoutingKey
        /// </summary>
        public string RoutingKey { get; set; }
    }
}
