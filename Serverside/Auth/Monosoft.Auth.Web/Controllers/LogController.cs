using Microsoft.AspNetCore.Mvc;
using Monosoft.Web.AuthApi.Hubs;

namespace Monosoft.Auth.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        public string Index()
        {
            var res = string.Join("\r\n", authApiHub.memoryLog.ToArray());
            authApiHub.memoryLog.Clear();
            return res;
        }
    }
}