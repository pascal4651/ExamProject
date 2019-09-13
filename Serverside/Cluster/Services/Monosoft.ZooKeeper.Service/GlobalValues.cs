namespace Monosoft.ZooKeeper.Service
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
            return Scope + "." + Monosoft.Common.Utils.MicroServiceConfig.ServiceId();
        }

        /// <summary>
        /// Scope
        /// </summary>
        public static readonly string Scope = "Monosoft.ZooKeeper.Service";
    }
}