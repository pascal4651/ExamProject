// <copyright file="RequestServer.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.MessageQueue
{
    using System;
    using System.Collections.Generic;
    using Monosoft.Common.Utils;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    /// <summary>
    /// The request server is used to consume messages that are issued to the request exchange
    /// </summary>
    public class RequestServer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestServer"/> class.
        /// Starts the request server, so that it will consume messages from the request (ms.request) exchange
        /// </summary>
        /// <param name="settings">List of topic/routes that are consumed by this server</param>
        public RequestServer(List<MessageQueueConfiguration> settings, double? automaticShutdownSeconds = null)
        {
            DateTime shutdownTime = automaticShutdownSeconds.HasValue ? DateTime.Now.AddSeconds(automaticShutdownSeconds.Value) : DateTime.Now.AddYears(99);

            settings.Add(new RequestConfiguration("diagnostics", "diagnostics.seteventsettings", null));

            Console.WriteLine(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + ": Connecting to: " + MicroServiceConfig.Rabbit_hostname());
            var factory = new ConnectionFactory()
            {
                HostName = MicroServiceConfig.Rabbit_hostname(),
                UserName = MicroServiceConfig.Rabbit_username(),
                Password = MicroServiceConfig.Rabbit_password()
            };
            factory.AutomaticRecoveryEnabled = true;
            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);

            using (var connection = factory.CreateConnection())
            {
                var channel = connection.CreateModel();
                {
                    channel.BasicQos(0, 1, false);
                    foreach (var setting in settings)
                    {
                        Console.WriteLine(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + ": " + setting.QueueName + " connected");
                        channel.ExchangeDeclare(exchange: setting.ExchangeName, type: "topic");
                        channel.QueueDeclare(queue: "ms.queue." + setting.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                        channel.QueueBind("ms.queue." + setting.QueueName, setting.ExchangeName, setting.RoutingKey);
                        var consumer = new EventingBasicConsumer(channel);

                        consumer.Received += (model, ea) =>
                        {
                            Console.WriteLine(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + ": " + setting.QueueName + " message received");
                            System.Threading.Thread.Sleep(1); // just for a bit, to make sure we dont block everything..
                            byte[] response = null;
                            var body = ea.Body;
                            var props = ea.BasicProperties;
                            var replyProps = channel.CreateBasicProperties();
                            replyProps.CorrelationId = props.CorrelationId;
                            try
                            {
                                if (setting is EventConfiguration)
                                {
                                    MessageFlow.HandleEvent(setting.QueueName, ea, (setting as EventConfiguration).Handler);
                                }

                                if (setting is RequestConfiguration)
                                {
                                    response = MessageFlow.HandleMessage(setting.QueueName, ea, (setting as RequestConfiguration).Handler);
                                }
                            }
                            catch (Exception ex)
                            {
                                string reporttxt = Monosoft.Common.Utils.ExceptionHelper.GetExceptionAsReportText(ex);
                                Common.MessageQueue.Diagnostics.Instance.LogEvent("Catestrophic failure", reporttxt, Common.DTO.Severity.Information, Guid.Empty);
                            }
                            finally
                            {
                                channel.BasicAck(ea.DeliveryTag, false);
                                Console.WriteLine(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + ": ACK");
                                if (props != null && props.ReplyTo != null)
                                {
                                    channel.BasicPublish(
                                        exchange: string.Empty,
                                        routingKey: props.ReplyTo,
                                        basicProperties: replyProps,
                                        body: response);
                                    Console.WriteLine(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + ": RPC responded");
                                }
                            }
                        };

                        channel.BasicConsume("ms.queue." + setting.QueueName, false, consumer);
                    }
                }

                while (DateTime.Now < shutdownTime)
                {
                    System.Threading.Thread.Sleep(2500);
                    Console.Write(".");
                }
            }
        }
    }
}