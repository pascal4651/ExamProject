using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Monosoft.Common.DTO;
using Monosoft.Common.MessageQueue;
using Monosoft.Common.Utils;
using Monosoft.Web.ClusterApi.Hubs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClusterWebApi
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
                    channel.QueueDeclare(queue: "ms.queue.clusterapi"  , durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueBind("ms.queue.clusterapi", "ms.events", "#");
                    channel.BasicQos(0, 1, false);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        try
                        {
                            MessageFlow.HandleEvent("clusterapi", ea, clusterApiHub.HandleMessage);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("ex: " + ex.Message);
                        }
                        finally
                        {
                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                    };

                    channel.BasicConsume("ms.queue.clusterapi", false, consumer);
                    CreateWebHostBuilder(args).Build().Run();
                }

                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "ms.request", type: "topic");//ms.request? eller ms.auth.request?
                    channel.QueueDeclare(queue: "ms.queue.forwardauthapi", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueBind("ms.queue.forwardauthapi", "ms.request", "token.#");
                    channel.QueueBind("ms.queue.forwardauthapi", "ms.request", "servicestore.#");
                    channel.BasicQos(0, 1, false);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        byte[] response = null;
                        var body = ea.Body;
                        var props = ea.BasicProperties;
                        var replyProps = channel.CreateBasicProperties();
                        replyProps.CorrelationId = props.CorrelationId;
                        try
                        {
                            response = MessageFlow.HandleMessage("clusterapi", ea, Program.FowardMessage /*send via signalR*/);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("ex: " + ex.Message);
                        }
                        finally
                        {
                            channel.BasicAck(ea.DeliveryTag, false);
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
                    channel.BasicConsume("ms.queue.forwardauthapi", false, consumer);
                    CreateWebHostBuilder(args).Build().Run();
                }
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) => WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();

        private static System.Collections.Generic.Dictionary<string, MessageWrapper> sendMessages = new System.Collections.Generic.Dictionary<string, MessageWrapper>();

        private static HubConnection _connection = null;

        private static HubConnection signalRConnection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new HubConnectionBuilder().WithUrl("https://auth.Monosoft.dk/authApiHub").Build();
                    _connection.Closed += async (error) =>
                    {
                        await Task.Delay(new Random().Next(0, 5) * 1000);
                        await signalRConnection.StartAsync();
                    };
                    _connection.On<string>("token.invalidate.user", param => { ForwardResponse("token.invalidate.user", param); });
                    _connection.On<string>("token.invalidate.token", param => { ForwardResponse("token.invalidate.token", param); });
                }

                return _connection;
            }
        }

        private static void ForwardResponse(string route, string param)
        {
            var returnmsg = Newtonsoft.Json.JsonConvert.DeserializeObject<ReturnMessageWrapper>(param);

            if (sendMessages.ContainsKey(returnmsg.ResponseToMessageid))
            {
                var orgmessage = sendMessages[returnmsg.ResponseToMessageid];
                MessageWrapper mw = new Monosoft.Common.DTO.MessageWrapper(
                    DateTime.Now,
                    orgmessage.UserContextToken,
                    orgmessage.Scope,
                    orgmessage.Clientid,
                    orgmessage.Messageid,
                    orgmessage.CallerIp,
                    System.Text.Encoding.UTF8.GetString(returnmsg.Data),
                    orgmessage.OrgContext,
                    orgmessage.Tracing);
                RequestClient.Instance.FAF(route, System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(mw)));
                sendMessages.Remove(returnmsg.ResponseToMessageid);
            }
        }

        public static ReturnMessageWrapper FowardMessage(string[] topicparts, MessageWrapper wrapper)
        {
            var client = new HttpClient();
            apiMessage message = new apiMessage()
            {
                Scope = wrapper.Scope,
                Route = string.Join('.', topicparts),
                Messageid = wrapper.Messageid,
                Json = Encoding.UTF8.GetString(wrapper.MessageData),
                OrganisationId = wrapper.OrgContext,
                UserContextToken = wrapper.UserContextToken,
                Tracing = wrapper.Tracing.Tracelevel
            };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            var task = client.PostAsync("https://auth.monosoft.dk/api/RPC", new StringContent(json, Encoding.UTF8, "application/json"));
            task.Wait();
            var res = task.Result.Content.ReadAsByteArrayAsync();
            res.Wait();
            return Monosoft.Common.Utils.MessageDataHelper.FromMessageData<ReturnMessageWrapper>(res.Result);
        }
    }
}
