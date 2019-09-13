// <copyright file="UserGroupQueueHandler.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.User.Service.MessageHandlers
{
    using System;
    using System.Linq;
    using Monosoft.Common.DTO;
    using Monosoft.Database.Auth;
    using Monosoft.Database.Auth.Datalayer;

    /// <summary>
    /// Handle usergroup messages
    /// </summary>
    public class UserGroupQueueHandler
    {
        /// <summary>
        /// Handle incomming usergroup requests
        /// </summary>
        /// <param name="topicparts">list of topicparts</param>
        /// <param name="wrapper">messagewrapper</param>
        /// <returns>null</returns>
        public static ReturnMessageWrapper HandleMessage(string[] topicparts, Common.DTO.MessageWrapper wrapper)
        {
            var operation = topicparts[1];
            if (wrapper.MessageData != null)
            {
                CallContext cc = new CallContext(
                    wrapper.OrgContext,
                    new Common.DTO.Token() { Scope = wrapper.Scope, Tokenid = wrapper.UserContextToken },
                    null,
                    wrapper.IssuedDate,
                    Monosoft.User.Service.GlobalValues.Scope);

                if (cc.IsAdministrator)
                {
                    switch (operation)
                    {
                        case "create": // TESTET OK: 28-09-2018
                            var createusergrp = Common.DTO.MessageWrapperHelper<Monosoft.Auth.DTO.UserGroup>.GetData(wrapper);
                            var affectedUsersCreate = UserGroup.CreateUserGroup(cc, createusergrp);
                            if (affectedUsersCreate != null)
                            {
                                Common.MessageQueue.EventClient.Instance.RaiseEvent(
                                    Monosoft.User.Service.GlobalValues.RouteUserGroupCreated + "." + wrapper.OrgContext,
                                    new Common.DTO.EventDTO(createusergrp, wrapper.Clientid, wrapper.Messageid));

                                var eventdata = CreateEventData(wrapper.Clientid, wrapper.Messageid, affectedUsersCreate);
                                Common.MessageQueue.EventClient.Instance.RaiseEvent(
                                    Monosoft.User.Service.GlobalValues.RouteTokenInvalidateUser,
                                    eventdata);
                            }

                            return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, createusergrp);
                        case "update": // TESTET OK: 28-09-2018
                            var updateusergrp = Common.DTO.MessageWrapperHelper<Monosoft.Auth.DTO.UserGroup>.GetData(wrapper);
                            var affectedUsersUpdate = UserGroup.UpdateUserGroup(cc, updateusergrp);
                            if (affectedUsersUpdate != null)
                            {
                                Common.MessageQueue.EventClient.Instance.RaiseEvent(
                                    Monosoft.User.Service.GlobalValues.RouteUserGroupUpdated + "." + wrapper.OrgContext,
                                    new Common.DTO.EventDTO(updateusergrp, wrapper.Clientid, wrapper.Messageid));

                                var eventdata = CreateEventData(wrapper.Clientid, wrapper.Messageid, affectedUsersUpdate);
                                Common.MessageQueue.EventClient.Instance.RaiseEvent(
                                    Monosoft.User.Service.GlobalValues.RouteTokenInvalidateUser,
                                    eventdata);
                            }

                            return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, updateusergrp);
                        case "delete": // TESTET OK: 28-09-2018
                            var deleteusrgrp = Common.DTO.MessageWrapperHelper<Monosoft.Auth.DTO.UserGroupId>.GetData(wrapper);
                            var affectedUsersDelete = UserGroup.DeleteUserGroup(cc, deleteusrgrp);
                            if (affectedUsersDelete != null)
                            {
                                Common.MessageQueue.EventClient.Instance.RaiseEvent(
                                    Monosoft.User.Service.GlobalValues.RouteUserGroupDeleted + "." + wrapper.OrgContext,
                                    new Common.DTO.EventDTO(deleteusrgrp, wrapper.Clientid, wrapper.Messageid));

                                var eventdata = CreateEventData(wrapper.Clientid, wrapper.Messageid, affectedUsersDelete);
                                Common.MessageQueue.EventClient.Instance.RaiseEvent(
                                    Monosoft.User.Service.GlobalValues.RouteTokenInvalidateUser,
                                    eventdata);
                            }

                            return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, deleteusrgrp);
                        case "get": // TESTET OK: 28-09-2018
                            var res = UserGroup.ReadUserGroup(cc);
                            Common.MessageQueue.EventClient.Instance.RaiseEvent(
                                Monosoft.User.Service.GlobalValues.RouteUserGroupRead,
                                new Common.DTO.EventDTO(res, wrapper.Clientid, wrapper.Messageid));

                            return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, res);
                        default: /*log error event*/
                            Common.MessageQueue.Diagnostics.Instance.LogEvent(
                                "Unknow topic for UserGroup.",
                                operation + " is unknown",
                                Common.DTO.Severity.Information,
                                wrapper.OrgContext);
                            break;
                    }
                }
            }

            return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "message data missing" } }, null);
        }

        private static Common.DTO.EventDTO CreateEventData(string clientid, string messageid, System.Collections.Generic.List<Monosoft.Auth.DTO.User> affectedUsersCreate)
        {
            return new Common.DTO.EventDTO(
                    new InvalidateUserData() { Userids = affectedUsersCreate.Select(p => p.Userid).ToArray(), ValidUntil = DateTime.Now },
                    clientid,
                    messageid);
        }
    }
}