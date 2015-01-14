//taken from example by Dejan Geci: https://github.com/headsigned/csharp-scripting-example
//article: http://headsigned.com/article/csharp-scripting-example-using-csharpcodeprovider

namespace Visual_TCPRecon
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CSharp;
    using System.Windows.Forms;

    public static class Helper
    {

        public static Assembly CompileAssembly(string sourceFile)
        {
            string[] tmp = {sourceFile};
            return CompileAssembly(tmp, "");
        }

        public static Assembly CompileAssembly(string sourceFile, string outputPath)
        {
            string[] tmp = { sourceFile };
            return CompileAssembly(tmp, outputPath);
        }

        /// <summary>
        /// Compiles the list of C# source files into a dll.
        /// </summary>
        /// <param name="sourceFiles">List of files to compile.</param>
        /// <param name="outputAssemblyPath">Path for the new assembly.</param>
        /// <returns></returns>
        public static Assembly CompileAssembly(string[] sourceFiles, string outputPath)
        {
            var codeProvider = new CSharpCodeProvider();

            var compilerParameters = new CompilerParameters
            {
                GenerateExecutable = false,     // Make a DLL
                GenerateInMemory = (outputPath.Length == 0 ? false : true),       // Explicitly save it to path specified by compilerParameters.OutputAssembly
                IncludeDebugInformation = true, // Enable debugging - generate .pdb
                OutputAssembly = (outputPath.Length == 0 ? "" : outputPath)
            };

            // !! This is important: It adds the THIS project as a reference to the compiled dll to expose the public interfaces (as you would add it in the visual studio)
            compilerParameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
            compilerParameters.ReferencedAssemblies.Add("system.dll");
            compilerParameters.ReferencedAssemblies.Add("mscorlib.dll");
            compilerParameters.ReferencedAssemblies.Add("system.xml.dll");
            compilerParameters.ReferencedAssemblies.Add("system.data.dll");
            compilerParameters.ReferencedAssemblies.Add("system.web.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            //compilerParameters.ReferencedAssemblies.Add("DbLib.dll");

            var result = codeProvider.CompileAssemblyFromFile(compilerParameters, sourceFiles); // Compile

            if (result.Errors.HasErrors)
            {
                string text = "Compile error: ";
                foreach (CompilerError ce in result.Errors)
                {
                    text += "rn" + ce.ToString();
                }
                throw new Exception(text);
            }
                
            return result.CompiledAssembly;
        }

        /// <summary>
        /// Returns all types that implement the specified interface.
        /// </summary>
        /// <param name="assembly">Assembly to search.</param>
        /// <param name="interfaceType">Interface that types must implement.</param>
        /// <returns></returns>
        public static List<Type> GetTypesImplementingInterface(Assembly assembly, Type interfaceType)
        {
            if (!interfaceType.IsInterface) throw new ArgumentException("Not an interface.", "interfaceType");

            return assembly.GetTypes()
                           .Where(t => interfaceType.IsAssignableFrom(t))
                           .ToList();
        }
    }
}
