namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MefContrib.Hosting.Conventions.Configuration;

    public class AppDomainPartRegistryLocator : IPartRegistryLocator
    {
        private readonly AppDomain domain;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDomainPartRegistryLocator"/> class,
        /// for the current <see cref="AppDomain"/>.
        /// </summary>
        public AppDomainPartRegistryLocator()
            : this(AppDomain.CurrentDomain)
        {
        }

        public AppDomainPartRegistryLocator(AppDomain domain)
        {
            if (this.domain == null)
            {
                throw new ArgumentNullException("domain", "The domain parameter cannot be null.");
            }

            this.domain = this.domain;
        }

        public IEnumerable<IPartRegistry<IContractService>> GetRegistries()
        {
            var registries =
                GetPublicPartRegistryInstancesInAppDomain(this.domain);

            var locator =
                new PartRegistryLocator(registries);

            return locator.GetRegistries();
        }

        private static IEnumerable<IPartRegistry<IContractService>> GetPublicPartRegistryInstancesInAppDomain(AppDomain domain)
        {
            var registryInstancesLocatedInDomain = domain
                .GetAssemblies()
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