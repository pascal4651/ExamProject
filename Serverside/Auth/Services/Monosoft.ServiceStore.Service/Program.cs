// <copyright file="Program.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Service.UserDB
{
    using System;
    using System.Reflection;
    using Monosoft.Common.MessageQueue;

    /// <summary>
    /// UserDB contains logic for handling security (Users and usergroups through tokens) through rabbitMQ
    /// </summary>
    internal class Program
    {
        private static readonly RequestConfiguration ServiceStoreConfig = new RequestConfiguration(GlobalValues.ServiceName() + "_servicestore", "servicestore.#", MessageHandlers.ServiceStoreQueueHandler.HandleMessage);

        private static void Main(string[] args)
        {
            Assembly thisAssem = typeof(Program).Assembly;
            AssemblyName thisAssemName = thisAssem.GetName();
            Version ver = thisAssemName.Version;
            Console.WriteLine(thisAssemName.Name);
            Console.WriteLine("ver: " + ver.ToString());

            // start diagnostics for logevent and heatbeats....
            Common.MessageQueue.Diagnostics.Initialize(GlobalValues.ServiceName(), ver.ToString());

            // start the request server (for handling incomming requests)
            var server = new Common.MessageQueue.RequestServer(
                new System.Collections.Generic.List<MessageQueueConfiguration>()
                {
                    ServiceStoreConfig,
                    Monosoft.Common.TokenHandler.TokenEventHandler.InvalidateConfig(GlobalValues.ServiceName())
                });
        }
    }
}