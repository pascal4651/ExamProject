using System;
using Microsoft.AspNetCore.Mvc;
using Monosoft.Common.DTO;
using Monosoft.Common.MessageQueue;

namespace Monosoft.Auth.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FAFController : ControllerBase
    {
        [HttpPost]
        [RequestSizeLimit(524288000)]
        public void Post([FromBody]apiMessage message)
        {
            string ip = HttpContext.Connection.RemoteIpAddress.ToString();
            string clientid = string.Empty;

            MessageWrapper mw = new Monosoft.Common.DTO.MessageWrapper(
                        DateTime.Now,
                        message.UserContextToken,
                        message.Scope,
                        clientid,
                        message.Messageid,
                        ip,
                        message.Json,
                        message.OrganisationId,
                        new Tracing() { Tracelevel = message.Tracing });
            RequestClient.Instance.FAF(message.Route, System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(mw)));
        }
    }
}