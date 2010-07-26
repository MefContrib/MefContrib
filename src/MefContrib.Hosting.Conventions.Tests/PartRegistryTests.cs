namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using MefContrib.Hosting.Conventions.Configuration;
    using NUnit.Framework;

    [TestFixture]
    public class PartRegistryTests
    {
        [Test]
        public void ContractService_should_be_instance_of_defaultconventioncontractservice_on_new_instance()
        {
            var registry =
                new PartRegistry();

            registry.ContractService.ShouldBeOfType<DefaultConventionContractService>();
        }

        [Test]
        public void TypeScanner_should_be_null_on_new_instance()
        {
            var registry =
                new PartRegistry();

            registry.TypeScanner.ShouldBeNull();
        }

        [Test]
        public void Part_should_return_instance_of_partconventionbuilder_for_partconvention_type()
        {
            var registry =
                new PartRegistry();

            var result =
                registry.Part();

            result.ShouldBeOfType<PartConventionBuilder<PartConvention>>();
        }

        [Test]
        public void Part_of_tconvention_should_return_instance_of_partconventionbuilder_for_tconvention_type()
        {
            var registry =
                new PartRegistry();

            var result =
                registry.Part<PartConvention>();

            result.ShouldBeOfType<PartConventionBuilder<PartConvention>>();
        }

        [Test]
        public void Scan_should_throw_argumentnullexception_when_called_with_null()
        {
            var registry =
                new PartRegistry();

            var exception =
                Catch.Exception(() => registry.Scan(null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }
    }

    public class AssemblyFactory : IDisposable
    {
        public AssemblyFactory()
        {
            this.Assemblies = new List<AssemblyInfo>();
            this.AssemblyDirectory = CreateTemporaryDirectory();
        }

        public AssemblyInfo Build(string code)
        {
            var builtAssembly =
                CSharpAssemblyFactory.Compile(code, this.GenerateTemporaryAssemblyName());

            this.Assemblies.Add(new AssemblyInfo(builtAssembly));

            return this.Assemblies.Last();
        }

        public IList<AssemblyInfo> Assemblies { get; private set; }

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

    public class AssemblyInfo : IDisposable
    {
        public AssemblyInfo(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly", "The assembly cannot be null");
            }

            this.Assembly = assembly;
        }

        public Assembly Assembly { get; private set; }

        public void Dispose()
        {
            try
            {
                File.Delete(this.Assembly.FullName);
            }
            catch
            {
            }
            
        }

        public static implicit operator Assembly(AssemblyInfo info)
        {
            return info.Assembly;
        }

        public static implicit operator string(AssemblyInfo info)
        {
            return info.Assembly.Location;
        }
    }
}