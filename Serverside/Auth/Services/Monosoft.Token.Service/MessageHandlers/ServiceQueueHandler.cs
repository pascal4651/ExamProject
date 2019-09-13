// <copyright file="ServiceQueueHandler.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Service.TokenDB.MessageHandlers
{
    using Monosoft.Common.DTO;
    using Monosoft.Database.Auth;

    /// <summary>
    /// The handler for services messages
    /// </summary>
    public class ServiceQueueHandler
    {
        /// <summary>
        /// Handle an incomming message routed for services
        /// </summary>
        /// <param name="topicparts">list of topics</param>
        /// <param name="wrapper">the message wrapper</param>
        /// <returns>null</returns>
        public static ReturnMessageWrapper HandleMessage(string[] topicparts, Common.DTO.MessageWrapper wrapper)
        {
            var operation = topicparts[1];
            switch (operation)
            {
                case "metadata":
                    var result = ClaimDefinitions.Definitions;
                    Common.MessageQueue.EventClient.Instance.RaiseEvent(Monosoft.Service.TokenDB.GlobalValues.RouteServiceMetadata, new Common.DTO.EventDTO(result, wrapper.Clientid, wrapper.Messageid));
                    break;
                default: /*log error event*/
                    break;
            }

            return null;
        }
    }
}