namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Contains extension methods for the <see cref="Type"/> type.
    /// </summary>
    public static class TypeExtensions
    {
        public static IEnumerable<RequiredMetadataItem> GetRequiredMetadata(this Type source)
        {
            var requiredMetadata =
                from property in source.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                select new RequiredMetadataItem(property.Name, property.PropertyType);

            return requiredMetadata;
        }

        /// <summary>
        /// Checks if the <see cref="Type"/> implements the <see cref="ICollection"/> interface.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to inspect.</param>
        /// <returns><see langword="true" /> if the type implements the <see cref="ICollection"/> interface; otherwise <see langword="false" />.</returns>
        public static bool IsCollection(this Type type)
        {
            var results = type
                .GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(ICollection<>));

            return results.Count() > 0;
        }

        /// <summary>
        /// Checks if the <see cref="Type"/> implements the <see cref="IEnumerable"/> interface.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to inspect.</param>
        /// <returns><see langword="true" /> if the type iimplements the <see cref="IEnumerable"/> interface; otherwise <see langword="false" />.</returns>
        /// <remarks>The method will return <see langword="false"/> for <see cref="string"/> type.</remarks>
        public static bool IsEnumerable(this Type type)
        {
            if (type == typeof(string))
            {
                return false;
            }

            return type == typeof(IEnumerable) || type.GetInterfaces().Contains(typeof(IEnumerable));
        }

        public static Type GetActualType(this Type type)
        {
            return type.IsGenericType ? type.GetGenericArguments()[0] : type;
        }

        public static bool HasDefaultConstructor(this Type self)
        {
            return self.GetConstructor(Type.EmptyTypes) != null;
        }

        public static ConstructorInfo GetGreediestConstructor(this Type type)
        {
            var constructor = type
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .OrderByDescending(x => x.GetParameters().Count())
                .FirstOrDefault();

            return constructor;
        }
    }
}