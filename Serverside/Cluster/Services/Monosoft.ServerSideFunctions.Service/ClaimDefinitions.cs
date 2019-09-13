using System.Collections.Generic;

namespace Monosoft.ServerSideFunctions.Service
{
    /// <summary>
    /// Contains the claims used by UserDB to handle security levels
    /// </summary>
    public static class ClaimDefinitions
    {
        /// <summary>
        /// Global list of claim definitions for this microservice
        /// </summary>
        internal static readonly Monosoft.Common.DTO.MetaDataDefinitions Definitions = new Monosoft.Common.DTO.MetaDataDefinitions()
        {
            Definitions = new List<Monosoft.Common.DTO.MetaDataDefinition>()
                                {
                                    ClaimDefinitions.IsServerSideFunctionsAdmin
                                }.ToArray()
        };

        /// <summary>
        /// The claim definition for system admins
        /// </summary>
        internal static readonly Monosoft.Common.DTO.MetaDataDefinition IsServerSideFunctionsAdmin = new Common.DTO.MetaDataDefinition()
        {
            Key = "ssServerSideFunctionsAdmin",
            Description = new Common.DTO.LocalizedString[]
            {
                new Common.DTO.LocalizedString()
                {
                    Lang = "en",
                    Text = "ServerSideFunctions administrator, i.e. remote setup of ServerSideFunctions, and thereby handling which programs are installed"
                }
            },
            Datacontext = "orgClaims", // must be: userClaims, orgClaims or userGroupClaims (defines the context they can be found in)
            Scope = GlobalValues.Scope
        };
    }
}
