namespace MefContrib.Hosting.Conventions.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using MefContrib.Tests;
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using MefContrib.Hosting.Conventions.Configuration;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class AppDomainPartRegistryLocatorTests
    {
        private Mock<_AppDomain> fakeAppDomain;
        private AppDomainPartRegistryLocator registryLocator;

        [SetUp]
        public void Setup()
        {
            
        }

        private Mock<_Assembly> CreateFakeAssembly()
        {


            throw new NotImplementedException();
        }

        private IPartRegistry<IContractService> CreateFakeRegistry()
        {
            return (new Mock<IPartRegistry<IContractService>>()).Object;
        }

        [Test]
        public void Should_scan_appdomain_for_all_objects_that_implements_ipartregistry()
        {
            throw new System.NotImplementedException();
        }

        [Test]
        public void Should_only_locate_public_registries()
        {
            this.fakeAppDomain = new Mock<_AppDomain>();
            this.fakeAppDomain.Setup(x => x.GetAssemblies()).Returns((Func<Assembly[]>)null);

            this.registryLocator = new AppDomainPartRegistryLocator((AppDomain)this.fakeAppDomain.Object);

            int a = 10;
        }

        [Test]
        public void Dynamic_assemblies_should_be_added_to_application_domain()
        {
            //CREATE THE ASSEMBLY IN A DIFFERENT DOMAIN
            //http://stackoverflow.com/questions/1799373/how-can-i-prevent-compileassemblyfromsource-from-leaking-memory

            //var applicationDomain =
            //    AppDomain.CreateDomain("Testing");

            //applicationDomain.

            var assembly =
                CSharpAssemblyFactory.Compile(@"
                    public class Quack
                    {
                    }
                ");

            var bar = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes().Where(y => y.IsPublic)
                .Where(z => z.Name.Equals("Quack")));

            var dynamicAssemblyCount = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(x => x.IsDynamic)
                .Select(x => x);

            dynamicAssemblyCount.Count().ShouldEqual(1);
        }

        [Test]
        public void Should_locate_types_implementing_ipartregistry()
        {
            var results = AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(x => x.GetTypes())
                .SelectMany(GetPublicPartRegistryTypes);

            results.Count().ShouldBeGreaterThan(0);
        }

        [Test]
        public void Should_create_partregistry_instance_from_type()
        {
            var instance =
                CreateNewRegistryInstance(typeof(FakeConventionRegistry));

            instance.ShouldNotBeNull();
        }

        [Test]
        public void Should_only_create_instance_of_partregistry_type_that_has_default_ctor()
        {
            var exception =
                Catch.Exception(() => CreateNewRegistryInstance(typeof(PartRegistryWithNoDefaultCtor)));

            exception.ShouldBeNull();
        }

        [Test]
        public void Should_return_partregistry_instances_from_domain()
        {
            var locator =
                new AppDomainPartRegistryLocator(AppDomain.CurrentDomain);

            var instances =
                locator.Locate();

            instances.Count().ShouldBeGreaterThan(0);
        }

        private static IPartRegistry<IContractService> CreateNewRegistryInstance(Type registryType)
        {
            if (!registryType.HasDefaultConstructor())
            {
                return null;
            }

            var instance =
                Activator.CreateInstance(registryType);

            return (IPartRegistry<IContractService>)instance;
        }

        private static IEnumerable<Type> GetPublicPartRegistryTypes(IEnumerable<Type> types)
        {
            return types
                .Where(x => x.IsPublic)
                .Where(TypeImplements<IPartRegistry<IContractService>>);
        }

        private static bool TypeImplements<T>(Type type)
        {
            return type.GetInterfaces()
                .Where(x => x.IsGenericType)
                .Any(x => x.GetGenericTypeDefinition().Equals(typeof(IPartRegistry<>)));
        }

        private class PartRegistryWithNoDefaultCtor : PartRegistry
        {
            public PartRegistryWithNoDefaultCtor(int value)
            {
            }
        }
    }
}