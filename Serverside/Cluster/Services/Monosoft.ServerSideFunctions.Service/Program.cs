using Monosoft.Common.MessageQueue;
using System;
using System.Reflection;

namespace Monosoft.ServerSideFunctions.Service
{
    class Program
    {
        private static readonly RequestConfiguration ServerSideFunctionsConfig = new RequestConfiguration(GlobalValues.ServiceName() + "_serverSideFunctionsQueue", "functions.#", MessageHandlers.ServerSideFunctionsQueueHandler.HandleMessage);

        static void Main(string[] args)
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
                    ServerSideFunctionsConfig,
                    Common.TokenHandler.TokenEventHandler.InvalidateConfig(GlobalValues.ServiceName())
                });
        }
    }
}
