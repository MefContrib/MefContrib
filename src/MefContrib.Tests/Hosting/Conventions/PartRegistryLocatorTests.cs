namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.Linq;
    using MefContrib.Hosting.Conventions.Configuration;
    using MefContrib.Tests;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class PartRegistryLocatorTests
    {
        [Test]
        public void Should_throw_argumentnullexception_when_instansiated_with_null()
        {
            // Arrange, Act
            var exception =
                Catch.Exception(() => new PartRegistryLocator(null));

            // Assert
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void Should_return_instances_of_registry_types_returned_by_typescanners()
        {
            // Arrange
            var knownRegistry =
                CreateMockRegistryOfType<FakePartRegistryWithDefinedTypeScanner>()
                    .WithTypeScannerContaining(typeof(FakePartRegistry));

            var locator =
                CreatePartRegistryLocatorWithMockRegistries(knownRegistry);

            // Act
            var result = locator.GetRegistries();

            // Assert
            result.ShouldContainType<FakePartRegistry>();
        }

        [Test]
        public void Should_only_return_instances_of_registry_types_returned_by_typescanners_that_are_public()
        {
            // Arrange
            var knownRegistry =
                CreateMockRegistryOfType<FakePartRegistryWithDefinedTypeScanner>()
                    .WithTypeScannerContaining(
                        typeof(FakePartRegistry),
                        typeof(FakeNonPublicPartRegistry));

            var locator =
                CreatePartRegistryLocatorWithMockRegistries(knownRegistry);

            // Act
            var result = locator.GetRegistries();

            // Assert
            result.Count().ShouldEqual(2);
        }

        [Test]
        public void Should_only_return_instances_of_registry_types_returned_by_typescanners_that_have_public_ctor()
        {
            // Arrange
            var knownRegistry =
                CreateMockRegistryOfType<FakePartRegistryWithDefinedTypeScanner>()
                    .WithTypeScannerContaining(
                        typeof(FakePartRegistry),
                        typeof(FakePartRegistryWithNoDefaultCtor));

            var locator =
                CreatePartRegistryLocatorWithMockRegistries(knownRegistry);

            // Act
            var result = locator.GetRegistries();

            // Assert
            result.Count().ShouldEqual(2);
        }

        [Test]
        public void Should_ignore_known_registries_that_has_no_typescanner_defined()
        {
            // Arrange
            var knownRegistry = new Mock<IPartRegistry<IContractService>>();
            knownRegistry.Setup(x => x.TypeScanner).Returns((ITypeScanner)null);

            var locator =
                CreatePartRegistryLocatorWithMockRegistries(knownRegistry);

            // Act
            var result = locator.GetRegistries();

            // Assert
            result.Count().ShouldEqual(1);
        }

        [Test]
        public void Should_return_combination_of_known_and_located_registries()
        {
            // Arrange
            var knownRegistry = 
                CreateMockRegistryOfType<FakePartRegistryWithDefinedTypeScanner>()
                    .WithTypeScannerContaining(typeof(FakePartRegistry));

            var locator =
                CreatePartRegistryLocatorWithMockRegistries(knownRegistry);

            // Act
            var result = locator.GetRegistries();

            // Assert
            result.Count().ShouldEqual(2);
        }

        [Test]
        public void Should_ignore_non_registry_type_returned_by_typescanners()
        {
            // Arrange
            var knownRegistry =
                CreateMockRegistryOfType<FakePartRegistryWithDefinedTypeScanner>()
                    .WithTypeScannerContaining(
                        typeof(FakePartRegistry),
                        typeof(object));
            
            var locator =
                CreatePartRegistryLocatorWithMockRegistries(knownRegistry);

            // Act
            var result = locator.GetRegistries();

            // Assert
            result.Count().ShouldEqual(2);
        }

        [Test]
        public void Should_not_instantiate_same_registry_more_than_once_time()
        {
            // Arrange
            var locator = 
                CreatePartRegistryLocatorWithConcreteRegistries(
                    new FakePartRegistry(),
                    new PartRegistryWithNoTypeScanner());

            // Act
            var result = locator.GetRegistries();

            // Assert
            result.Count().ShouldEqual(2);
        }

        [Test]
        public void Should_traverse_all_typescanners_until_no_more_registries_can_be_located()
        {
            // Arrange
            var locator =
                CreatePartRegistryLocatorWithConcreteRegistries(
                    new FakePartRegistryWithDefinedTypeScanner());

            // Act
            var result = locator.GetRegistries();

            // Assert
            result.Count().ShouldEqual(3);
        }

        private static PartRegistryLocator CreatePartRegistryLocatorWithMockRegistries(params Mock<IPartRegistry<IContractService>>[] registries)
        {
            var concreateRegistries =
                registries.Select(mock => mock.Object).ToArray();

            return CreatePartRegistryLocatorWithConcreteRegistries(concreateRegistries);
        }

        private static PartRegistryLocator CreatePartRegistryLocatorWithConcreteRegistries(params IPartRegistry<IContractService>[] registries)
        {
            return new PartRegistryLocator(registries);
        }

        private static Mock<IPartRegistry<IContractService>> CreateMockRegistryOfType<T>() where T : IPartRegistry<IContractService>
        {
            var registry = new Mock<IPartRegistry<IContractService>>();
            registry.Setup(x => x.GetType()).Returns(typeof(T));

            return registry;
        }
    }

    public class FakePartRegistry : PartRegistry
    {
    }

    class FakeNonPublicPartRegistry : PartRegistry
    {
    }

    public class PartRegistryWithNoTypeScanner : PartRegistry
    {
        public PartRegistryWithNoTypeScanner()
        {
            this.TypeScanner = null;
        }
    }

    public class FakePartRegistryWithNoDefaultCtor : PartRegistry
    {
        private FakePartRegistryWithNoDefaultCtor()
        {
        }
    }

    public class FakePartDiscoveredFromTypeScanner : PartRegistry
    {
        public FakePartDiscoveredFromTypeScanner()
        {
            Scan(x => x.Types(new[] { typeof(FakePartRegistry) }));
        }
    }

    public class FakePartRegistryWithDefinedTypeScanner : PartRegistry
    {
        public FakePartRegistryWithDefinedTypeScanner()
        {
            Scan(x => x.Types(new[] { typeof(FakePartDiscoveredFromTypeScanner) }));
        }
    }

    public static class IPartRegistryExtensions
    {
        public static Mock<IPartRegistry<IContractService>> WithTypeScannerContaining(this Mock<IPartRegistry<IContractService>> registry, params Type[] registryTypes)
        {
            var scanner = new Mock<ITypeScanner>();
            scanner.Setup(x => x.GetTypes(It.IsAny<Predicate<Type>>())).Returns(registryTypes);

            registry.Setup(x => x.TypeScanner).Returns(scanner.Object);

            return registry;
        }
    }
}