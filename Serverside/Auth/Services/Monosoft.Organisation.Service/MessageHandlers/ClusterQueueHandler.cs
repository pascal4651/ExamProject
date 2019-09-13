// <copyright file="ClusterQueueHandler.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Organisation.Service.MessageHandlers
{
    using System;
    using System.Linq;
    using Monosoft.Common.DTO;
    using Monosoft.Organisation.Service.Datalayer;
    using Monosoft.Organisation.Service.MessageHandlers;
    using Monosoft.Service.OrganisationDB.Datalayer;

    /// <summary>
    /// Cluster messages handler
    /// </summary>
    internal class ClusterQueueHandler
    {
        /// <summary>
        /// Handles messages regardring cluster
        /// </summary>
        /// <param name="topicparts">topic parts</param>
        /// <param name="wrapper">wrapper</param>
        /// <returns>null</returns>
        public static ReturnMessageWrapper HandleMessage(string[] topicparts, Common.DTO.MessageWrapper wrapper)
        {
            CallContext cc = new CallContext(
                wrapper.OrgContext,
                new Common.DTO.Token() { Scope = GlobalValues.Scope, Tokenid = wrapper.UserContextToken },
                wrapper.IssuedDate);
            if (cc.IsSystemAdministrator)
            {
                var operation = topicparts[1];
                Monosoft.Auth.DTO.Cluster cluster = new Auth.DTO.Cluster();
                if (wrapper.MessageData != null)
                {
                    Auth.DTO.Cluster result = null;
                    switch (operation)
                    {
                        case "create": // TESTET OK: 30-09-2018
                            cluster = Common.DTO.MessageWrapperHelper<Monosoft.Auth.DTO.Cluster>.GetData(wrapper);
                            result = Cluster.ConvertToDTO(Cluster.Create(cluster, cc.OrganisationId.Value));
                            Common.MessageQueue.EventClient.Instance.RaiseEvent(GlobalValues.RouteClusterCreated, new Common.DTO.EventDTO(result, wrapper.Clientid, wrapper.Messageid));
                            return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, result);
                        case "update": // TESTET OK: 30-09-2018
                            cluster = Common.DTO.MessageWrapperHelper<Monosoft.Auth.DTO.Cluster>.GetData(wrapper);
                            result = Cluster.ConvertToDTO(Cluster.Update(cluster, cc.OrganisationId.Value));
                            Common.MessageQueue.EventClient.Instance.RaiseEvent(GlobalValues.RouteClusterUpdated, new Common.DTO.EventDTO(result, wrapper.Clientid, wrapper.Messageid));
                            return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, result);
                        case "delete": // TESTET OK: 30-09-2018
                            var clusterid = Common.DTO.MessageWrapperHelper<Monosoft.Common.DTO.IntIdDTO>.GetData(wrapper);
                            Common.DTO.Success isDeleted = new Common.DTO.Success();
                            isDeleted.Succeeded = Cluster.Delete(clusterid.Id, cc.OrganisationId.Value);
                            Common.MessageQueue.EventClient.Instance.RaiseEvent(GlobalValues.RouteClusterDeleted, new Common.DTO.EventDTO(isDeleted, wrapper.Clientid, wrapper.Messageid));
                            return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, isDeleted);
                        case "getbyorganisation": // TESTET OK: 30-09-2018
                            var organisationId = Common.DTO.MessageWrapperHelper<Monosoft.Common.DTO.GuidIdDTO>.GetData(wrapper);
                            Auth.DTO.Clusters getresult = new Auth.DTO.Clusters();
                            getresult.Cluster = Cluster.Read(organisationId.Id).Select(p => Cluster.ConvertToDTO(p)).ToArray();
                            Common.MessageQueue.EventClient.Instance.RaiseEvent(GlobalValues.RouteClusterRead, new Common.DTO.EventDTO(getresult, wrapper.Clientid, wrapper.Messageid));
                            return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, result);
                        default: /*log error event*/
                            Common.MessageQueue.Diagnostics.Instance.LogEvent("Unknow topic for Cluster.", operation + " is unknown", Common.DTO.Severity.Information, wrapper.OrgContext);
                            break;
                    }
                }
            }

            return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "missing credentials" } }, null);
        }
    }
}

//FEJL 
//------------
//Ser ikke ud til at reset token fungere når man får nye rettigheder!!!
