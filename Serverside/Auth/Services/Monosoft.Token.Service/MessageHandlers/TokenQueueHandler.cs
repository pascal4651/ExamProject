// <copyright file="TokenQueueHandler.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Service.TokenDB.MessageHandlers
{
    using Monosoft.Common.DTO;
    using Monosoft.Database.Auth.Datalayer;
    using System;

    /// <summary>
    /// Handle incomming token requests
    /// </summary>
    public class TokenQueueHandler
    {
        /// <summary>
        /// Handle incomming token requests
        /// </summary>
        /// <param name="topicparts">list of topics</param>
        /// <param name="wrapper">messagewrapper</param>
        /// <returns>tokendata as byte array - but only on verify, otherwise null is returned</returns>
        public static ReturnMessageWrapper HandleMessage(string[] topicparts, Common.DTO.MessageWrapper wrapper)
        {
            var operation = topicparts[1];
            if (wrapper.MessageData != null)
            {
                switch (operation)
                {
                    // case "resetpassword"://RPC (evt. FAF for logging!!!)
                    //    //TODO: implement... request reset password.... benyt otp til reset?
                    //    return BsonHelper.ToBson<Auth.DTO.TokenData>(null);
                    // case "otp"://RPC (evt. FAF for logging!!!)
                    //    //TODO: implement... request otp, send out OTP-code, and allow that code on login below
                    //    return BsonHelper.ToBson<Auth.DTO.TokenData>(null);
                    case "login": // TESTET OK: 28-09-2018
                        var loginRes = Database.Auth.Datalayer.Token.Login(
                            Common.DTO.MessageWrapperHelper<Monosoft.Auth.DTO.Login>.GetData(wrapper),
                            wrapper.CallerIp,
                            wrapper.OrgContext,
                            wrapper.Scope);

                        foreach (var old in loginRes.oldToken)
                        {
                            Common.MessageQueue.EventClient.Instance.RaiseEvent(GlobalValues.RouteTokenInvalidateToken, new Common.DTO.EventDTO(old, wrapper.Clientid, wrapper.Messageid));
                        }

                        Common.MessageQueue.EventClient.Instance.RaiseEvent(GlobalValues.RouteTokenLogin, new Common.DTO.EventDTO(loginRes.newToken, wrapper.Clientid, wrapper.Messageid));
                        if (loginRes.newToken.Tokenid == Guid.Empty)
                        {
                            return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "Uknown user or invalid password" } }, loginRes.newToken);
                        } else
                        {
                            return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, loginRes.newToken);
                        }

                    case "verify": // TESTET OK: 28-09-2018
                        var verifyRes = Database.Auth.Datalayer.Token.Verify(
                            Common.DTO.MessageWrapperHelper<Monosoft.Common.DTO.Token>.GetData(wrapper),
                            wrapper.OrgContext);
                        Common.MessageQueue.EventClient.Instance.RaiseEvent(GlobalValues.RouteTokenVerify, new Common.DTO.EventDTO(verifyRes, wrapper.Clientid, wrapper.Messageid));
                        return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, verifyRes);
                    case "logout": // TESTET OK: 28-09-2018
                        var logoutToken = Common.DTO.MessageWrapperHelper<Monosoft.Common.DTO.Token>.GetData(wrapper);
                        var usr = Database.Auth.Datalayer.Token.Logout(logoutToken, wrapper.OrgContext);
                        Common.MessageQueue.EventClient.Instance.RaiseEvent(GlobalValues.RouteTokenInvalidateToken, new Common.DTO.EventDTO(usr, wrapper.Clientid, wrapper.Messageid));
                        return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, usr);
                    default: /*log error event*/
                        Common.MessageQueue.Diagnostics.Instance.LogEvent("Unknow topic for Event.", operation + " is unknown", Common.DTO.Severity.Information, System.Guid.Empty);
                        break;
                }
            }

            return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "missing messagedata" } }, null);
        }
    }
}