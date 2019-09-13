// <copyright file="organistationQueueHandler.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Organisation.Service.MessageHandlers
{
    using System;
    using System.Linq;
    using Monosoft.Common.DTO;
    using Monosoft.Organisation.Service.Datalayer;

    /// <summary>
    /// Message handler for organisation messages
    /// </summary>
    internal class OrganistationQueueHandler
    {
        /// <summary>
        /// The message handler logic for organisation messages
        /// </summary>
        /// <param name="topicparts">topic parts</param>
        /// <param name="wrapper">wrapper object</param>
        /// <returns>resulting object or null</returns>
        public static ReturnMessageWrapper HandleMessage(string[] topicparts, Common.DTO.MessageWrapper wrapper)
        {
            CallContext cc = new CallContext(
                wrapper.OrgContext,
                new Common.DTO.Token() { Tokenid = wrapper.UserContextToken, Scope = GlobalValues.Scope},
                wrapper.IssuedDate);
            var operation = topicparts[1];
            Monosoft.Auth.DTO.Organisation organisation = new Auth.DTO.Organisation();

            if (cc.IsKeyAccountManager)
            {
                if (wrapper.MessageData != null)
                {
                    Auth.DTO.Organisation result = null;
                    switch (operation)
                    {
                        case "create": // TESTET OK: 30-09-2018
                            organisation = Common.DTO.MessageWrapperHelper<Monosoft.Auth.DTO.Organisation>.GetData(wrapper);
                            result = Organisation.ConvertToDTO(cc.Scope, Organisation.Create(organisation, cc.Scope));
                            Common.MessageQueue.EventClient.Instance.RaiseEvent(GlobalValues.RouteOrganisationCreated, new Common.DTO.EventDTO(result, wrapper.Clientid, wrapper.Messageid));
                            return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, result);
                        case "update": // TESTET OK: 30-09-2018
                            organisation = Common.DTO.MessageWrapperHelper<Monosoft.Auth.DTO.Organisation>.GetData(wrapper);
                            result = Organisation.ConvertToDTO(cc.Scope, Organisation.Update(organisation, cc.Scope));
                            Common.MessageQueue.EventClient.Instance.RaiseEvent(GlobalValues.RouteOrganisationUpdated, new Common.DTO.EventDTO(result, wrapper.Clientid, wrapper.Messageid));
                            return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, result);
                        case "delete": // TESTET OK: 30-09-2018
                            var deleteid = Common.DTO.MessageWrapperHelper<Monosoft.Common.DTO.GuidIdDTO>.GetData(wrapper);
                            Common.DTO.Success isDeleted = new Common.DTO.Success();
                            isDeleted.Succeeded = Organisation.Delete(deleteid.Id);
                            Common.MessageQueue.EventClient.Instance.RaiseEvent(GlobalValues.RouteOrganisationDeleted, new Common.DTO.EventDTO(isDeleted, wrapper.Clientid, wrapper.Messageid));
                            return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, isDeleted);
                        default: /*log error event*/
                            // Common.MessageQueue.Diagnostics.Instance.LogEvent("Unknow topic for Organisation.", operation + " is unknown", Common.DTO.Severity.Information, wrapper.OrgContext);
                            break;
                    }
                }
            }

            if (wrapper.MessageData != null)
            { // operations without any security conserns
                switch (operation)
                {
                    case "create":
                    case "update":
                    case "delete":
                        Common.MessageQueue.Diagnostics.Instance.LogEvent("Missing credentials.", "Missing credentials for " + operation + " is unknown", Common.DTO.Severity.Information, wrapper.OrgContext);
                        return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "missing credentials" } }, null);
                    case "getbyids":
                        var getbyid = Common.DTO.MessageWrapperHelper<Monosoft.Common.DTO.GuidIdsDTO>.GetData(wrapper);
                        var orgsList = Organisation.GetByIds(getbyid.Ids).Select(p => Organisation.ConvertToDTOWoDetails(cc.Scope, p)).ToList();
                        var results = Organisation.ConvertToDTO(cc.Scope, orgsList);
                        Common.MessageQueue.EventClient.Instance.RaiseEvent(GlobalValues.RouteOrganisationRead, new Common.DTO.EventDTO(results, wrapper.Clientid, wrapper.Messageid));
                        return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, results);
                    default: /*log error event*/
                        Common.MessageQueue.Diagnostics.Instance.LogEvent("Unknow topic for Organisation.", operation + " is unknown", Common.DTO.Severity.Information, wrapper.OrgContext);
                        return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = operation + " is unknown" } }, null);
                }
            }

            Common.MessageQueue.Diagnostics.Instance.LogEvent("Missing MessageData.", "MessageData is null", Common.DTO.Severity.Information, wrapper.OrgContext);
            return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "MessageData is null" } }, null);
        }
    }
}