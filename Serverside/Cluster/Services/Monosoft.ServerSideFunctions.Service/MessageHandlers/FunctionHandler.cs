using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Monosoft.ServerSideFunctions.Service.MessageHandlers
{
    public enum Language { Sharp, JS};

    public class FunctionHandler
    {
        private readonly IEnumerable<string> defaultNamespaces;

        private readonly string runtimePath;

        private readonly IEnumerable<MetadataReference> defaultReferences;

        private readonly CSharpCompilationOptions options;


        public FunctionHandler()
        {
            defaultNamespaces = new[]
           {
                "System",
                "System.IO",
                "System.Net",
                "System.Linq",
                "System.Text",
                "System.Text.RegularExpressions",
                "System.Collections.Generic"
           };

            runtimePath = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6\{0}.dll";
            defaultReferences = new[]
            {
                MetadataReference.CreateFromFile(string.Format(runtimePath, "mscorlib")),
                MetadataReference.CreateFromFile(string.Format(runtimePath, "System")),
                MetadataReference.CreateFromFile(string.Format(runtimePath, "System.Core"))
            };

            options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithOverflowChecks(true).WithOptimizationLevel(OptimizationLevel.Release)
                    .WithUsings(defaultNamespaces);
        }
        
        /// <summary>
        /// Creates DLL files and JSON files for functions
        /// </summary>
        /// <param name="jsonObj">DTO.FunctionDefinitons, dataen til den JSON fil der skal laves</param>
        /// <returns>Svare tilbage om filen blev lavet eller ej</returns>
        public string CreateFunction(DTO.FunctionDefinitions jsonObj)
        {
            //Her laves der 2 strings some indeholder navnet og koden til den funktion der skal oprettes
            string functionName = jsonObj.Name;
            string functionString = jsonObj.FunctionData;
            //Her laver vi en string med vores pat for nem brug senere
            string path = Directory.GetCurrentDirectory() + @"\Dller\" + functionName + @"\";
            //Tjekker om mappen findes
            if(!Directory.Exists(path))
            {
                //Hvis mappen ikke findes laver vi den + dens backup mappe
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Dller\" + functionName);
                Directory.CreateDirectory(path + functionName + "BackUps");
                //her lavet vi en string som har navnet på den DLL fil der skal laves
                string fileName = functionName + ".dll";
                var pathToEmit = Path.Combine(path, fileName);
                //Her der tjekker vi om syntaxen er korrekt
                SyntaxTree syntaxTree = SyntaxFactory.ParseSyntaxTree(functionString);
                //Her bliver koden compilet
                var compilation = CSharpCompilation.Create(fileName, new SyntaxTree[] { syntaxTree }, defaultReferences, options);
                //result string bliver lavet med error fejlen så den er klar til at blive sendt hvis der er fejl
                string res = "ERROR WHILE CREATING FUNCTION '" + functionName + "'";
                try
                {
                    //Får fat i den resulteret fil
                    var result = compilation.Emit(pathToEmit);
                    //tjekker om hvis DLL filen blev lavet uden problemer
                    if (result.Success)
                    {
                        //JSON fil den funktionen bliver lavet
                        File.WriteAllText(path + functionName + ".json", JsonConvert.SerializeObject(jsonObj));
                        //Result string bliver fyldt ud med sucess resultat
                        res = "FUNCTION '" + functionName + "' WAS CREATED";
                    }
                    else
                    {
                        //Sletter mappen der er lavet hvis der er en error
                        DeleteFunction(functionName, Language.Sharp);
                        //Får sat error messagesne ind
                        IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                            diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                        foreach (Diagnostic diagnostic in failures)
                        {
                            //sender error messagesne 
                            Console.Error.WriteLine("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Hvis der opstår en fejl vi ikke kender til 
                    Console.WriteLine(ex);
                }
                //Retunere at der var et problem mens at filen blev lavet
                return res;
            }
            else
            {
                //Sender tilbage at funktionen allerede eksistere
                return "FUNCTION '" + functionName + "' ALREADY EXSISTS!";
            }
        }

        /// <summary>
        /// Runs the function called, if it exists
        /// </summary>
        /// <param name="functionName">String med navnet på funktionen</param>
        /// <param name="parameters">Object[] med alle de indtastede parametere</param>
        /// <returns>Svare tilbage med funktionens resultat, eller at filen ikke findes</returns>
        public object RunFunction(string functionName, object[] parameters)
        {
            string path, runFunctionName;
            //tjekker om funktionen der skal køre ikke er en backup
            if (!functionName.Contains("BackUp"))
            {
                //sætter variablen til den funktion som bliver kaldt
                runFunctionName = functionName;
                //Sætter pathen til mappen for at gøre det nemmere at køre filen
                path = Directory.GetCurrentDirectory() + @"\Dller\" + functionName + @"\";
            }
            else
            {
                //Mappen som filen ligger i bliver sat ind i en variable
                runFunctionName = functionName.Split("BackUp")[0];
                //Path til mappen bliver udfyldt for nemmere tilgang til fil
                path = Directory.GetCurrentDirectory() + @"\Dller\" + runFunctionName + @"\" + runFunctionName + @"BackUps\" + functionName + @"\";
            }

            string fileName = functionName + ".dll";
            //tjekker om mappen til filen eksistere
            if(Directory.Exists(path))
            {
                //Sætter den fulde path til filen
                string pathToRead = Path.Combine(path, fileName);

                //Resultat objectet bliver lavet
                object result = new object();
                try
                {
                    //Dll filens byte data bliver loaded ind
                    byte[] tempFileArray = File.ReadAllBytes(pathToRead);
                    //Her køre vi vores DLL igennem assembly og ligger den ind i en variable
                    var dll = Assembly.Load(tempFileArray);

                    //Her der køre vi igennem hver type som der er i en DLL
                    foreach (Type type in dll.GetExportedTypes())
                    {
                        //Her laver vi en dynamisk variable som indeholder en instans af den type programmet er kommet til
                        dynamic c = Activator.CreateInstance(type);
                        //Her der sætter vi resultatet til det som der bliver sendt tilbage når funktionen bliver kørt
                        result = type.InvokeMember(runFunctionName, BindingFlags.InvokeMethod, null, c, parameters);
                        //Skriver ud i server konsollen til debugging
                        Console.WriteLine(result);
                    }
                }
                catch (Exception ex)
                {
                    //Hvis der opstår en fejl skriver vi det ud til konsollen og sender den tilbage til brugeren
                    Console.WriteLine(ex);
                    return ex.ToString();
                }
                return result;
            }
            else
            {
                //Hvis funktions mappen ikke eksistere sender vi tilbage at den ikke findes
                return "FUNCTION '" + functionName + "' DOES NOT EXSIST!";
            }
        }

        /// <summary>
        /// Converts the param string into objects
        /// </summary>
        /// <param name="inputString">String med parametere resultat</param>
        /// <returns>Færdige paramete opsætnng</returns>
        public object[] ConvertToObjectArray(string inputString)
        {
            //hvis der ikke er blevet givet nogen parametere med så sendes der et tomt object array tilbage
            if (string.IsNullOrEmpty(inputString.Replace(" ", "")))
            {
                return new object[] { };
            }
            string[] tempArray = inputString.Split(",");
            string variables = string.Empty;
            string returnArray = string.Empty;

            for (int i = 0; i < tempArray.Length; i++)
            {
                if (!tempArray[i].Contains(":"))
                {
                    return null;
                }
                //Her laver vi et string array some der derefter splitter hvert 
                //parameter der er blevet send op i 2 bidder, variable type(0) og value(1)
                string[] item = tempArray[i].Split(":");
                variables += item[0] + " value" + i.ToString() + " = " + item[1] + ";";
                returnArray += i == 0 ? item[1] : ", " + item[1];
            }
            //String der bliver brugt til at lave DLL til at konvertere variablerne
            string paramFunctionString = "namespace ConvertParams { public class MyClass { public object[] GetArray() { " + variables + " return new object[] { " + returnArray + " }; } } }";

            return GetParametersArray(paramFunctionString);
        }

        /// <summary>
        /// Converts the param obejcts into usable data
        /// </summary>
        /// <param name="inputString">String til DLL</param>
        /// <returns>DLL resualtat</returns>
        public object[] GetParametersArray(string inputString)
        {
            //Gøre klar til at køre funktion, da den altid kommer til at hedde dette
            string functionName = "GetArray";
            string fileName = functionName + ".dll";
            //Resten her under er kommenteret i Create 
            SyntaxTree syntaxTree = SyntaxFactory.ParseSyntaxTree(inputString);

            var compilation = CSharpCompilation.Create(fileName, new SyntaxTree[] { syntaxTree }, defaultReferences, options);

            string res = "Failed";
            object[] outputArray = new object[] { };

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (result.Success)
                {
                    res = "Succeed";
                    ms.Seek(0, SeekOrigin.Begin);

                    var dll = Assembly.Load(ms.ToArray());
                    foreach (Type type in dll.GetExportedTypes())
                    {
                        dynamic c = Activator.CreateInstance(type);
                        outputArray = type.InvokeMember(functionName, BindingFlags.InvokeMethod, null, c, new object[] { });
                    }
                }
                else
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }
                    outputArray = null;
                }
            }
            Console.WriteLine(res);
            return outputArray;
        }

        /// <summary>
        /// Deletes selected funktion
        /// </summary>
        /// <param name="functionName">String med navnet på funktion</param>
        /// <returns>Svar om sletningen lykkedes eller ej</returns>
        public string DeleteFunction(string functionName, Language currentLanguage)
        {
            //Sætter pathen til nem tilgænglighed senere
            string path = currentLanguage == Language.Sharp ? Directory.GetCurrentDirectory() + @"\Dller\" + functionName : Directory.GetCurrentDirectory() + @"\DllerJS\" + functionName;
            //Tjekker om filen findes, hvis den gør bliver dens mappe og under mapper plus alle filer slettet.
            //Brugeren få derefter en besked tilbage om at det blev gjordt
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                return "FUNCTION '" + functionName + "' WAS DELETED!";
            }
            //Hvis filen ikke findes bliver der sendt tilbage at den ikke findes
            return "FUNCTION '" + functionName + "' DOES NOT EXIST!";
        }

        /// <summary>
        /// Updates a functon
        /// </summary>
        /// <param name="jsonobj">DTO.FunctionDefinitons, dataen til den JSON fil der skal laves</param>
        /// <returns>Svare tilbage om det lykkedes at opdatere eller ej</returns>
        public string UpdateFunction(DTO.FunctionDefinitions jsonobj)
        {
            //For næsten alt i denne metode tjek Create
            string functionName = jsonobj.Name;
            string functionString = jsonobj.FunctionData;
            string path = Directory.GetCurrentDirectory() + @"\Dller\" + functionName + @"\";

            if (File.Exists(path + functionName + ".dll"))
            {
                Directory.CreateDirectory(path + functionName + @"BackUps\" + functionName + "BackUp" +  (Directory.GetDirectories(path + functionName + @"BackUps").Count() + 1));
                string pathForBackUp = path + functionName + @"BackUps\" + functionName + "BackUp" + (Directory.GetDirectories(path + functionName + @"BackUps").Count()) + @"\" + functionName + "BackUp" + (Directory.GetDirectories(path + functionName + @"BackUps").Count());


                string fileName = functionName + "Holder.dll";
                var pathToEmit = Path.Combine(path, fileName);

                SyntaxTree syntaxTree = SyntaxFactory.ParseSyntaxTree(functionString);

                var compilation = CSharpCompilation.Create(fileName, new SyntaxTree[] { syntaxTree }, defaultReferences, options);

                string res = "ERROR WHILE UPDATING FUNCTION " + functionName;
                try
                {
                    var result = compilation.Emit(pathToEmit);
                    if (result.Success)
                    {
                        res = functionName + " was updated!";
                        //De nu gamle funktions filer bliver rykket og "holder" filen bliver om døbt til at blive den nye "hoved fil"
                        File.Move(path + functionName + ".dll", pathForBackUp + ".dll");
                        File.Move(path + functionName + ".json", pathForBackUp + ".json");
                        File.Move(path + functionName + "Holder.dll", path + functionName + ".dll");

                        File.WriteAllText(Directory.GetCurrentDirectory() + @"\Dller\" + functionName + @"\" + functionName + ".json", JsonConvert.SerializeObject(jsonobj));
                    }
                    else
                    {
                        Directory.Delete(pathForBackUp);
                        File.Delete(path + functionName + "Holder.dll");
                        IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                            diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                        foreach (Diagnostic diagnostic in failures)
                        {
                            Console.Error.WriteLine("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                return res;
            }
            else
            {
                return "FUNCTION '" + functionName + "' DOES NOT EXIST!";
            }
        }

        /// <summary>
        /// Reads the funktions code and returns it to the client
        /// </summary>
        /// <param name="functionName">String med navnet på funktionen</param>
        /// <returns>Sender tilbage filens kode eller om at filen ikke findes </returns>
        public string ReadFunction(string functionName, Language currentLanguage)
        {
            string path;
            string basePath = currentLanguage == Language.Sharp ? Directory.GetCurrentDirectory() + @"\Dller\" : Directory.GetCurrentDirectory() + @"\DllerJS\";
            if (!functionName.Contains("BackUp"))
            {
                path = basePath + functionName + @"\";
            }
            else
            {
                var folderName = functionName.Split("BackUp")[0];
                path = basePath + folderName + @"\" + folderName + @"BackUps\" + functionName + @"\";
            }

            if (File.Exists(path + functionName + ".json"))
            {
                DTO.FunctionDefinitions func = JsonConvert.DeserializeObject<DTO.FunctionDefinitions>(File.ReadAllText(path + functionName + ".json"));
                return func.FunctionData;
            }
            else
            {
                return "FUNCTION '" + functionName + "' DOES NOT EXIST!";
            }
        }

        /// <summary>
        /// Reads every file that exists and makes a list with the names
        /// </summary>
        /// <param name="functionName">String med funktions navn hvis man vil tjekke alle backups</param>
        /// <returns>Sender tilbage en liste over alle navnene på funktioner der findes. Eller Backups af en funktion</returns>
        public string ReadAllFunctions(string functionName, Language currentLanguage)
        {
            string path;
            if (string.IsNullOrEmpty(functionName))
            {
                path = currentLanguage == Language.Sharp ? Directory.GetCurrentDirectory() + @"\Dller" : Directory.GetCurrentDirectory() + @"\DllerJS";
            }
            else
            {
                if (functionName.Contains("BackUp"))
                {
                    return "FUNCTION '" + functionName + "' DOES NOT CONTAIN BACKUPS";
                }
                path = currentLanguage == Language.Sharp ? Directory.GetCurrentDirectory() + @"\Dller\" + functionName + @"\" + functionName + "BackUps" : Directory.GetCurrentDirectory() + @"\DllerJS\" + functionName + @"\" + functionName + "BackUps";
                if (!Directory.Exists(path))
                {
                    return "FUNCTION '" + functionName + "' DOES NOT EXIST!";
                }
            }
            var directories = Directory.GetDirectories(path);
            if (directories.Length == 0)
            {
                return "THERE ARE NO FUNCTIONS / BACKUPS";
            }
            string directoriesString = "";

            for (int i = 0; i < directories.Length; i++)
            {
                directoriesString += directories[i].Remove(0, path.Length).Replace(@"\", " ") + "\r\n";
            }
            return directoriesString;
        }

        // ****************************************JavaScript************************************************

        public string CreateFunctionJS(DTO.FunctionDefinitions funcDef)
        {
            //Her laves der 2 strings some indeholder navnet og koden til den funktion der skal oprettes
            string functionName = funcDef.Name;
            string functionString = funcDef.FunctionData;
            //Her laver vi en string med vores pat for nem brug senere
            string path = Directory.GetCurrentDirectory() + @"\DllerJS\" + functionName + @"\";
            //Tjekker om mappen findes
            if (!Directory.Exists(path))
            {
                //Hvis mappen ikke findes laver vi den + dens backup mappe
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\DllerJS\" + functionName);
                Directory.CreateDirectory(path + functionName + "BackUps");

                //result string bliver lavet med error fejlen så den er klar til at blive sendt hvis der er fejl
                string res = "ERROR WHILE CREATING FUNCTION '" + functionName + "'";
                try
                {
                    //JSON fil den funktionen bliver lavet
                    File.WriteAllText(path + functionName + ".json", JsonConvert.SerializeObject(funcDef));
                    //Result string bliver fyldt ud med sucess resultat
                    res = "FUNCTION '" + functionName + "' WAS CREATED";
                }
                catch (Exception ex)
                {
                    //Hvis der opstår en fejl vi ikke kender til 
                    Console.WriteLine(ex);
                }
                //Retunere at der var et problem mens at filen blev lavet
                return res;
            }
            else
            {
                //Sender tilbage at funktionen allerede eksistere
                return "FUNCTION '" + functionName + "' ALREADY EXSISTS!";
            }
        }

        public string UpdateFunctionJS(DTO.FunctionDefinitions funcDef)
        {
            //For næsten alt i denne metode tjek Create
            string functionName = funcDef.Name;
            string functionString = funcDef.FunctionData;
            string path = Directory.GetCurrentDirectory() + @"\DllerJS\" + functionName + @"\";

            if (File.Exists(path + functionName + ".json"))
            {
                Directory.CreateDirectory(path + functionName + @"BackUps\" + functionName + "BackUp" + (Directory.GetDirectories(path + functionName + @"BackUps").Count() + 1));
                string pathForBackUp = path + functionName + @"BackUps\" + functionName + "BackUp" + (Directory.GetDirectories(path + functionName + @"BackUps").Count()) + @"\" + functionName + "BackUp" + (Directory.GetDirectories(path + functionName + @"BackUps").Count());

                string res = "ERROR WHILE UPDATING FUNCTION " + functionName;
                try
                {
                    File.Move(path + functionName + ".json", pathForBackUp + ".json");
                    File.WriteAllText(Directory.GetCurrentDirectory() + @"\DllerJS\" + functionName + @"\" + functionName + ".json", JsonConvert.SerializeObject(funcDef));

                    res = "FUNCTION '" + functionName + "' WAS UPDATED!";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                return res;
            }
            else
            {
                return "FUNCTION '" + functionName + "' DOES NOT EXIST!";
            }
        }
    }
}
