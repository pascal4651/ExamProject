using Monosoft.Common.DTO;
using Monosoft.Auth.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using Monosoft.Common.MessageQueue;
using Monosoft.Common.Utils;
using System.Linq;

namespace Monosoft.Auth.Service.MessageHandlers
{
    public class AuthQueueHandler
    {        /// <summary>
             /// Handle an incomming message
             /// </summary>
             /// <param name="topicparts">The topic/route as a list of strings</param>
             /// <param name="wrapper">Message wrapper</param>
             /// <returns>topicparts[1] == "login" ? LoginInformation DTO</returns>
        public static ReturnMessageWrapper HandleMessage(string[] topicparts, Common.DTO.MessageWrapper wrapper)
        {
            var operation = topicparts[1];

            switch (operation)
            {
                case "login":
                    byte[] tokenBytes = RequestClient.Instance.Rpc("token.login", MessageDataHelper.ToMessageData(wrapper));
                    ReturnMessageWrapper returnTokenData = MessageDataHelper.FromMessageData<ReturnMessageWrapper>(tokenBytes);
                    TokenData token = MessageDataHelper.FromMessageData<TokenData>(returnTokenData.Data);

                    Organisations organisations = null;
                    User user = null;

                    if (token.Tokenid != Guid.Empty)
                    {
                        // Get full user data on user id
                        MessageWrapper messageWrapper = new MessageWrapper(wrapper);
                        messageWrapper.UserContextToken = token.Tokenid;
                        User userData = new User();
                        userData.Userid = token.Userid;
                        messageWrapper.MessageData = MessageDataHelper.ToMessageData(userData);
                        byte[] returnUserBytes = RequestClient.Instance.Rpc("user.get", MessageDataHelper.ToMessageData(messageWrapper));
                        ReturnMessageWrapper returnUserData = MessageDataHelper.FromMessageData<ReturnMessageWrapper>(returnUserBytes);
                        user = MessageDataHelper.FromMessageData<User>(returnUserData.Data);

                        // Get full organisation data on organisation ids
                        MessageWrapper orgMessageWrapper = new MessageWrapper(messageWrapper);
                        GuidIdsDTO orgIds = new GuidIdsDTO();
                        if (user.Organisations == null)
                            orgIds.Ids = new Guid[0];
                        else
                        {
                            orgIds.Ids = user.Organisations.Select(p => p.Id).ToArray();
                            orgMessageWrapper.MessageData = MessageDataHelper.ToMessageData(orgIds);
                            byte[] returbOrgsBytes = RequestClient.Instance.Rpc("organisation.getbyids", MessageDataHelper.ToMessageData(orgMessageWrapper));
                            ReturnMessageWrapper returbOrgsData = MessageDataHelper.FromMessageData<ReturnMessageWrapper>(returbOrgsBytes);
                            organisations = MessageDataHelper.FromMessageData<Organisations>(returbOrgsData.Data);
                        }
                    }

                    // Constructing our combined result
                    LoginInformation result = new LoginInformation(token, user, organisations);

                    EventClient.Instance.RaiseEvent(GlobalValues.RouteLoginRead, new EventDTO(result, wrapper.Clientid, wrapper.Messageid));
                    return ReturnMessageWrapper.CreateResult(true, wrapper, new List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, result);
                case "initorganisation": // createorganisation":
                    var tokendata = Monosoft.Common.DTO.MemoryCache.FindToken(new Common.DTO.Token() { Scope = GlobalValues.Scope, Tokenid = wrapper.UserContextToken }, wrapper.OrgContext);
                    if (tokendata.IsValidToken())
                    {
                        // 1. create organisation
                        byte[] returnUserBytes = RequestClient.Instance.Rpc("organisation.create", MessageDataHelper.ToMessageData(wrapper));
                        ReturnMessageWrapper returnOrgData = MessageDataHelper.FromMessageData<ReturnMessageWrapper>(returnUserBytes);
                        if (returnOrgData.Success)
                        {
                            Organisation org = MessageDataHelper.FromMessageData<Organisation>(returnOrgData.Data);

                            // 2. create user relation to the organisation
                            Monosoft.Auth.DTO.UserGroup defaultUserGroup = new UserGroup()
                            {
                                 Name = "Default",
                                 Claims = new MetaData[] { new MetaData() { Key = "isAdmin", Scope = "Monosoft.Service.AUTH", Value = "true" } },
                                 Organisationid = org.Id,
                                 Users = new User[] { new User() { Userid = tokendata.Userid } }
                            };
                            MessageWrapper messageWrapperUserGroup = new MessageWrapper(wrapper);
                            messageWrapperUserGroup.MessageData = MessageDataHelper.ToMessageData(defaultUserGroup);
                            byte[] returnUsergroupBytes = RequestClient.Instance.Rpc("usergroup.create", MessageDataHelper.ToMessageData(messageWrapperUserGroup));
                            ReturnMessageWrapper returnUserGroupData = MessageDataHelper.FromMessageData<ReturnMessageWrapper>(returnUserBytes);
                            if (returnUserGroupData.Success)
                            {
                                var createOrgresult = new Monosoft.Common.DTO.BoolDTO() { Value = true };
                                EventClient.Instance.RaiseEvent(GlobalValues.RouteLoginRead, new EventDTO(createOrgresult, wrapper.Clientid, wrapper.Messageid));
                                return ReturnMessageWrapper.CreateResult(true, wrapper, new List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, createOrgresult);
                            }
                        }
                    }

                    var createOrgNegativeResult = new Monosoft.Common.DTO.BoolDTO() { Value = false };
                    EventClient.Instance.RaiseEvent(GlobalValues.RouteLoginRead, new EventDTO(createOrgNegativeResult, wrapper.Clientid, wrapper.Messageid));
                    return ReturnMessageWrapper.CreateResult(true, wrapper, new List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "Something went wrong, please contact support" } }, createOrgNegativeResult);
                default: /* log error event*/
                    Common.MessageQueue.Diagnostics.Instance.LogEvent(
                        "Unknow topic for Auth.",
                        operation + " is unknown",
                        Common.DTO.Severity.Information,
                        wrapper.OrgContext);
                    break;
            }

            return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "unknown situation" } }, null);

        }
    }
}
