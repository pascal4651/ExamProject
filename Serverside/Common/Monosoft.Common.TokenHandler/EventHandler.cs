// <copyright file="TokenQueueHandler.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

using Monosoft.Common.DTO;
using Monosoft.Common.MessageQueue;
using System;

namespace Monosoft.Common.TokenHandler
{

    /// <summary>
    /// Handle incomming token events
    /// </summary>
    public class TokenEventHandler
    {
        public static EventConfiguration InvalidateConfig(string servicename)
        {
            return new EventConfiguration("token_invalidate_" + servicename, "token.invalidate.#", Monosoft.Common.TokenHandler.TokenEventHandler.HandelEvent);
        }

        /// <summary>
        /// Handles incomming token events
        /// </summary>
        /// <param name="topicparts">list of topics</param>
        /// <param name="data">the event dto</param>
        public static void HandelEvent(string[] topicparts, Monosoft.Common.DTO.EventDTO data)
        {
            var operation = topicparts[1];
            var section = topicparts[2];
            var json = System.Text.Encoding.UTF8.GetString(data.Data);

            switch (operation)
            {
                case "invalidate":
                    if (section == "token")
                    {
                        var token = Newtonsoft.Json.JsonConvert.DeserializeObject<Monosoft.Common.DTO.TokenData>(json);
                        Console.WriteLine(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + ": " + "Invalidating token: " + token.Tokenid);
                        MemoryCache.Instance.InvalidateToken(token);
                    } 
                    if (section == "user")
                    {
                        var users = Newtonsoft.Json.JsonConvert.DeserializeObject<InvalidateUserData>(json);
                        Console.WriteLine(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + ": " + "Invalidating users: " + string.Join(",", users.Userids));
                        MemoryCache.Instance.InvalidateUsers(users);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}