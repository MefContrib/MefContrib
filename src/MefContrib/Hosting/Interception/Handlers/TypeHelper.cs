namespace MefContrib.Hosting.Interception.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Primitives;
    using System.ComponentModel.Composition.ReflectionModel;
    using System.Reflection;

    public static class TypeHelper
    {
        public static Type GetImportDefinitionType(ImportDefinition definition)
        {
            
            Type importDefinitionType = null;
            importDefinitionType = ReflectionModelServices.IsImportingParameter(definition)
                                       ? GetParameterType(definition)
                                       : GetMethodType(definition, importDefinitionType);
            
            return importDefinitionType;
        }

        public static Type GetMethodType(ImportDefinition definition, Type importDefinitionType)
        {
            var memberInfos = ReflectionModelServices.GetImportingMember(definition).GetAccessors();
            var memberInfo = memberInfos[0];

            if (memberInfo.MemberType == MemberTypes.Method)
            {
                var methodInfo = (MethodInfo)memberInfo;
                importDefinitionType = methodInfo.ReturnType;
            }
            else if (memberInfo.MemberType == MemberTypes.Field)
            {
                var fieldInfo = (FieldInfo)memberInfo;
                importDefinitionType = fieldInfo.FieldType;
            }
            return importDefinitionType;
        }

        public static Type GetParameterType(ImportDefinition definition)
        {
            var importingParameter = ReflectionModelServices.GetImportingParameter(definition);
            var parameterInfo = importingParameter.Value;
            var importDefinitionType = parameterInfo.ParameterType;
            
            return importDefinitionType;
        }

        public static Type BuildGenericType(Type importDefinitionType, IDictionary<Type, Type> genericTypes)
        {
            var genericImportTypeDefinition = importDefinitionType.GetGenericTypeDefinition();
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
