// <copyright file="Program.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Service.TokenDB
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Monosoft.Common.DTO;
    using Monosoft.Common.MessageQueue;

    /// <summary>
    /// TokenDB contains logic for handling security (Users and usergroups through tokens) through rabbitMQ
    /// </summary>
    internal class Program
    {
        private static readonly RequestConfiguration TokenConfig = new RequestConfiguration(GlobalValues.ServiceName() + "_token", "token.#", MessageHandlers.TokenQueueHandler.HandleMessage);
        private static readonly RequestConfiguration ServiceConfig = new RequestConfiguration(GlobalValues.ServiceName() + "_service", "service.#", MessageHandlers.ServiceQueueHandler.HandleMessage);

        private static void Main(string[] args)
        {
            Assembly thisAssem = typeof(Program).Assembly;
            AssemblyName thisAssemName = thisAssem.GetName();
            Version ver = thisAssemName.Version;
            Console.WriteLine(thisAssemName.Name);
            Console.WriteLine("ver: " + ver.ToString());

            // start diagnostics for logevent and heatbeats....
            Common.MessageQueue.Diagnostics.Initialize(GlobalValues.ServiceName(), ver.ToString());
            MemoryCache.Instance.OnTokenInvalidated += OnTokenInvalidatedEvent;

            // start the request server (for handling incomming requests)
            var server = new Common.MessageQueue.RequestServer(new System.Collections.Generic.List<MessageQueueConfiguration>() { ServiceConfig, TokenConfig, Monosoft.Common.TokenHandler.TokenEventHandler.InvalidateConfig(GlobalValues.ServiceName()) });
        }

        private static void OnTokenInvalidatedEvent(Guid tokenid, DateTime validuntil)
        {
            var token = Database.Auth.Datalayer.DataContext.Instance.Tokens.Where(p => p.Id == tokenid).FirstOrDefault();
            if (token != null && token.ValidUntil > validuntil)
            {
                token.ValidUntil = validuntil;
                Database.Auth.Datalayer.DataContext.Instance.SaveChanges();
            }
        }
    }
}