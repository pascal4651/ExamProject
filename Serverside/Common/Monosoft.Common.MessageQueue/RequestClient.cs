// <copyright file="RequestClient.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.MessageQueue
{
    using System;
    using System.Collections.Concurrent;
    using Monosoft.Common.Utils;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    /// <summary>
    /// Client for publishing messages to the request exchange
    /// </summary>
    public class RequestClient
    {
        private static RequestClient instance = null;

        private readonly IConnection connection;

        private readonly IModel channel;

        private readonly string replyQueueName;

        private readonly EventingBasicConsumer consumer;

        private readonly BlockingCollection<byte[]> respQueue = new BlockingCollection<byte[]>();

        private readonly IBasicProperties props;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestClient"/> class.
        /// </summary>
        public RequestClient()
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = MicroServiceConfig.Rabbit_hostname(),
                UserName = MicroServiceConfig.Rabbit_username(),
                Password = MicroServiceConfig.Rabbit_password()
            };
            factory.AutomaticRecoveryEnabled = true;
            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);
            this.connection = factory.CreateConnection();
            this.channel = this.connection.CreateModel();
            this.channel.ExchangeDeclare(exchange: "ms.request", type: "topic");
            this.replyQueueName = this.channel.QueueDeclare().QueueName;
            this.consumer = new EventingBasicConsumer(this.channel);
            this.props = this.channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            this.props.CorrelationId = correlationId;
            this.props.ReplyTo = this.replyQueueName;
            this.consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    this.respQueue.Add(body);
                }
            };
        }

        /// <summary>
        /// Gets the singleton instance of the request client
        /// </summary>
        public static RequestClient Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RequestClient();
                }

                return instance;
            }
        }

        /// <summary>
        /// Make a RemoteProcedureCall, which willl only return once a result is returned from the server
        /// </summary>
        /// <param name="routingkey">routing key</param>
        /// <param name="bytes">data as utf8 byte array of the json representation</param>
        /// <returns>resulting object as utf8 bytearray</returns>
        public byte[] Rpc(string routingkey, byte[] bytes)
        {
            this.channel.BasicPublish(
                exchange: "ms.request",
                routingKey: routingkey,
                basicProperties: this.props,
                body: bytes);

            this.channel.BasicConsume(
                consumer: this.consumer,
                queue: this.replyQueueName,
                autoAck: true);

            return this.respQueue.Take();
        }

        /// <summary>
        /// Fire And Forget, which just places a message on the queue (very fast!)
        /// </summary>
        /// <param name="routingkey">routingkey</param>
        /// <param name="data">data</param>
        public void FAF(string routingkey, byte[] data)
        {
            this.channel.BasicPublish(
                exchange: "ms.request",
                routingKey: routingkey,
                basicProperties: null,
                body: data);
        }

        /// <summary>
        /// Close the connection to the messagequeue
        /// </summary>
        public void Close()
        {
            this.connection.Close();
        }
    }
}