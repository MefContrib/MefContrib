namespace MefContrib.Hosting.Conventions.Tests
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.CSharp;

    /// <summary>
    /// Compiles CSharp code into an in-memory assembly.
    /// </summary>
    public static class CSharpAssemblyFactory
    {
        /// <summary>
        /// Compiles the specified source code into an in-memory assembly.
        /// </summary>
        /// <param name="code">The source code that should be compiled into the assembly.</param>
        /// <param name="references">Assemblies that should be referenced by the created assembly.</param>
        /// <returns>An in-memory <see cref="Assembly"/> instance containing the compiled <paramref name="code"/>.</returns>
        public static Assembly Compile(string code, params string[] references)
        {
            var parameters =
                new CompilerParameters
                {
                    GenerateExecutable = false,
                    GenerateInMemory = true
                };

            if (references != null)
            {
                foreach (var reference in references)
                {
                    parameters.ReferencedAssemblies.Add(reference);
                }
            }

            var provider =
                new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v3.5" } });

            var results =
                provider.CompileAssemblyFromSource(parameters, code);

            return results.CompiledAssembly;
        }

        public static Assembly Compile(string code, string path, params string[] references)
        {
            var parameters =
                new CompilerParameters
                {
                    GenerateExecutable = false,
                    GenerateInMemory = string.IsNullOrEmpty(path),
                    OutputAssembly = path ?? "fake.dll"
                };

            if (references != null)
            {
                foreach (var reference in references)
                {
                    parameters.ReferencedAssemblies.Add(reference);
                }
            }

            var provider =
                new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v4.0" } });

            var results =
                provider.CompileAssemblyFromSource(parameters, code);
            
            return results.CompiledAssembly;
        }
    }
}