using System;
using Xunit;
using Monosoft.ServerSideFunctions.Service.DTO;
using Monosoft.ServerSideFunctions.Service.MessageHandlers;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace FunctionHandlerTester
{

    public class UnitTest1
    {
        FunctionHandler funcH = new FunctionHandler();
        #region CreateFuncTest
        [Fact]
        public void CreateFuncTest()
        {
            FunctionDefinitions funcDef = new FunctionDefinitions();
            funcDef.Id = "1";
            funcDef.Name = "ForTesting";
            funcDef.FunctionData = "";

            var actualResult = funcH.CreateFunction(funcDef);
            var expectedResult = "FUNCTION '" + funcDef.Name + "' WAS CREATED";

            Assert.Equal(expectedResult, actualResult);
        }
        [Fact]
        public void CreateFuncTestFail()
        {
            FunctionDefinitions funcDef = new FunctionDefinitions();
            funcDef.Id = "1";
            funcDef.Name = "Blergh";
            funcDef.FunctionData = "iwhegiwheg";

            var actualResult = funcH.CreateFunction(funcDef);
            var expectedResult = "ERROR WHILE CREATING FUNCTION '" + funcDef.Name + "'";

            Assert.Equal(expectedResult, actualResult);

        }
        [Fact]
        public void CreateFuncTestAlreadyExsists()
        {
            FunctionDefinitions funcDef = new FunctionDefinitions();
            funcDef.Id = "1";
            funcDef.Name = "ForTesting";
            funcDef.FunctionData = "";

            var actualResult = funcH.CreateFunction(funcDef);
            var expectedResult = "FUNCTION '" + funcDef.Name + "' ALREADY EXSISTS!";

            Assert.Equal(expectedResult, actualResult);
        }


        #endregion
        #region RunFuncTest
        [Fact]
        public void RunFuncTest()
        {
            var actualResult = funcH.RunFunction("Yepser", null);
            int expectedResult = 6;

            Assert.Equal(expectedResult, actualResult);
        }
        [Fact]
        public void RunFuncBackupTest()
        {
            var actualResult = funcH.RunFunction("YepserBackUp1", null);
            int expectedResult = 6;

            Assert.Equal(expectedResult, actualResult);
        }
        [Fact]
        public void RunFuncTestFail()
        {
            var actualResult = funcH.RunFunction("wurg", null);
            string expectedResult = "FUNCTION 'wurg' DOES NOT EXSIST!";

            Assert.Equal(expectedResult, actualResult);

        }




        #endregion
        #region DeleteFuncTests
        [Fact]
        public void DeleteFuncTest()
        {
            var actualResult = funcH.DeleteFunction("ForTesting");
            string expectedResult = "FUNCTION 'ForTesting' WAS DELETED!";

            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void DeleteFuncTestFail()
        {
            var actualResult = funcH.DeleteFunction("gwegweg");
            string expectedResult = "FUNCTION 'gwegweg' DOES NOT EXIST!";

            Assert.Equal(expectedResult, actualResult);
        }
        #endregion
        #region ParameterConvert
        [Fact]
        public void ParameterConvertTestInt()
        {
            var actualResult = funcH.ConvertToObjectArray("int: 1");
            var expectedResult = 1;

            Assert.Equal(expectedResult, actualResult[0]);
        }
        [Fact]
        public void ParameterConvertTestString()
        {

            var actualResult = funcH.ConvertToObjectArray($"string: \"hej\" ");
            var expectedResult = "hej";

            Assert.Equal(expectedResult, actualResult[0]);
        }
        [Fact]
        public void ParameterConvertTestStringAndInt()
        {
            var actualResult = funcH.ConvertToObjectArray("int: 1,string: \"hej\" ");
            var expectedResult1 = 1;
            var expectedResult2 = "hej";


            Assert.Equal(expectedResult1, actualResult[0]);
            Assert.Equal(expectedResult2, actualResult[1]);
        }
        [Fact]
        public void ParamaterConvertTestFail()
        {
            var actualResult = funcH.ConvertToObjectArray("srh" +
                "");
            var expectedResult = 0;

            Assert.Equal(expectedResult, actualResult.Length);
        }


        #endregion
        #region UpdateFuncTest
        [Fact]
        public void UpdateFuncTesting()
        {
            FunctionDefinitions funcDef = new FunctionDefinitions();
            funcDef.Id = "1";
            funcDef.Name = "ForTesting";
            funcDef.FunctionData = "";

            var actualResult = funcH.UpdateFunction(funcDef);
            var expectedResult = funcDef.Name + " was updated!";

            Assert.Equal(expectedResult, actualResult);
        }
        [Fact]
        public void UpdateFuncTestingFail()
        {
            FunctionDefinitions funcDef = new FunctionDefinitions();
            funcDef.Id = "1";
            funcDef.Name = "ForTesting";
            funcDef.FunctionData = "egrs";

            var actualResult = funcH.UpdateFunction(funcDef);
            var expectedResult = "ERROR WHILE UPDATING FUNCTION " + funcDef.Name;

            Assert.Equal(expectedResult, actualResult);
        }
        [Fact]
        public void UpdateFuncTestingDoesNotExist()
        {

            FunctionDefinitions funcDef = new FunctionDefinitions();
            funcDef.Id = "1";
            funcDef.Name = "3rgwet";
            funcDef.FunctionData = "egrs";

            var actualResult = funcH.UpdateFunction(funcDef);
            var expectedResult = "FUNCTION '" + funcDef.Name + "' DOES NOT EXIST!";

            Assert.Equal(expectedResult, actualResult);
        }

        #endregion
        #region ReadFuncTest
        [Fact]
        public void ReadFuncTest()
        {
            var actualResult = funcH.ReadFunction("ForTesting");
            var expectedResult = "";

            Assert.Equal(expectedResult, actualResult);
        }
        [Fact]
        public void ReadFuncTestFail()
        {
            var actualResult = funcH.ReadFunction("wegweg");
            var expectedResult = "FUNCTION 'wegweg' DOES NOT EXIST!";

            Assert.Equal(expectedResult, actualResult);
        }
        [Fact]
        public void ReadFuncTestBackup()
        {
            var actualResult = funcH.ReadFunction("ForTestingBackUp1");
            var expectedResult = "";

            Assert.Equal(expectedResult, actualResult);
        }
        [Fact]
        public void ReadFuncTestBackupFail()
        {
            var actualResult = funcH.ReadFunction("wegwegBackUp");
            var expectedResult = "FUNCTION 'wegwegBackUp' DOES NOT EXIST!";

            Assert.Equal(expectedResult, actualResult);
        }
        #endregion
        #region ReadAllFuncTest

        [Fact]
        public void ReadAllFuncTest()
        {
            var acctualResult = funcH.ReadAllFunctions("");
            Assert.NotEmpty(acctualResult);
        }
        [Fact]
        public void ReadAllFuncBackupTest()
        {
            var actualResult = funcH.ReadAllFunctions("ForTesting");
            var expectedResult = "ForTestingBackUp1";

            Assert.Contains(expectedResult, actualResult);
        }
        [Fact]
        public void ReadAllFuncFail()
        {
            var actualResult = funcH.ReadAllFunctions("wegwg");
            var expectedResult = "FUNCTION 'wegwg' DOES NOT EXIST!";

            Assert.Equal(expectedResult, actualResult);

        }
        [Fact]
        public void ReadAllFuncBackupFail()
        {
            var actualResult = funcH.ReadAllFunctions("ForTestingBackUp");
            var expectedResult = "FUNCTION 'ForTestingBackUp' DOES NOT CONTAIN BACKUPS";

            Assert.Equal(expectedResult, actualResult);
        }

        #endregion
    }
}
