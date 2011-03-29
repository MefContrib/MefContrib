namespace MefContrib
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Primitives;
    using System.ComponentModel.Composition.ReflectionModel;
    using System.Reflection;
    using MefContrib.Hosting.Conventions;

    /// <summary>
    /// Contains the methods used to retrive contract name and type identity for imports and exports.
    /// </summary>
    /// <remarks>
    /// The contract name and type identities produced by this class are compatible with those that are 
    /// used by the Attributed Programming Model that ships with the Managed Extensibility Framework.
    /// </remarks>
    public static class ContractServices
    {
        /// <summary>
        /// Gets the export contract name based on the provided hints.
        /// </summary>
        /// <param name="contractName">A <see cref="string"/> containing the name of the contract to use for the export.</param>
        /// <param name="contractType">The <see cref="Type"/> of the contract to use for the export.</param>
        /// <param name="member">A <see cref="MemberInfo"/> instance for the member that is being exported.</param>
        /// <returns>A <see cref="string"/> containing the contract name.</returns>
        /// <exception cref="ArgumentNullException">The value of the <paramref name="member"/> parameter was <see langword="null"/>.</exception>
        public static string GetExportContractName(string contractName, Type contractType, MemberInfo member)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member", "The member cannot be null.");
            }

            if (contractName != null)
            {
                return contractName;
            }

            if (contractType != null)
            {
                return AttributedModelServices.GetContractName(contractType);
            }

            return member.MemberType == MemberTypes.Method ?
                AttributedModelServices.GetTypeIdentity((MethodInfo)member) :
                AttributedModelServices.GetContractName(member.GetContractMember());
        }

        /// <summary>
        /// Gets export type identity based on the provided hints.
        /// </summary>
        /// <param name="contractType">The <see cref="Type"/> of the type identity to use for the export.</param>
        /// <param name="member">A <see cref="MemberInfo"/> instance for the member that is being exported.</param>
        /// <returns>A <see cref="string"/> containing the type identity.</returns>
        /// <exception cref="ArgumentNullException">The value of the <paramref name="member"/> parameter was <see langword="null"/>.</exception>
        public static string GetExportTypeIdentity(Type contractType, MemberInfo member)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member", "The member cannot be null.");
            }

            if (contractType != null)
            {
                return AttributedModelServices.GetTypeIdentity(contractType);
            }

            return member.MemberType.Equals(MemberTypes.Method) ?
                AttributedModelServices.GetTypeIdentity((MethodInfo)member) :
                AttributedModelServices.GetTypeIdentity(member.GetContractMember());
        }

        /// <summary>
        /// Gets the import contract name based on the provided hints.
        /// </summary>
        /// <param name="contractName">A <see cref="string"/> containing the name of the contract to use for the import.</param>
        /// <param name="contractType">The <see cref="Type"/> of the contract to use for the import.</param>
        /// <param name="member">A <see cref="MemberInfo"/> instance for the member that is being imported.</param>
        /// <returns>A <see cref="string"/> containing the contract name.</returns>
        /// <exception cref="ArgumentNullException">The value of the <paramref name="member"/> parameter was <see langword="null"/>.</exception>
        public static string GetImportContractName(string contractName, Type contractType, MemberInfo member)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member", "The member cannot be null.");
            }

            if (contractName != null)
            {
                return contractName;
            }

            return contractType != null ?
                AttributedModelServices.GetTypeIdentity(contractType) :
                AttributedModelServices.GetTypeIdentity(member.GetContractMember());
        }

        /// <summary>
        /// Gets import type identity based on the provided hints.
        /// </summary>
        /// <param name="contractType">The <see cref="Type"/> of the type identity to use for the import.</param>
        /// <param name="member">A <see cref="MemberInfo"/> instance for the member that is being imported.</param>
        /// <returns>A <see cref="string"/> containing the type identity.</returns>
        /// <exception cref="ArgumentNullException">The value of the <paramref name="member"/> parameter was <see langword="null"/>.</exception>
        public static string GetImportTypeIdentity(Type contractType, MemberInfo member)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member", "The member cannot be null.");
            }

            if (contractType == null)
            {
                return AttributedModelServices.GetTypeIdentity(member.GetContractMember());
            }

            if (contractType.Equals(typeof(object)))
            {
                return null;
            }

            var memberType =
                member.GetContractMember();

            return memberType.IsSubclassOf(typeof(Delegate)) ?
                AttributedModelServices.GetTypeIdentity(memberType.GetMethod("Invoke")) :
                AttributedModelServices.GetTypeIdentity(contractType);
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of the given <see cref="ImportDefinition"/>.
        /// </summary>
        /// <param name="definition">The <see cref="ImportDefinition"/> instance.</param>
        /// <returns>A <see cref="Type"/> of the requested import.</returns>
        /// <remarks>Works only for reflection model import definitions.</remarks>
        public static Type GetImportDefinitionType(ImportDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }

            var importDefinitionType = ReflectionModelServices.IsImportingParameter(definition)
                                           ? GetParameterType(definition)
                                           : GetMethodType(definition);

            return importDefinitionType;
        }

        private static Type GetMethodType(ImportDefinition definition)
        {
            Type importDefinitionType = null;
            var member = ReflectionModelServices.GetImportingMember(definition);
            if (member.MemberType == MemberTypes.Property)
            {
                var methodInfo = (MethodInfo)member.GetAccessors()[1];
                importDefinitionType = methodInfo.GetParameters()[0].ParameterType;
            }
            else if (member.MemberType == MemberTypes.Method)
            {
                var methodInfo = (MethodInfo)member.GetAccessors()[0];
                importDefinitionType = methodInfo.ReturnType;
            }
            else if (member.MemberType == MemberTypes.Field)
            {
                var fieldInfo = (FieldInfo)member.GetAccessors()[0];
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
            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }

            var name = definition.GetType().Name;
            return name == "ReflectionMemberImportDefinition" ||
                   name == "ReflectionParameterImportDefinition";
        }
    }
}