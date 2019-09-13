// <copyright file="MessageFlow.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.MessageQueue
{
    using System;
    using System.Collections.Generic;
    using Monosoft.Common.DTO;
    using RabbitMQ.Client.Events;

    /// <summary>
    /// Class for handling the generic part of the messageflow, unwrapping the wrapper and handling diagnostics
    /// </summary>
    public class MessageFlow
    {
        /// <summary>
        /// A delegate description for handling request messages
        /// </summary>
        /// <param name="topicparts">a list of route parts (ex: [user, create])</param>
        /// <param name="wrapper">The messagewrapper object</param>
        /// <returns>the returning object serialised as json and converted to utf8 byte array</returns>
        public delegate ReturnMessageWrapper MessageHandler(string[] topicparts, MessageWrapper wrapper);

        /// <summary>
        /// A delegate description for handling event messages
        /// </summary>
        /// <param name="topicparts">a list of route parts (ex: [user, create])</param>
        /// <param name="data">The eventdto object</param>
        public delegate void EventHandler(string[] topicparts, EventDTO data);

        /// <summary>
        /// Handles the basic messageflow, including traceing
        /// </summary>
        /// <param name="servicename">name of the calling service (used for traceing)</param>
        /// <param name="ea">the rabbitMQ delivery event</param>
        /// <param name="handler">the message handler for handling specifik logic</param>
        /// <returns>a messagewrapper for with the resulting response</returns>
        public static byte[] HandleMessage(string servicename, BasicDeliverEventArgs ea, MessageHandler handler)
        {
            ReturnMessageWrapper returnObj = null;
            string[] topicparts = ea.RoutingKey.Split(".");
            if (ea.Body.Length == 0)
            {
                return null;
            }

            var wrapper = Newtonsoft.Json.JsonConvert.DeserializeObject<Common.DTO.MessageWrapper>(System.Text.Encoding.UTF8.GetString(ea.Body));

            List<Common.DTO.Trace> tracelist = new List<Common.DTO.Trace>();
            if (wrapper.Tracing != null && wrapper.Tracing.Trace != null)
            {
                tracelist.AddRange(wrapper.Tracing.Trace);
            }

            Common.DTO.Trace trace = null;
            try
            {
                if (wrapper.Tracing != null && wrapper.Tracing.Tracelevel != Common.DTO.Tracing.Level.None)
                {
                    trace = new DTO.Trace(servicename, ea.RoutingKey);
                }

                if (topicparts[0] == "diagnostics")
                {
                    if (topicparts[1] == "seteventsettings")
                    {
                        DTO.DiagnosticsSettings settings = DTO.MessageWrapperHelper<DTO.DiagnosticsSettings>.GetData(wrapper);
                        if (settings.Servicename == servicename)
                        {
                            Diagnostics.Instance.CurrentSettings = settings;
                            Diagnostics.Instance.HeatbeatWorker(servicename, settings);
                        }
                    } else
                    if (topicparts[1] == "geteventsettings")
                    {
                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(Diagnostics.Instance.CurrentSettings);
                        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
                        return System.Text.Encoding.UTF8.GetBytes(
                            Newtonsoft.Json.JsonConvert.SerializeObject(
                                new Monosoft.Common.DTO.ReturnMessageWrapper(
                                    wrapper.Clientid,
                                    wrapper.Messageid,
                                    bytes)));
                    }

                } else
                if (handler != null)
                {
                    returnObj = handler(topicparts, wrapper);
                }
            }
            catch (Exception ex)
            {
                if (wrapper.Tracing != null && wrapper.Tracing.Tracelevel == Common.DTO.Tracing.Level.All)
                {
                    trace.InternalTrace = Common.Utils.ExceptionHelper.GetExceptionAsReportText(ex);
                }
            }
            finally
            {
                if (trace != null)
                {
                    tracelist.Add(trace);
                    wrapper.Tracing.Trace = tracelist.ToArray();
                    Monosoft.Common.MessageQueue.EventClient.Instance.RaiseEvent("diagnostics.trace", new DTO.EventDTO(trace, wrapper.Clientid, wrapper.Messageid));
                }
            }

            return System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(returnObj));
        }

        /// <summary>
        /// Handles the basic messageflow for events
        /// </summary>
        /// <param name="servicename">name of the calling service</param>
        /// <param name="ea">the rabbitMQ delivery event</param>
        /// <param name="handler">the message handler for handling specifik logic</param>
        public static void HandleEvent(string servicename, BasicDeliverEventArgs ea, EventHandler handler)
        {
            string[] topicparts = ea.RoutingKey.Split(".");
            if (ea.Body.Length != 0)
            {
                var wrapper = Newtonsoft.Json.JsonConvert.DeserializeObject<Common.DTO.EventDTO>(System.Text.Encoding.UTF8.GetString(ea.Body));
                handler?.Invoke(topicparts, wrapper);
            }
        }
    }
}