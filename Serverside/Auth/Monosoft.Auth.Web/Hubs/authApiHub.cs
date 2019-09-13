using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Monosoft.Common.DTO;
using Monosoft.Common.MessageQueue;
using System;
using System.Collections.Generic;

namespace Monosoft.Web.AuthApi.Hubs
{
    public class authApiHub : Hub
    {
        public static List<string> memoryLog = new List<string>();

        public static void Log(string str)
        {
            memoryLog.Add(DateTime.Now.ToLongTimeString() + " : " + str);
        }

        public void WriteMessage(
            string scope,
            string route,
            string messageid,
            string json,
            Guid organisationId,
            Guid userContextToken,
            Tracing.Level tracing = Tracing.Level.None)
        {
            string ip = this.Context.GetHttpContext().Connection.RemoteIpAddress.ToString();
            string clientid = this.Context.ConnectionId;

            Log("got data from clientid: " + clientid);

            MessageWrapper mw = new Monosoft.Common.DTO.MessageWrapper(
                        DateTime.Now,
                        userContextToken,
                        scope,
                        clientid,
                        messageid,
                        ip,
                        json,
                        organisationId,
                        new Tracing() { Tracelevel = tracing });
            RequestClient.Instance.FAF(route, System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(mw)));
        }


        public string RPC(
            string scope,
            string route,
            string messageid,
            string json,
            Guid organisationId,
            Guid userContextToken,
            Tracing.Level tracing = Tracing.Level.None)
        {
            string ip = this.Context.GetHttpContext().Connection.RemoteIpAddress.ToString();
            string clientid = this.Context.ConnectionId;
            MessageWrapper mw = new Monosoft.Common.DTO.MessageWrapper(
                        DateTime.Now,
                        userContextToken,
                        scope,
                        clientid,
                        messageid,
                        ip,
                        json,
                        organisationId,
                        new Tracing() { Tracelevel = tracing });
            var result = RequestClient.Instance.Rpc(route, System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(mw)));
            var jsonresult = System.Text.Encoding.UTF8.GetString(result);
            var resultobj = Newtonsoft.Json.JsonConvert.DeserializeObject<ReturnMessageWrapper>(jsonresult);
            return System.Text.Encoding.UTF8.GetString(resultobj.Data);
        }



        public static void HandleMessage(string[] topicparts, EventDTO data)
        {
            var context = AuthWebApi.Startup.signalRHub;

            var area = topicparts[0];
            var operation = topicparts[1];
            var json = data.Json();
            if (json.Length > 32000)
            {
                //return stream
                //TODO:: hvis result er >32KB skal der istedet blot gives en url til data, med en TTL
            }

            

            if (string.IsNullOrEmpty(data.ClientId)==false)
            {
                context.Clients.Client(data.ClientId).SendAsync(
                    string.Join('.', topicparts[0], topicparts[1]),
                    json
                ).Wait();

                Log($"tried to send {string.Join('.', topicparts)} to clientid: {data.ClientId}");
            }
            else
            {
                Log($"tried to send {string.Join('.', topicparts)} to everyone");
                context.Clients.All.SendAsync(
                    string.Join('.', topicparts[0], topicparts[1]),
                    json
                ).Wait();
            }
        }
    }

}