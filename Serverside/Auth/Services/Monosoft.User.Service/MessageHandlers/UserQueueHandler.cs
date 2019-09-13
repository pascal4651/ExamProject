// <copyright file="UserQueueHandler.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.User.Service.MessageHandlers
{
    using Monosoft.Common.DTO;
    using Monosoft.Database.Auth;
    using Monosoft.Database.Auth.Datalayer;

    /// <summary>
    /// Message handler for "user" messages (i.e. user.#)
    /// </summary>
    public class UserQueueHandler
    {
        /// <summary>
        /// Handle an incomming message
        /// </summary>
        /// <param name="topicparts">The topic/route as a list of strings</param>
        /// <param name="wrapper">Message wrapper</param>
        /// <returns>NULL</returns>
        public static ReturnMessageWrapper HandleMessage(string[] topicparts, Common.DTO.MessageWrapper wrapper)
        {
            var operation = topicparts[1];

            Monosoft.Auth.DTO.User user = new Monosoft.Auth.DTO.User();
            if (wrapper.MessageData != null)
            {
                user = Common.DTO.MessageWrapperHelper<Monosoft.Auth.DTO.User>.GetData(wrapper);
            }

            CallContext cc = new CallContext(
                wrapper.OrgContext,
                new Common.DTO.Token() { Scope = wrapper.Scope, Tokenid = wrapper.UserContextToken },
                user,
                wrapper.IssuedDate,
                Monosoft.User.Service.GlobalValues.Scope);

            Common.DTO.EventDTO eventdata = null;
            switch (operation)
            {
                case "create": // TESTET OK: 28-09-2018
                    var createRes = User.CreateUser(cc);
                    var res = createRes == null ? null : createRes.Convert2DTO(cc);
                    eventdata = new Common.DTO.EventDTO(
                        res,
                        wrapper.Clientid,
                        wrapper.Messageid);
                    Common.MessageQueue.EventClient.Instance.RaiseEvent(Monosoft.User.Service.GlobalValues.RouteUserCreated + "." + wrapper.OrgContext, eventdata);
                    if (createRes == null)
                    {
                        return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, res);
                    }
                    else
                    {
                        return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "Missing rights" } }, res);
                    }

                case "update": // TESTET OK: 28-09-2018
                    var updatedUser = User.UpdateUser(cc);
                    var updatedResult = updatedUser == null ? null : updatedUser.Convert2DTO(cc);
                    eventdata = new Common.DTO.EventDTO(
                        updatedResult,
                        wrapper.Clientid,
                        wrapper.Messageid);
                    Common.MessageQueue.EventClient.Instance.RaiseEvent(Monosoft.User.Service.GlobalValues.RouteUserUpdated + "." + wrapper.OrgContext, eventdata);
                    if (updatedUser == null)
                    {
                        return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, updatedResult);
                    }
                    else
                    {
                        return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "Missing rights" } }, updatedResult);
                    }

                case "delete": // TESTET OK: 28-09-2018
                    User.DeleteUser(cc);
                    eventdata = new Common.DTO.EventDTO(
                        user,
                        wrapper.Clientid,
                        wrapper.Messageid);
                    Common.MessageQueue.EventClient.Instance.RaiseEvent(Monosoft.User.Service.GlobalValues.RouteTokenInvalidateUser, eventdata);
                    return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, null);
                case "get":
                    var userRes = User.ReadUser(cc);
                    Common.MessageQueue.EventClient.Instance.RaiseEvent(
                        Monosoft.User.Service.GlobalValues.RouteUserRead,
                        new Common.DTO.EventDTO(userRes, wrapper.Clientid, wrapper.Messageid));
                    return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, userRes);
                case "getall": // TESTET OK: 28-09-2018
                    var usersRes = User.ReadUsers(cc);
                    Common.MessageQueue.EventClient.Instance.RaiseEvent(
                        Monosoft.User.Service.GlobalValues.RouteUserRead + "." + wrapper.OrgContext,
                        new Common.DTO.EventDTO(usersRes.ToArray(), wrapper.Clientid, wrapper.Messageid));//TODO ikke lovlig resultat...
                    return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, usersRes.ToArray());//TODO ikke lovlig resultat...
                default: /*log error event*/
                    Common.MessageQueue.Diagnostics.Instance.LogEvent(
                        "Unknow topic for User.",
                        operation + " is unknown",
                        Common.DTO.Severity.Information,
                        wrapper.OrgContext);
                    break;
            }

            return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "unknown situation" } }, null);
        }
    }
}