namespace MefContrib.Hosting.Generics
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Contains utility methods for open-generics support mechanism.
    /// </summary>
    public static class TypeHelper
    {
        public static IEnumerable<Type> BuildGenericTypes(Type importDefinitionType, IEnumerable<Type> implementations)
        {
            if (importDefinitionType == null)
            {
                throw new ArgumentNullException("importDefinitionType");
            }

            if (implementations == null)
            {
                throw new ArgumentNullException("implementations");
            }

            if (implementations.Count() == 0)
            {
                throw new ArgumentException("Implementation collection is empty.", "implementations");
            }
            
            var genericArguments = importDefinitionType.GetGenericArguments();
            return implementations
                .Select(implementationType => implementationType.MakeGenericType(genericArguments))
                .ToList();
        }

        public static bool ShouldCreateClosedGenericPart(Type importDefinitionType)
        {
            if (importDefinitionType == null)
            {
                throw new ArgumentNullException("importDefinitionType");
            }

            return importDefinitionType.IsGenericType;
        }

        /// <summary>
        /// Checks if the given type is a collection.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>True</c> if the given type is a collection. False otherwise.</returns>
        public static bool IsCollection(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return typeof(IEnumerable).IsAssignableFrom(type) && typeof(string) != type;
        }
        
        /// <summary>
        /// Checks if the given type is a generic collection.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>True</c> if the given type is a generic collection. False otherwise.</returns>
        public static bool IsGenericCollection(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return IsCollection(type) && TryGetAncestor(type, typeof(IEnumerable<>)) != null;
        }
        
        /// <summary>
        /// Gets the type of object the collection can hold.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of the collection.</param>
        /// <returns><see cref="Type"/> of object the collection can hold.</returns>
        public static Type GetGenericCollectionParameter(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (IsCollection(type) == false)
            {
                throw new ArgumentException("Type is not a collection.", "type");
            }

            var genericType = TryGetAncestor(type, typeof(IEnumerable<>));
            if (genericType == null)
            {
                throw new ArgumentException("Type is not a generic collection.", "type");
            }

            return genericType.GetGenericArguments()[0];
        }
        
        /// <summary>
        /// Tries to get ancestor of a given type.
        /// </summary>
        /// <param name="type">Source type.</param>
        /// <param name="ancestor">Ancestor's type.</param>
        /// <returns>The ancestor type if found. <c>Null</c> otherwise.</returns>
        public static Type TryGetAncestor(Type type, Type ancestor)
        {
            if (ancestor == type) return type;
            if (type.IsGenericType && ancestor == type.GetGenericTypeDefinition()) return type;

            var baseTypes = new List<Type>(type.GetInterfaces()) { type.BaseType };
            foreach (var baseType in baseTypes)
            {
                if (baseType != null)
                {
                    var result = TryGetAncestor(baseType, ancestor);
                    if (result != null) return result;
                }
            }

            return null;
        }
    }
}
