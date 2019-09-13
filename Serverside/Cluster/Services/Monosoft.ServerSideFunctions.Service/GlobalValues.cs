
namespace Monosoft.ServerSideFunctions.Service
{
    /// <summary>
    /// Contains the global values for this program
    /// </summary>
    public class GlobalValues
    {
        /// <summary>
        /// Global value for the services name
        /// </summary>
        /// <returns>The full servicename, including its unique id</returns>
        public static string ServiceName()
        {
            //return Scope + "." + Monosoft.Common.Utils.MicroServiceConfig.ServiceId();
            return Scope;
        }

        /// <summary>
        /// Scope
        /// </summary>
        public static readonly string Scope = "Monosoft.ServerSideFuctions.Service";

        /// <summary>
        /// Route information for function created event
        /// </summary>
        public static readonly string RouteFunctionCreated = "function.create";

        /// <summary>
        /// Route information for function updated event
        /// </summary>
        public static readonly string RouteFunctionUpdated = "function.update";

        /// <summary>
        /// Route information for function deleted event
        /// </summary>
        public static readonly string RouteFunctionDeleted = "function.delete";

        /// <summary>
        /// Route information for function read event
        /// </summary>
        public static readonly string RouteFunctionRead = "function.read";

        /// <summary>
        /// Route information for function read event
        /// </summary>
        public static readonly string RouteFunctionReadAll = "function.readall";

        /// <summary>
        /// Route information for function read event
        /// </summary>
        public static readonly string RouteFunctionRun = "function.run";

        /// <summary>
        /// Route information for function * event
        /// </summary>
        /// <param name="operation">current operation</param>
        /// <returns></returns>
        public static string GetRouteFunction(string operation) => "function." + operation;
    }
}
