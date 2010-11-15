namespace MefContrib.Hosting.Generics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Primitives;

    public static class TypeHelper
    {
        public static Type BuildGenericType(Type importDefinitionType, IDictionary<Type, Type> genericTypes)
        {
            var genericImportTypeDefinition = importDefinitionType.GetGenericTypeDefinition();

            if (!genericTypes.ContainsKey(genericImportTypeDefinition))
            {
                throw new MappingNotFoundException(
                    genericImportTypeDefinition,
                    string.Format("Implementation type for {0} has not been found.",
                        genericImportTypeDefinition.Name));
            }

            var genericTypeLocator = genericTypes[genericImportTypeDefinition];
            var genericType = genericTypeLocator.MakeGenericType(importDefinitionType.GetGenericArguments());
            
            return genericType;
        }

        public static bool ShouldCreateClosedGenericPart(ContractBasedImportDefinition contractDef, Type importDefinitionType)
        {
            return contractDef.Cardinality != ImportCardinality.ZeroOrMore && importDefinitionType.IsGenericType;
        }
    }
}
