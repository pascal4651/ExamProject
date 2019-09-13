// <copyright file="EventClient.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.MessageQueue
{
    using System;
    using Monosoft.Common.Utils;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    /// <summary>
    /// A client for sending events to the message queue
    /// </summary>
    public class EventClient
    {
        private static EventClient instance = null;

        private readonly IConnection connection;

        private readonly IModel channel;

        private readonly EventingBasicConsumer consumer;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventClient"/> class.
        /// </summary>
        public EventClient()
        {
            var factory = new ConnectionFactory() { HostName = MicroServiceConfig.Rabbit_hostname(), UserName = MicroServiceConfig.Rabbit_username(), Password = MicroServiceConfig.Rabbit_password() };
            factory.AutomaticRecoveryEnabled = true;
            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);
            this.connection = factory.CreateConnection();
            this.channel = this.connection.CreateModel();
            this.channel.ExchangeDeclare(exchange: "ms.event", type: "topic");
            this.consumer = new EventingBasicConsumer(this.channel);
        }

        /// <summary>
        /// Gets the singleton instance of the client
        /// </summary>
        public static EventClient Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EventClient();
                }

                return instance;
            }
        }

        /// <summary>
        /// Places an event on the message queue
        /// </summary>
        /// <param name="route">The topic/route of the event (ex. user.created)</param>
        /// <param name="eventdata">The event details</param>
        public void RaiseEvent(string route, Monosoft.Common.DTO.EventDTO eventdata)
        {
            Console.WriteLine("Event: " + route);
            byte[] bsonMessage = System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(eventdata));
            var factory = new ConnectionFactory()
            {
                HostName = MicroServiceConfig.Rabbit_hostname(),
                UserName = MicroServiceConfig.Rabbit_username(),
                Password = MicroServiceConfig.Rabbit_password()
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    channel.ExchangeDeclare(exchange: "ms.events", type: "topic");
                    channel.BasicPublish(
                        exchange: "ms.events",
                        routingKey: route,
                        basicProperties: properties,
                        body: bsonMessage);
                }
            }
        }

        /// <summary>
        /// Close the connection
        /// </summary>
        public void Close()
        {
            this.connection.Close();
        }
    }
}