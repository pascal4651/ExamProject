using System;

namespace Monosoft.Common.DTO
{
    public class apiMessage
    {
        public string Scope { get; set; }

        public string Route { get; set; }

        public string Messageid { get; set; }

        public string Json { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid UserContextToken { get; set; }

        public Tracing.Level Tracing { get; set; }
    }
}
