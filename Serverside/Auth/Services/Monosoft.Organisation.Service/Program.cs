// <copyright file="Program.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Organisation.Service
{
    using System;
    using System.Reflection;
    using Monosoft.Common.MessageQueue;

    /// <summary>
    /// Program for the organisationDB microservice
    /// </summary>
    internal class Program
    {
        private static readonly RequestConfiguration ClusterConfig =
            new RequestConfiguration(GlobalValues.ServiceName() + "_clusterQueue", "cluster.#", MessageHandlers.ClusterQueueHandler.HandleMessage);

        private static readonly RequestConfiguration OrganisationConfig =
            new RequestConfiguration(GlobalValues.ServiceName() + "_organisationQueue", "organisation.#", MessageHandlers.OrganistationQueueHandler.HandleMessage);

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
            var server = new Common.MessageQueue.RequestServer(new System.Collections.Generic.List<MessageQueueConfiguration>() { ClusterConfig, OrganisationConfig, Monosoft.Common.TokenHandler.TokenEventHandler.InvalidateConfig(GlobalValues.ServiceName()) });
        }
    }
}