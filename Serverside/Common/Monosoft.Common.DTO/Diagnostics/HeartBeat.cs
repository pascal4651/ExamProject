// <copyright file="HeartBeat.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Common.DTO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Diagnostics information about the program and the envoriment it is running within
    /// </summary>
    public class HeartBeat
    {
        /// <summary>
        /// Gets or sets the servicename/program name
        /// </summary>
        public string Servicename { get; set; }

        /// <summary>
        /// Gets or sets the version of the service
        /// </summary>
        public string ServiceVersion { get; set; }

        /// <summary>
        /// Gets or sets the servername
        /// </summary>
        public string Servername { get; set; }

        /// <summary>
        /// Gets or sets the timestamp for when the information was gathered
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets information about the hard-drives
        /// </summary>
        public DiskInfo[] Disks { get; set; }

        /// <summary>
        /// Gets or sets CPU usage
        /// </summary>
        public CpuInfo Cpu { get; set; }

        /// <summary>
        /// Gets or sets RAM usage
        /// </summary>
        public RamInfo Ram { get; set; }

        /// <summary>
        /// Gets or sets how long the program have been running
        /// </summary>
        public TimeSpan Uptime { get; set; }

        /// <summary>
        /// Gather all informations about the server and the program
        /// </summary>
        /// <param name="servicename">Name of the service from where this is collected</param>
        /// <param name="programVersion">Versionname of the program/service</param>
        public void CollectData(string servicename, string programVersion)
        {
            this.Servername = Process.GetCurrentProcess().MachineName;
            this.Disks = DiskInfo.CollectData().ToArray();
            this.Uptime = DateTime.Now.Subtract(Process.GetCurrentProcess().StartTime);
            this.Cpu = CpuInfo.CollectData(this.Uptime);
            this.Ram = RamInfo.CollectData();
            this.Timestamp = DateTime.Now;
            this.Servicename = servicename;
            this.ServiceVersion = programVersion;
        }

        /// <summary>
        /// Provides information about a harddrive
        /// </summary>
        public class DiskInfo
        {
            /// <summary>
            /// Gets or sets the name of the drive
            /// </summary>
            public string Drivename { get; set; }

            /// <summary>
            /// Gets or sets number of total bytes available on the disk
            /// </summary>
            public long Totalbytes { get; set; }

            /// <summary>
            /// Gets or sets number of used bytes on the disk
            /// </summary>
            public long Usedbytes { get; set; }

            /// <summary>
            /// Gets or sets number of free bytes on the disk
            /// </summary>
            public long Freebytes { get; set; }

            /// <summary>
            /// Gets or sets the free space in %
            /// </summary>
            public decimal PctFree { get; set; }

            /// <summary>
            /// Collect data about all harddrives on the system
            /// </summary>
            /// <returns>List of harddrives</returns>
            public static List<DiskInfo> CollectData()
            {
                List<DiskInfo> drives = new List<DiskInfo>();
                foreach (var d in System.IO.DriveInfo.GetDrives().Where(x => x.DriveType == System.IO.DriveType.Fixed))
                {
                    if (d.TotalSize > 0 && d.IsReady)
                    {
                        drives.Add(new DiskInfo()
                        {
                            Drivename = d.Name,
                            Totalbytes = d.TotalSize,
                            Usedbytes = d.TotalSize - d.TotalFreeSpace,
                            Freebytes = d.TotalFreeSpace,
                            PctFree = (decimal)d.TotalFreeSpace / (decimal)d.TotalSize * (decimal)100
                        });
                    }
                }

                return drives;
            }
        }

        /// <summary>
        /// Information about the CPU
        /// </summary>
        public class CpuInfo
        {
            private static CpuInfo lastinfo = null;

            private double totalProcessorTimeMS = 0;

            private double runningTimeMS = 0;

            /// <summary>
            /// Gets or sets the amount of CPU time this program have used
            /// </summary>
            public TimeSpan TotalProcessorTime { get; set; }

            /// <summary>
            /// Gets or sets the amount of CPU time in % that this program have used
            /// </summary>
            public decimal PctUsed { get; set; }

            /// <summary>
            /// Collect information about the cpu
            /// </summary>
            /// <param name="runningTime">How long the program have been running</param>
            /// <returns>cpu information</returns>
            public static CpuInfo CollectData(TimeSpan runningTime)
            {
                CpuInfo res = null;

                if (lastinfo == null)
                {
                    res = new CpuInfo()
                    {
                        TotalProcessorTime = Process.GetCurrentProcess().TotalProcessorTime,
                        totalProcessorTimeMS = Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds,
                        runningTimeMS = runningTime.TotalMilliseconds,
                        PctUsed = (decimal)Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds / (decimal)runningTime.TotalMilliseconds * (decimal)100
                    };
                }
                else
                {
                    res = new CpuInfo()
                    {
                        TotalProcessorTime = Process.GetCurrentProcess().TotalProcessorTime,
                        totalProcessorTimeMS = Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds,
                        runningTimeMS = runningTime.TotalMilliseconds,
                        PctUsed =
                        (
                            (decimal)Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds
                            - (decimal)lastinfo.totalProcessorTimeMS)
                        /
                        (
                            (decimal)runningTime.TotalMilliseconds
                            - (decimal)lastinfo.runningTimeMS)
                         * (decimal)100
                    };
                }

                lastinfo = res;
                return res;
            }
        }

        /// <summary>
        /// Infomation about the RAM installed on the machine
        /// </summary>
        public class RamInfo
        {
            /// <summary>
            /// Gets or sets PagedMemorySize
            /// </summary>
            public long PagedMemorySize { get; set; }

            /// <summary>
            /// Gets or sets PagedSystemMemorySize
            /// </summary>
            public long PagedSystemMemorySize { get; set; }

            /// <summary>
            /// Gets or sets PrivateMemorySize
            /// </summary>
            public long PrivateMemorySize { get; set; }

            /// <summary>
            /// Gets or sets PeakPagedMemorySize
            /// </summary>
            public long PeakPagedMemorySize { get; set; }

            /// <summary>
            /// Collect information about RAM usage
            /// </summary>
            /// <returns>ram information</returns>
            public static RamInfo CollectData()
            {
                RamInfo res = new RamInfo()
                {
                    PagedMemorySize = Process.GetCurrentProcess().PagedMemorySize64,
                    PagedSystemMemorySize = Process.GetCurrentProcess().PagedSystemMemorySize64,
                    PrivateMemorySize = Process.GetCurrentProcess().PrivateMemorySize64,
                    PeakPagedMemorySize = Process.GetCurrentProcess().PeakPagedMemorySize64
                };
                return res;
            }
        }
    }
}