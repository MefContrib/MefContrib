namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class AssemblyFactory : IDisposable
    {
        public AssemblyFactory()
        {
            this.Assemblies = new List<AssemblyWrapper>();
            this.AssemblyDirectory = CreateTemporaryDirectory();
        }

        public AssemblyWrapper Build(string code)
        {
            var builtAssembly =
                CSharpAssemblyFactory.Compile(code, this.GenerateTemporaryAssemblyName());

            this.Assemblies.Add(new AssemblyWrapper(builtAssembly));

            return this.Assemblies.Last();
        }

        public IList<AssemblyWrapper> Assemblies { get; private set; }

        public DirectoryInfo AssemblyDirectory { get; set; }

        private static DirectoryInfo CreateTemporaryDirectory()
        {
            var tempDirectoryPath =
                Path.Combine(Path.GetTempPath(),
                    Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

            return Directory.CreateDirectory(tempDirectoryPath);
        }

        private string GenerateTemporaryAssemblyName()
        {
            var assemblyName =
                Path.Combine(this.AssemblyDirectory.FullName,
                    string.Concat(Path.GetFileNameWithoutExtension(Path.GetRandomFileName()), ".dll"));

            return assemblyName;
        }

        public void Dispose()
        {
            foreach (var assemblyInfo in this.Assemblies)
            {
                assemblyInfo.Dispose();
            }

            try
            {
                Directory.Delete(this.AssemblyDirectory.FullName, true);
            }
            catch
            {
            }
        }
    }
}