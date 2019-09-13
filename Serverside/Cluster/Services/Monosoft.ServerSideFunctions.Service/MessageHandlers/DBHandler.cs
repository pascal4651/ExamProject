using MongoDB.Driver;
using Monosoft.ServerSideFunctions.Service.DTO;
using System.Collections.Generic;

namespace Monosoft.ServerSideFunctions.Service.MessageHandlers
{
    class DBHandler
    {
        private const string connectionString = "mongodb://localhost:27017";
        private MongoClient client;
        private IMongoDatabase database;
        private IMongoCollection<MongoFunctionDefinitions> collection;

        public DBHandler()
        {
            client = new MongoClient(connectionString);
            database = client.GetDatabase("monosoft");
            collection = database.GetCollection<MongoFunctionDefinitions>("functions_js");
        }

        public string CreateFunction(FunctionDefinitions funcDefinitions)
        {
            try
            {
                if (collection.Find(_ => _.Name == funcDefinitions.Name).Any())
                {
                    return "FUNCTION '" + funcDefinitions.Name + "' ALREADY EXSISTS!";
                }
                collection.InsertOne(new MongoFunctionDefinitions {
                    Id = funcDefinitions.Id,
                    Name = funcDefinitions.Name,
                    Function = funcDefinitions.FunctionData,
                    BackUps = new Dictionary<string, string>()
                });
                return "FUNCTION WAS CREATED";
            }
            catch (System.Exception ex)
            {
                return ex.ToString();
            }
        }

        public string ReadFunction(string functionName)
        {
            try
            {
                if (functionName.Contains("BackUp"))
                {
                    var rootFuncName = functionName.Split("BackUp")[0];
                    var currentBackUps = collection.Find(_ => _.Name == rootFuncName).FirstOrDefault().BackUps;
                    if (currentBackUps.TryGetValue(functionName, out string funcValue))
                    {
                        return funcValue;
                    }
                    return "FUNCTION '" + functionName + "' DOES NOT EXIST!";
                }
                else
                {
                    var mongoFuncDef = collection.Find(x => x.Name == functionName).FirstOrDefault();
                    if (mongoFuncDef == null)
                    {
                        return "FUNCTION '" + functionName + "' DOES NOT EXIST!";
                    }
                    return mongoFuncDef.Function;
                }
            }
            catch (System.Exception ex)
            {
                return ex.ToString();
            }
        }

        public string DeleteFunction(string functionName)
        {
            try
            {
                if (collection.Find(_ => _.Name == functionName).Any())
                {
                    collection.DeleteOne(_ => _.Name == functionName);
                    return "FUNCTION '" + functionName + "' WAS DELETED!";
                }
                return "FUNCTION '" + functionName + "' DOES NOT EXIST!";
            }
            catch (System.Exception ex)
            {
                return ex.ToString();
            }
        }

        public string UpdateFunction(FunctionDefinitions funcDefinitions)
        {
            try
            {
                if (collection.Find(_ => _.Name == funcDefinitions.Name).Any())
                {
                    // read current function from DB
                    var currentFunction = collection.Find(x => x.Name == funcDefinitions.Name).FirstOrDefault().Function;

                    // get current backups count
                    var rootFuncName = funcDefinitions.Name.Split("BackUp")[0];
                    var newNumber = collection.Find(_ => _.Name == rootFuncName).FirstOrDefault().BackUps.Count + 1;

                    // update by new function
                    var updateDef = Builders<MongoFunctionDefinitions>.Update.Set(_ => _.Function, funcDefinitions.FunctionData);
                    collection.UpdateOne(_ => _.Name == funcDefinitions.Name, updateDef);

                    // add new backup with previous function
                    updateDef = Builders<MongoFunctionDefinitions>.Update.Set("BackUps." + funcDefinitions.Name + "BackUp" + newNumber, currentFunction);
                    collection.UpdateOne(_ => _.Name == funcDefinitions.Name, updateDef);

                    return "FUNCTION '" + funcDefinitions.Name + "' WAS UPDATED!";
                }
                return "FUNCTION '" + funcDefinitions.Name + "' DOES NOT EXIST!";
            }
            catch (System.Exception ex)
            {
                return ex.ToString();
            }
        }

        public string ReadAllFunctions(string functionName)
        {
            try
            {
                string namesString = "";
                if (string.IsNullOrEmpty(functionName))
                {
                    var functions = collection.Find(_ => true).ToList();
                    if (functions.Count > 0)
                    {
                        foreach (var item in functions)
                        {
                            namesString += item.Name + "\r\n";
                        }
                        return namesString;
                    }
                    return "THERE ARE NO FUNCTIONS";
                }
                else if (functionName.Contains("BackUp"))
                {
                    return "THERE ARE NO BACKUPS";
                }
                else
                {
                    if (collection.Find(_ => _.Name == functionName).Any())
                    {
                        var backUpsKeys = collection.Find(_ => _.Name == functionName).FirstOrDefault().BackUps.Keys;
                        if (backUpsKeys.Count > 0)
                        {
                            foreach (var item in backUpsKeys)
                            {
                                namesString += item + "\r\n";
                            }
                            return namesString;
                        }
                        return "THERE ARE NO BACKUPS";
                    }
                    return "FUNCTION '" + functionName + "' DOES NOT EXIST!";
                }
            }
            catch (System.Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
