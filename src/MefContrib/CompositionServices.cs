namespace MefContrib
{
    using System;
    using System.ComponentModel.Composition.Primitives;
    using System.ComponentModel.Composition.ReflectionModel;
    using System.Reflection;

    public static class CompositionServices
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

        public static bool IsReflectionImportDefinition(ImportDefinition definition)
        {
            var name = definition.GetType().Name;
            return name == "ReflectionMemberImportDefinition" ||
                   name == "ReflectionParameterImportDefinition";
        }
    }
}