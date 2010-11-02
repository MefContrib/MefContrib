namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MefContrib.Hosting.Conventions.Configuration;

    public class PartRegistryTypeFilter
    {
        public static IEnumerable<Type> Filter(IEnumerable<Type> types)
        {
            return types
                .Where(TypeImplementsPartRegistryInterface)
                .Where(RegistryHasDefaultConstructor)
                .Where(RegistryIsPublicType);
        }

        private static bool TypeImplementsPartRegistryInterface(Type type)
        {
            return type.GetInterfaces()
                .Where(x => x.IsGenericType)
                .Any(x => x.GetGenericTypeDefinition().Equals(typeof(IPartRegistry<>)));
        }

        private static bool RegistryHasDefaultConstructor(Type type)
        {
            return type.HasDefaultConstructor();
        }

        private static bool RegistryIsPublicType(Type type)
        {
            return type.IsPublic;
        }
    }
}