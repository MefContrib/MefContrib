namespace MefContrib.Hosting.Generics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Primitives;

    /// <summary>
    /// Contains utility methods for open-generics support mechanism.
    /// </summary>
    public static class TypeHelper
    {
        public static Type BuildGenericType(Type importDefinitionType, IDictionary<Type, Type> genericTypes)
        {
            if (importDefinitionType == null)
            {
                throw new ArgumentNullException("importDefinitionType");
            }

            if (genericTypes == null)
            {
                throw new ArgumentNullException("genericTypes");
            }

            var genericImportTypeDefinition = importDefinitionType.GetGenericTypeDefinition();

            if (!genericTypes.ContainsKey(genericImportTypeDefinition))
            {
                if (importDefinitionType.IsClass && !importDefinitionType.IsAbstract)
                {
                    return importDefinitionType;
                }

                throw new MappingNotFoundException(
                    genericImportTypeDefinition,
                    string.Format("Implementation type for {0} has not been found.",
                        genericImportTypeDefinition.Name));
            }

            var genericTypeLocator = genericTypes[genericImportTypeDefinition];
            var genericType = genericTypeLocator.MakeGenericType(importDefinitionType.GetGenericArguments());
            
            return genericType;
        }

        public static bool ShouldCreateClosedGenericPart(ContractBasedImportDefinition definition, Type importDefinitionType)
        {
            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }

            if (importDefinitionType == null)
            {
                throw new ArgumentNullException("importDefinitionType");
            }

            return definition.Cardinality != ImportCardinality.ZeroOrMore && importDefinitionType.IsGenericType;
        }
    }
}
