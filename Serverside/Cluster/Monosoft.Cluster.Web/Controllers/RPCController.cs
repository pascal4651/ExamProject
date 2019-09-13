using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Monosoft.Common.DTO;
using Monosoft.Common.MessageQueue;

namespace Monosoft.Cluster.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RPCController : ControllerBase
    {
        [HttpPost]
        [RequestSizeLimit(524288000)]
        public byte[] Post([FromBody]apiMessage message)
        {
            string ip = HttpContext.Connection.RemoteIpAddress.ToString();
            string clientid = string.Empty;
            MessageWrapper mw = new MessageWrapper(
                        DateTime.Now,
                        message.UserContextToken,
                        message.Scope,
                        clientid,
                        message.Messageid,
                        ip,
                        message.Json,
                        message.OrganisationId,
                        new Tracing() { Tracelevel = message.Tracing });

            return RequestClient.Instance.Rpc(message.Route, Monosoft.Common.Utils.MessageDataHelper.ToMessageData(mw));
        }
    }
}