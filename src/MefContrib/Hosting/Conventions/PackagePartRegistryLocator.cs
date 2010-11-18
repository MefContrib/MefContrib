namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using MefContrib.Hosting.Conventions.Configuration;

    /// <summary>
    /// Locates <see cref="IPartRegistry{TContractService}"/> instances in the XAP file. For each located registry, a
    /// <see cref="PartRegistryLocator"/> is created and invoked.
    /// </summary>
    public class PackagePartRegistryLocator : IPartRegistryLocator
    {
        private readonly IEnumerable<Assembly> assemblies;

        /// <summary>
        /// Initializes a new instance of the <see cref="PackagePartRegistryLocator"/> class.
        /// </summary>
        public PackagePartRegistryLocator()
        {
            assemblies = Package.CurrentAssemblies;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackagePartRegistryLocator"/> class.
        /// </summary>
        /// <param name="packageStream"><see cref="Stream"/> to the XAP file.</param>
        public PackagePartRegistryLocator(Stream packageStream)
        {
            if (packageStream == null)
            {
                throw new ArgumentNullException("packageStream");
            }

            assemblies = Package.LoadPackagedAssemblies(packageStream);
        }

        /// <summary>
        /// Locates <see cref="IPartRegistry{TContractService}"/> instances in the domain.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IPartRegistry{TContractService}"/> instances.</returns>
        public IEnumerable<IPartRegistry<IContractService>> GetRegistries()
        {
            var registries =
                GetPublicPartRegistryInstances();

            var locator =
                new PartRegistryLocator(registries);

            return locator.GetRegistries();
        }

        private IEnumerable<IPartRegistry<IContractService>> GetPublicPartRegistryInstances()
        {
            var registryInstancesLocatedInDomain = this.assemblies
                .SelectMany(x => PartRegistryTypeFilter.Filter(x.GetTypes()))
                .Select(CreatePartRegistryInstance);

            return registryInstancesLocatedInDomain;
        }

        private static IPartRegistry<IContractService> CreatePartRegistryInstance(Type registryType)
        {
            return (IPartRegistry<IContractService>)Activator.CreateInstance(registryType);
        }
    }
}