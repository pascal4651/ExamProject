// <copyright file="Program.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.ZooKeeper.Service
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using Monosoft.Common.DTO;
    using Monosoft.Common.MessageQueue;
    using Monosoft.ServiceStore.DTO;

    internal class Program
    {
        private static List<ProcessManager> manager = new List<ProcessManager>();
        private static List<ServiceProgramVersion> programs = new List<ServiceProgramVersion>(); // todo, from server
        private static readonly RequestConfiguration ZookeeperConfig = new RequestConfiguration(GlobalValues.ServiceName() + "_zookeeperQueue", "zookeeper.#", MessageHandlers.ZookeeperQueueHandler.HandleMessage);


        private static void Main(string[] args)
        {
            Assembly thisAssem = typeof(Program).Assembly;
            AssemblyName thisAssemName = thisAssem.GetName();
            Version ver = thisAssemName.Version;
            Console.WriteLine(thisAssemName.Name);
            Console.WriteLine("ver: " + ver.ToString());

            // start diagnostics for logevent and heatbeats....
            Common.MessageQueue.Diagnostics.Initialize(GlobalValues.ServiceName(), ver.ToString());

            var settings = ZooKeeperDefinition.LoadFromDisk();
            try
            {
                while (true)
            {
                var server = new Common.MessageQueue.RequestServer(
                    new System.Collections.Generic.List<MessageQueueConfiguration>()
                    {
                        ZookeeperConfig,
                        Monosoft.Common.TokenHandler.TokenEventHandler.InvalidateConfig(GlobalValues.ServiceName())
                    },
                    10);

                foreach (var s in settings.Services)
                {
                    string directory = Path.Combine(Environment.CurrentDirectory, $"services/{s.ServiceInfo.Id}");

                    ProgamVersion version = new ProgamVersion("0.0.0.0");
                    var versions = ServiceProgramVersion.FindLocalVersions(s.ServiceInfo.Id);
                    if (versions.Any())
                    {
                        version = versions.FirstOrDefault();
                    }

                    ServiceProgramVersion program = DownloadNewVersion(s, version);
                    bool restartNeeded = InstallNewVersion(directory, program);
                    if (program != null)
                    {// New version installed! use the new version
                        version = program.Version;
                    }

                    string executeableDir = string.Format($"./services/{s.ServiceInfo.Id}/{version.Version}/");
                    string executeableName = string.Empty;

                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        executeableName = string.Format($"{s.ServiceInfo.Name}");
                    }
                    else
                    {
                        executeableName = string.Format($"{s.ServiceInfo.Name}.exe");
                    }

                    if (restartNeeded && RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    { // give access to the executable on linux...
                        Console.WriteLine("Execute: sudo chmod 777 on " + executeableDir+executeableName);
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("sudo", $"chmod 777 {executeableDir+executeableName}") {  UseShellExecute = false });
                    }

                    // start
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        var runningProcess = manager.Where(p => p.Service.Id == s.ServiceInfo.Id).FirstOrDefault();
                        if (runningProcess == null)
                        {
                            Console.WriteLine($"{s.ServiceInfo.Name} isnt running, starting");
                            Console.WriteLine("Execute: " + executeableDir + executeableName);

                            var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(executeableDir+executeableName) { WorkingDirectory = executeableDir, UseShellExecute = false });
                            manager.Add(new ProcessManager() { Process = process, Service = s.ServiceInfo });
                        }
                        else if (runningProcess.Process.HasExited == true)
                        {
                            Console.WriteLine($"{s.ServiceInfo.Name} has exited, restarting"); // TODO: report exitcode!!!
                            Console.WriteLine("Execute: " + executeableDir + executeableName);

                            // RESTART
                            runningProcess.Process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(executeableDir+executeableName) { WorkingDirectory = executeableDir, UseShellExecute = false });
                        }
                        else if (restartNeeded)
                        {
                            Console.WriteLine("RESTART NEEDED");
                            Console.WriteLine("Execute: " + executeableDir + executeableName);

                            runningProcess.Process.Kill(); // TODO: AUCH: close er nok bedre - men så skal vi sikre at alle services faktisk tillader dette...
                            runningProcess.Process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(executeableDir+executeableName) { WorkingDirectory = executeableDir, UseShellExecute = false });
                        }
                    }
                }

                int seconds = 1000;
                int minutes = 60 * seconds;
                int hours = 60 * minutes;
                Console.WriteLine("Sleep for 1 hour - before recheck");
                System.Threading.Thread.Sleep(1 * hours);
                }
            }
            finally
            {
                foreach (var s in settings.Services)
                {
                    var runningProcess = manager.Where(p => p.Service.Id == s.ServiceInfo.Id).FirstOrDefault();
                    Console.WriteLine("Killing: " + s.ServiceInfo.Name);
                    runningProcess.Process.Kill();
                }
            }
        }

        private static bool InstallNewVersion(string directory, ServiceProgramVersion program)
        {
            if (program != null)
            {
                var installDir = program.SaveToDisk();
                program.InstallToDisk();
                return true;
            }

            return false;
        }

        private static ServiceProgramVersion DownloadNewVersion(ServiceDefinition s, ProgamVersion version)
        {
            Common.DTO.MessageWrapper mw = new Common.DTO.MessageWrapper(
                                        DateTime.Now,
                                        Guid.Empty /*no user needed*/,
                                        "ZooKeeper",
                                        "N/A", // ??
                                        "N/A", // ??
                                        "N/A", // ??
                                        Guid.Empty /*no org needed*/);
            var data = new ServiceDownloadDefinition()
            {
                AutoUpdate = s.AutoUpdateSetting,
                ServiceId = s.ServiceInfo.Id,
                CurrentVersion = version
            };
            Common.DTO.MessageWrapperHelper<ServiceDownloadDefinition>.SetData(mw, data);
            Console.WriteLine("Looking for updates: " + s.ServiceInfo.Name);

            byte[] rpc_res = RequestClient.Instance.Rpc("servicestore.download", System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(mw)));
            var resstring = System.Text.Encoding.UTF8.GetString(rpc_res);
            if (resstring == "null")
            {
                return null;
            }
            var wrapper = Newtonsoft.Json.JsonConvert.DeserializeObject<ReturnMessageWrapper>(resstring);
            var innerres = System.Text.Encoding.UTF8.GetString(wrapper.Data);
            var program = Newtonsoft.Json.JsonConvert.DeserializeObject<ServiceProgramVersion>(innerres);
            return program;
        }
    }
}
