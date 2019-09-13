// <copyright file="ServiceStoreQueueHandler.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Service.UserDB.MessageHandlers
{
    using Monosoft.Common.DTO;
    using Monosoft.ServiceStore.DTO;
    using Monosoft.ServiceStore.Service.MessageHandlers;

    /// <summary>
    /// Message handler for "user" messages (i.e. user.#)
    /// </summary>
    public class ServiceStoreQueueHandler
    {
        /// <summary>
        /// Handle an incomming message
        /// </summary>
        /// <param name="topicparts">The topic/route as a list of strings</param>
        /// <param name="wrapper">Message wrapper</param>
        /// <returns>NULL</returns>
        public static ReturnMessageWrapper HandleMessage(string[] topicparts, Common.DTO.MessageWrapper wrapper)
        {
            CallContext cc = new CallContext(
                wrapper.OrgContext,
                new Common.DTO.Token() { Tokenid = wrapper.UserContextToken, Scope = GlobalValues.Scope },
                wrapper.IssuedDate);

            var operation = topicparts[1];

            switch (operation)
            {
                case "download": // BEMÆRK RPC !!!
                    ServiceDownloadDefinition downloadDef = Common.DTO.MessageWrapperHelper<ServiceDownloadDefinition>.GetData(wrapper);
                    var foundupdate = downloadDef.FindUpdate();
                    Common.MessageQueue.EventClient.Instance.RaiseEvent("servicestore.newversion." + downloadDef.ServiceId, new Common.DTO.EventDTO(foundupdate, wrapper.Clientid, wrapper.Messageid));
                    return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, foundupdate);
                // break;
                case "upload":
                    if (cc.IsServiceStoreAdmin)
                    {
                        ServiceProgramVersion program = Common.DTO.MessageWrapperHelper<ServiceProgramVersion>.GetData(wrapper);
                        program.SaveToDisk();
                        Common.MessageQueue.EventClient.Instance.RaiseEvent("servicestore.newserviceupdate", new Common.DTO.EventDTO(program.Version, string.Empty, string.Empty));
                        return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, program.Version);
                    }
                    break;
                case "create":
                    if (cc.IsServiceStoreAdmin)
                    {
                        Monosoft.ServiceStore.DTO.Service createservice = Common.DTO.MessageWrapperHelper<Monosoft.ServiceStore.DTO.Service>.GetData(wrapper);
                        createservice.SaveToDisk(false);
                        Common.MessageQueue.EventClient.Instance.RaiseEvent("servicestore.newservice", new Common.DTO.EventDTO(createservice, string.Empty, string.Empty));
                        return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, createservice);
                    }

                    break;
                case "update":
                    if (cc.IsServiceStoreAdmin)
                    {
                        Monosoft.ServiceStore.DTO.Service updateservice = Common.DTO.MessageWrapperHelper<Monosoft.ServiceStore.DTO.Service>.GetData(wrapper);
                        updateservice.SaveToDisk(true);
                        Common.MessageQueue.EventClient.Instance.RaiseEvent("servicestore.serviceupdate", new Common.DTO.EventDTO(updateservice, string.Empty, string.Empty));
                        return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, updateservice);
                    }

                    break;
                case "delete":
                    if (cc.IsServiceStoreAdmin)
                    {
                        Monosoft.ServiceStore.DTO.Service deleteservice = Common.DTO.MessageWrapperHelper<Monosoft.ServiceStore.DTO.Service>.GetData(wrapper);
                        deleteservice.Delete();
                        Common.MessageQueue.EventClient.Instance.RaiseEvent("servicestore.servicedelete", new Common.DTO.EventDTO(deleteservice, string.Empty, string.Empty));
                        return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, deleteservice);
                    }

                    break;
                case "get":
                    if (cc.IsServiceStoreAdmin)
                    {
                        var res = ServicesInformation.ServiceInformation();
                        Common.MessageQueue.EventClient.Instance.RaiseEvent("servicestore.servicelist", new Common.DTO.EventDTO(res, wrapper.Clientid, wrapper.Messageid));
                        return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, res);
                    }

                    break;
                default: /*log error event*/
                    Common.MessageQueue.Diagnostics.Instance.LogEvent(
                        "Unknow topic for ServiceStore.",
                        operation + " is unknown",
                        Common.DTO.Severity.Information,
                        wrapper.OrgContext);
                    break;
            }

            return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "missing credentials" } }, null);
        }
    }
}