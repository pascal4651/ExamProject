using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Monosoft.Common.MessageQueue;
using Monosoft.Common.Utils;
using Monosoft.Web.AuthApi.Hubs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

using System.Reflection;

namespace AuthWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
                Console.WriteLine(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + ": Connected");
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "ms.events", type: "topic");
                    channel.QueueDeclare(queue: "ms.queue.authapi"  , durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueBind("ms.queue.authapi", "ms.events", "#");
                    channel.BasicQos(0, 1, false);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        try
                        {
                            MessageFlow.HandleEvent("authapi", ea, authApiHub.HandleMessage);
                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                        catch (Exception ex)
                        {
                            System.Threading.Thread.Sleep(1000); //try again in a second...
                            channel.BasicNack(ea.DeliveryTag, false, true);
                            authApiHub.Log($"Event exception: {Monosoft.Common.Utils.ExceptionHelper.GetExceptionAsReportText(ex)}");
                        }
                    };

                    channel.BasicConsume("ms.queue.authapi", false, consumer);
                    CreateWebHostBuilder(args).Build().Run();
                }
            }
        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) => WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
    }
}
