namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MefContrib.Hosting.Conventions.Configuration;

    public interface IPartRegistryLocator
    {
        IEnumerable<IPartRegistry<IContractService>> Locate();
    }

    public class AppDomainPartRegistryLocator : IPartRegistryLocator
    {
        private readonly AppDomain appDomain;

        public AppDomainPartRegistryLocator(AppDomain appDomain)
        {
            this.appDomain = appDomain;
        }

        public IEnumerable<IPartRegistry<IContractService>> Locate()
        {
            var registries =
                GetPublicPartRegistryInstancesInAppDomain(this.appDomain);

            var result =
                ScanForDiscoverableRegistries(registries);

            return result;
        }

        private static IEnumerable<IPartRegistry<IContractService>> ScanForDiscoverableRegistries(IEnumerable<IPartRegistry<IContractService>> registries)
        {
            var knowPartRegistries = 
                new List<IPartRegistry<IContractService>>();
            knowPartRegistries.AddRange(registries);

            var discoveredPartRegistries =
                DiscoverPartRegistriesFromRegistryTypeScanner(registries);

            if (discoveredPartRegistries.Count() > 0)
            {
                knowPartRegistries.AddRange(ScanForDiscoverableRegistries(discoveredPartRegistries));
            }

            return knowPartRegistries;
        }

        private static IEnumerable<IPartRegistry<IContractService>> DiscoverPartRegistriesFromRegistryTypeScanner(IEnumerable<IPartRegistry<IContractService>> registries)
        {
            return registries
                .Where(registry => registry.TypeScanner != null)
                .SelectMany(registry => registry.TypeScanner.GetTypes(TypeImplements<IPartRegistry<IContractService>>))
                //.Where(type => !registries.Any(x => x.GetType().Equals(type)))
                .Where(x => RegistryNotPreviouslyDiscovered(x, registries))
                .Select(CreateNewRegistryInstance);
        }

        private static bool RegistryNotPreviouslyDiscovered(Type type, IEnumerable<IPartRegistry<IContractService>> registries)
        {
            var isPreviouslyDiscovered =
                registries.Any(x => x.GetType().Equals(type));

            return !isPreviouslyDiscovered;
        }

        private static IEnumerable<IPartRegistry<IContractService>> GetPublicPartRegistryInstancesInAppDomain(AppDomain domain)
        {
            var results = domain
                .GetAssemblies()
                .Select(x => GetPublicPartRegistryTypes(x.GetTypes()))
                .SelectMany(CreateNewPartRegistryInstances);

            return results;
        }

        public static IEnumerable<IPartRegistry<IContractService>> CreateNewPartRegistryInstances(IEnumerable<Type> types)
        {
            return types.Select(CreateNewRegistryInstance);
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
    }
}