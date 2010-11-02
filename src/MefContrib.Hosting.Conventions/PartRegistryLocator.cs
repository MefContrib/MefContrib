namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MefContrib.Hosting.Conventions.Configuration;

    public class PartRegistryLocator : IPartRegistryLocator
    {
        private IEnumerable<IPartRegistry<IContractService>> registries;

        public PartRegistryLocator(IEnumerable<IPartRegistry<IContractService>> registries)
        {
            if (registries == null)
            {
                throw new ArgumentNullException("registries", "The registries parameter cannot be null.");
            }

            this.registries = registries;
        }

        public IEnumerable<IPartRegistry<IContractService>> GetRegistries()
        {
            var reachableTypes = this.registries
                .Where(TypeScannerIsNotNull)
                .SelectMany(GetTypesFromTypeScanner)  // This should filter on actual type. Need to fix test
                .ToList();

            var typesThatMatchCritierias = PartRegistryTypeFilter.Filter(reachableTypes)
                .Where(RegistryIsNotKnown)
                .ToList();

            if (typesThatMatchCritierias.Count() > 0)
            {
                this.registries = this.CreateInstancesAndRetrieveSateliteRegistries(typesThatMatchCritierias);
            }

            return this.registries;
        }

        private IEnumerable<IPartRegistry<IContractService>> CreateInstancesAndRetrieveSateliteRegistries(IEnumerable<Type> types)
        {
            var instances = types
                .Select(CreatePartRegistryInstance);

            var locator =
                new PartRegistryLocator(this.registries.Concat(instances));

            return locator.GetRegistries();
        }

        private static IEnumerable<Type> GetTypesFromTypeScanner(IPartRegistry<IContractService> registry)
        {
            return registry.TypeScanner.GetTypes(t => true);
        }

        private static IPartRegistry<IContractService> CreatePartRegistryInstance(Type type)
        {
            return (IPartRegistry<IContractService>)Activator.CreateInstance(type);
        }

        private static bool TypeScannerIsNotNull(IPartRegistry<IContractService> registry)
        {
            return registry.TypeScanner != null;
        }

        private bool RegistryIsNotKnown(Type type)
        {
            return !this.registries.Any(x => x.GetType().Equals(type));
        }
    }
}