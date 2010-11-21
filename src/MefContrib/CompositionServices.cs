namespace MefContrib
{
    using System;
    using System.ComponentModel.Composition.Primitives;
    using System.ComponentModel.Composition.ReflectionModel;
    using System.Reflection;

    /// <summary>
    /// Contains helper methods used during composition.
    /// </summary>
    public static class CompositionServices
    {
        /// <summary>
        /// Gets the <see cref="Type"/> of the requested import.
        /// </summary>
        /// <param name="definition">The <see cref="ImportDefinition"/> instance.</param>
        /// <returns>A <see cref="Type"/> of the requested import.</returns>
        /// <remarks>Works only for reflection model import definitions.</remarks>
        public static Type GetImportDefinitionType(ImportDefinition definition)
        {
            Type importDefinitionType = null;
            importDefinitionType = ReflectionModelServices.IsImportingParameter(definition)
                                       ? GetParameterType(definition)
                                       : GetMethodType(definition, importDefinitionType);

            return importDefinitionType;
        }

        private static Type GetMethodType(ImportDefinition definition, Type importDefinitionType)
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

        private static Type GetParameterType(ImportDefinition definition)
        {
            var importingParameter = ReflectionModelServices.GetImportingParameter(definition);
            var parameterInfo = importingParameter.Value;
            var importDefinitionType = parameterInfo.ParameterType;

            return importDefinitionType;
        }

        /// <summary>
        /// Checks if the given <see cref="ImportDefinition"/> is based on reflection model.
        /// </summary>
        /// <param name="definition">The <see cref="ImportDefinition"/> instance.</param>
        /// <returns><c>True</c> if the given import definition is based on the
        /// reflection model. <c>False</c> otherwise.</returns>
        public static bool IsReflectionImportDefinition(ImportDefinition definition)
        {
            if (definition == null) throw new ArgumentNullException("definition");

            var name = definition.GetType().Name;
            return name == "ReflectionMemberImportDefinition" ||
                   name == "ReflectionParameterImportDefinition";
        }
    }
}