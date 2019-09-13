namespace Monosoft.ZooKeeper.Service.MessageHandlers
{
    using Monosoft.Common.DTO;
    using System.Linq;

    /// <summary>
    /// Message handler for zookeeper messages
    /// </summary>
    internal class ZookeeperQueueHandler
    {
        /// <summary>
        /// The message handler logic for organisation messages
        /// </summary>
        /// <param name="topicparts">topic parts</param>
        /// <param name="wrapper">wrapper object</param>
        /// <returns>resulting object or null</returns>
        public static ReturnMessageWrapper HandleMessage(string[] topicparts, Common.DTO.MessageWrapper wrapper)
        {
            CallContext cc = new CallContext(
                wrapper.OrgContext,
                new Common.DTO.Token() { Tokenid = wrapper.UserContextToken, Scope = GlobalValues.Scope },
                wrapper.IssuedDate);
            var operation = topicparts[1];

            if (cc.IsZooKeeperAdmin)
            {
                if (wrapper.MessageData != null)
                {
                    switch (operation)
                    {
                        case "set":
                            var definition = Common.DTO.MessageWrapperHelper<ZooKeeperDefinition>.GetData(wrapper);
                            definition.SaveToDisk();
                            break;
                        case "get":
                            var result = ZooKeeperDefinition.LoadFromDisk();
                            Common.MessageQueue.EventClient.Instance.RaiseEvent("zookeeper.get", new Common.DTO.EventDTO(result, wrapper.Clientid, wrapper.Messageid));
                            break;
                        default: /*log error event*/
                            Common.MessageQueue.Diagnostics.Instance.LogEvent("Unknow topic for ZooKeeper.", operation + " is unknown", Common.DTO.Severity.Information, System.Guid.Empty );
                            break;
                    }
                }
            }

            return null;
        }
    }
}
