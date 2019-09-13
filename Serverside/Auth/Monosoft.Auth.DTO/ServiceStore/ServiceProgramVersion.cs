// <copyright file="Program.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.ServiceStore.DTO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Class that defines a specific version of a service (microservice)
    /// </summary>
    public class ServiceProgramVersion
    {
        /// <summary>
        /// Gets or sets the serviceId
        /// </summary>
        public Guid ServiceId { get; set; }

        /// <summary>
        /// Gets or sets the ReleaseNote
        /// </summary>
        public string ReleaseNote { get; set; }

        /// <summary>
        /// Gets or sets the Version
        /// </summary>
        public ProgamVersion Version { get; set; }

        /// <summary>
        /// Gets or sets the ZipFile
        /// </summary>
        public byte[] ZipFile { get; set; }

        public string Directory
        {
            get
            {
                return $"./services/{this.ServiceId}";
            }
        }

        public string FileName
        {
            get
            {
                return $"{this.Version.Version}.json";
            }
        }

        /// <summary>
        /// returns a list of local versions of this program, newest version first, oldest version last
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public static List<ProgamVersion> FindLocalVersions(Guid serviceId)
        {
            List<ProgamVersion> res = new List<ProgamVersion>();
            if (System.IO.Directory.Exists($"./services/{serviceId}"))
            {
                foreach (var versionDirectory in System.IO.Directory.GetFiles($"./services/{serviceId}", "*.json").ToList().OrderByDescending(p => p))
                {
                    System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(versionDirectory);
                    res.Add(new ProgamVersion(di.Name));
                }
            }

            return res;
        }

        public static void Delete(Guid serviceId)
        {
            System.IO.Directory.Delete($"/{serviceId}", true);
        }



        /// <summary>
        /// Save the definition to disk
        /// </summary>
        public string SaveToDisk()
        {
            var asJson = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            if (System.IO.Directory.Exists(this.Directory) == false)
            {
                System.IO.Directory.CreateDirectory(this.Directory);
            }
            string fullFileName = this.GetFullFilename();
            if (System.IO.File.Exists(fullFileName) == false)
            {
                System.IO.File.WriteAllText(fullFileName, asJson);
            }
            return this.Directory;
        }

        /// <summary>
        /// install to disk
        /// </summary>
        public void InstallToDisk()
        {
            string installDir = this.Directory + "/" + this.Version.Version;
            if (System.IO.Directory.Exists(installDir) == false)
            {
                System.IO.Directory.CreateDirectory(installDir);
            }

            System.IO.File.WriteAllBytes(this.Directory + "/package.zip", this.ZipFile);
            System.IO.Compression.ZipFile.ExtractToDirectory(this.Directory + "/package.zip", installDir);
        }

        public string GetFullFilename()
        {
            return this.Directory + "/" + this.FileName;
        }

        /// <summary>
        /// Loads the definition from disk
        /// </summary>
        public static ServiceProgramVersion LoadFromDisk(Guid serviceId, string versionNo)
        {
            var fullFileName = $"./services/{serviceId}/{versionNo}.json";

            if (System.IO.File.Exists(fullFileName))
            {
                var asJson = System.IO.File.ReadAllText(fullFileName);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<ServiceProgramVersion>(asJson);
            }
            else
            {
                return null;
            }
        }

    }
}
