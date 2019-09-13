using ClusterWebApi;
using Microsoft.AspNetCore.SignalR;
using Monosoft.Common.DTO;
using Monosoft.Common.MessageQueue;
using System;
using System.Collections.Generic;

namespace Monosoft.Web.ClusterApi.Hubs
{
    public class clusterApiHub : Hub
    {
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

        public byte[] RPC(
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
            return RequestClient.Instance.Rpc(route, System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(mw)));
        }

        public static void HandleMessage(string[] topicparts, EventDTO data)
        {
            var context = Startup.signalRHub;
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
            }
            else
            {
                context.Clients.All.SendAsync(
                    string.Join('.', topicparts[0], topicparts[1]),
                    json
                ).Wait();
            }
        }
    }

}