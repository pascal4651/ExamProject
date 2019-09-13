using Monosoft.Common.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Monosoft.ServerSideFunctions.Service.MessageHandlers
{
    /// <summary>
    /// Message handler for ServerSideFunctions messages
    /// </summary>
    class ServerSideFunctionsQueueHandler
    {
        /// <summary>
        /// The message handler logic for organisation messages
        /// </summary>
        /// <param name="topicparts">topic parts</param>
        /// <param name="wrapper">wrapper object</param>
        /// <returns>resulting object or null</returns>
        public static ReturnMessageWrapper HandleMessage(string[] topicparts, Common.DTO.MessageWrapper wrapper)
        {
            //CallContext cc = new CallContext(wrapper.OrgContext, new Common.DTO.Token() { Tokenid = wrapper.UserContextToken, Scope = GlobalValues.Scope }, wrapper.IssuedDate);
            var operation = topicparts[1];
            FunctionHandler functionHandler = new FunctionHandler();
            DBHandler dBHandler = new DBHandler();
            bool useMongoDBjs = true;

            if (true) // cc.IsServerSideFunctionsAdmin
            {
                if (wrapper.MessageData != null)
                {
                    switch (operation)
                    {
                        case "create":
                            var createFuncDef = Common.DTO.MessageWrapperHelper<DTO.FunctionDefinitions>.GetData(wrapper);

                            string createResult = functionHandler.CreateFunction(createFuncDef);
                            
                            return ResponseClient(wrapper, createResult, operation);

                        case "delete":
                            var deleteFuncDef = Common.DTO.MessageWrapperHelper<DTO.FunctionDefinitions>.GetData(wrapper);

                            string deleteResult = functionHandler.DeleteFunction(deleteFuncDef.Name, Language.Sharp);

                            return ResponseClient(wrapper, deleteResult, operation);

                        case "update":
                            var updateFuncDef = Common.DTO.MessageWrapperHelper<DTO.FunctionDefinitions>.GetData(wrapper);

                            string updateResult = functionHandler.UpdateFunction(updateFuncDef);

                            return ResponseClient(wrapper, updateResult, operation);

                        case "read":
                            var readFuncDef = Common.DTO.MessageWrapperHelper<DTO.FunctionDefinitions>.GetData(wrapper);

                            string readResult = functionHandler.ReadFunction(readFuncDef.Name, Language.Sharp);

                            return ResponseClient(wrapper, readResult, operation);

                        case "readall":
                            var readallFuncDef = Common.DTO.MessageWrapperHelper<DTO.FunctionDefinitions>.GetData(wrapper);

                            string readallResult = functionHandler.ReadAllFunctions(readallFuncDef.Name, Language.Sharp);

                            return ResponseClient(wrapper, readallResult, operation);
                            
                        case "run":
                            var runFuncDef = Common.DTO.MessageWrapperHelper<DTO.FunctionDefinitions>.GetData(wrapper);

                            object[] parameters = functionHandler.ConvertToObjectArray(runFuncDef.FunctionData);
                            if (parameters == null)
                            {
                                return ResponseClient(wrapper, "WRONG INPUT DATA", operation);
                            }
                            var runResult = functionHandler.RunFunction(runFuncDef.Name, parameters);
                            return ResponseClient(wrapper, runResult.ToString(), operation);

                        case "createjs":
                            var createFuncDefJS = Common.DTO.MessageWrapperHelper<DTO.FunctionDefinitions>.GetData(wrapper);
                            string createResultJS;
                            if (useMongoDBjs)
                            {
                                createResultJS = dBHandler.CreateFunction(createFuncDefJS);
                            }
                            else
                            {
                                createResultJS = functionHandler.CreateFunctionJS(createFuncDefJS);
                            }
                            return ResponseClient(wrapper, createResultJS, operation);

                        case "deletejs":
                            var deleteFuncDefJS = Common.DTO.MessageWrapperHelper<DTO.FunctionDefinitions>.GetData(wrapper);
                            string deleteResultJS;
                            if (useMongoDBjs)
                            {
                                deleteResultJS = dBHandler.DeleteFunction(deleteFuncDefJS.Name);
                            }
                            else
                            {
                                deleteResultJS = functionHandler.DeleteFunction(deleteFuncDefJS.Name, Language.JS);
                            }
                            return ResponseClient(wrapper, deleteResultJS, operation);

                        case "updatejs":
                            var updateFuncDefJS = Common.DTO.MessageWrapperHelper<DTO.FunctionDefinitions>.GetData(wrapper);
                            string updateResultJS;
                            if (useMongoDBjs)
                            {
                                updateResultJS = dBHandler.UpdateFunction(updateFuncDefJS);
                            }
                            else
                            {
                                updateResultJS = functionHandler.UpdateFunctionJS(updateFuncDefJS);
                            }
                            return ResponseClient(wrapper, updateResultJS, operation);

                        case "readjs":
                        case "runjs":
                            var readFuncDefJS = Common.DTO.MessageWrapperHelper<DTO.FunctionDefinitions>.GetData(wrapper);
                            string readResultJS;
                            if (useMongoDBjs)
                            {
                                readResultJS = dBHandler.ReadFunction(readFuncDefJS.Name);
                            }
                            else
                            {
                                readResultJS = functionHandler.ReadFunction(readFuncDefJS.Name, Language.JS);
                            }
                            return ResponseClient(wrapper, readResultJS, operation);

                        case "readalljs":
                            var readallFuncDefJS = Common.DTO.MessageWrapperHelper<DTO.FunctionDefinitions>.GetData(wrapper);
                            string readallResultJS;
                            if (useMongoDBjs)
                            {
                                readallResultJS = dBHandler.ReadAllFunctions(readallFuncDefJS.Name);
                            }
                            else
                            {
                                readallResultJS = functionHandler.ReadAllFunctions(readallFuncDefJS.Name, Language.JS);
                            }
                            return ResponseClient(wrapper, readallResultJS, operation);

                        default: /*log error event*/
                            Common.MessageQueue.Diagnostics.Instance.LogEvent("Unknow topic for ServerSideFunctions.", operation + " is unknown", Common.DTO.Severity.Information, System.Guid.Empty);
                            break;
                    }
                }
            }
            return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "unknown situation" } }, null);
        }

        private static ReturnMessageWrapper ResponseClient(MessageWrapper wrapper, string result, string operation)
        {
            var eventdata = new Common.DTO.EventDTO(result, wrapper.Clientid, wrapper.Messageid);
            Common.MessageQueue.EventClient.Instance.RaiseEvent(GlobalValues.GetRouteFunction(operation), eventdata);

            if (result != null)
            {
                return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, result);
            }
            else
            {
                return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "Missing rights" } }, result);
            }
        }
    }
}
