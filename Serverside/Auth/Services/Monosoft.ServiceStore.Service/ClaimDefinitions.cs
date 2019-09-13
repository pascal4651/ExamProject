
namespace Monosoft.ServiceStore.Service
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains the claims used by UserDB to handle security levels
    /// </summary>
    public static class ClaimDefinitions
    {
        /// <summary>
        /// Definitions of the claims used for controlling security in this service
        /// </summary>
        public static readonly Monosoft.Common.DTO.MetaDataDefinitions Definitions = new Monosoft.Common.DTO.MetaDataDefinitions()
        {
            Definitions = new List<Monosoft.Common.DTO.MetaDataDefinition>()
                                {
                                    ClaimDefinitions.ServiceStoreAdmin
                                }.ToArray()
        };

        /// <summary>
        /// The admin claim, which gives the user the rights for maintaining security
        /// </summary>
        internal static readonly Monosoft.Common.DTO.MetaDataDefinition ServiceStoreAdmin = new Common.DTO.MetaDataDefinition()
        {
            Key = "isServiceStoreAdmin",
            Description = new Common.DTO.LocalizedString[]
            {
                new Common.DTO.LocalizedString()
                {
                    Lang = "en",
                    Text = "ServiceStore administrator, i.e. can define new microservices, and upload new versions"
                }
            },
            Datacontext = "orgClaims", // must be: userClaims, orgClaims or userGroupClaims (defines the context they can be found in)
            Scope = GlobalValues.ClusterName
        };
    }

    public class GlobalValues
    {
        /// <summary>
        /// Name of the service
        /// </summary>
        public static readonly string ClusterName = "Monosoft.Service.AUTH";
    }
}
