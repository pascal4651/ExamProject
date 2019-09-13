// <copyright file="Contract.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Monosoft.ServiceStore.DTO
{
    public class ServicesInformation
    {
        public ServiceInformation[] Services { get; set; }

        public static ServicesInformation ServiceInformation()
        {
            List<ServiceInformation> servicesinfo = new List<ServiceInformation>();
            var services = Monosoft.ServiceStore.DTO.Service.FindServices(); 
            foreach (var service in services)
            {
                var versions = ServiceProgramVersion.FindLocalVersions(service.Id);

                ServiceInformation si = new ServiceInformation();
                si.Service = service;
                si.Versions = versions.ToArray();
                servicesinfo.Add(si);
            }

            ServicesInformation res = new ServicesInformation();
            res.Services = servicesinfo.ToArray();
            return res;
        }
    }

    public class ServiceInformation
    {
        public Service Service { get; set; }
        public ProgamVersion[] Versions { get; set; }
    }


    /// <summary>
    /// Service definition
    /// </summary>
    public class Service
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets Name
        /// </summary>
        public string Name { get; set; }



        public void Delete()
        {
            var fullFileName = $"./services/{this.Id}.json";
            ServiceProgramVersion.Delete(this.Id);

            if (System.IO.File.Exists(fullFileName))
            {
                System.IO.File.Delete(fullFileName);
            }
        }

        /// <summary>
        /// Save the definition to disk
        /// </summary>
        public void SaveToDisk(bool overwrite)
        {
            if (System.IO.Directory.Exists("./services/") == false)
            {
                System.IO.Directory.CreateDirectory("./services/");
            }
            var asJson = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            var fullFileName = $"./services/{this.Id}.json";
            if (System.IO.File.Exists(fullFileName) == false || overwrite)
            {
                System.IO.File.WriteAllText(fullFileName, asJson);
            }
        }

        /// <summary>
        /// Loads the definition from disk
        /// </summary>
        public static Service LoadFromDisk(Guid serviceId)
        {
            var fullFileName = $"./services/{serviceId}.json";

            if (System.IO.File.Exists(fullFileName))
            {
                var asJson = System.IO.File.ReadAllText(fullFileName);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Service>(asJson);
            }
            else
            {
                return null;
            }
        }

        public static List<Service> FindServices()
        {
            List<Service> res = new List<Service>();
            foreach (var file in System.IO.Directory.GetFiles($"./services/", "*.json"))
            {
                var asJson = System.IO.File.ReadAllText(file);
                res.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<Service>(asJson));
            }

            return res;
        }
    }
}
