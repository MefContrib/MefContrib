namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.ComponentModel.Composition;
    using System.Reflection;

    public static class ContractServices
    {
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
    }


    /// <summary>
    /// Contains the methods used to retrive contract name and type identity for imports and exports.
    /// </summary>
    public class ConventionContractService : IContractService
    {
        public virtual string GetExportContractName(IExportConvention exportConvention, MemberInfo member)
        {
            if (exportConvention == null)
            {
                throw new ArgumentNullException("exportConvention", "The export convention cannot be null.");
            }

            if (member == null)
            {
                throw new ArgumentNullException("member", "The member cannot be null.");
            }

            return ContractServices.GetExportContractName(
                (exportConvention.ContractName == null) ? null : exportConvention.ContractName.Invoke(member),
                (exportConvention.ContractType == null) ? null : exportConvention.ContractType.Invoke(member),
                member);
        }

        /// <summary>
        /// Gets contract name for the provided <see cref="IExportConvention"/>.
        /// </summary>
        /// <param name="exportConvention">The <see cref="IExportConvention"/> that the contract name should be retreived for.</param>
        /// <param name="member">The <see cref="MemberInfo"/> that is being exported.</param>
        /// <returns>A <see cref="string"/> containing the contract name for the export.</returns>
        public virtual string GetExportContractNames(IExportConvention exportConvention, MemberInfo member)
        {
            if (exportConvention == null)
            {
                throw new ArgumentNullException("exportConvention", "The export convention cannot be null.");
            }

            if (member == null)
            {
                throw new ArgumentNullException("member", "The member cannot be null.");
            }

            if (exportConvention.ContractName != null)
            {
                return exportConvention.ContractName.Invoke(member);
            }

            if (exportConvention.ContractType != null)
            {
                return AttributedModelServices.GetContractName(exportConvention.ContractType.Invoke(member));
            }

            return member.MemberType == MemberTypes.Method ?
                AttributedModelServices.GetTypeIdentity((MethodInfo)member) :
                AttributedModelServices.GetContractName(member.GetContractMember());
        }

        /// <summary>
        /// Gets type identity for the provided <see cref="IExportConvention"/>.
        /// </summary>
        /// <param name="exportConvention">The <see cref="IExportConvention"/> that the type identity should be retreived for.</param>
        /// <param name="member">The <see cref="MemberInfo"/> that is being exported.</param>
        /// <returns>A <see cref="string"/> containing the type identity for the export.</returns>
        public virtual string GetExportTypeIdentity(IExportConvention exportConvention, MemberInfo member)
        {
            if (exportConvention == null)
            {
                throw new ArgumentNullException("exportConvention", "The export convention cannot be null.");
            }

            if (member == null)
            {
                throw new ArgumentNullException("member", "The member cannot be null.");
            }

            if (exportConvention.ContractType != null)
            {
                return AttributedModelServices.GetTypeIdentity(exportConvention.ContractType.Invoke(member));
            }

            return member.MemberType.Equals(MemberTypes.Method) ?
                AttributedModelServices.GetTypeIdentity((MethodInfo)member) :
                AttributedModelServices.GetTypeIdentity(member.GetContractMember());
        }

        /// <summary>
        /// Gets contract name for the provided <see cref="IImportConvention"/>.
        /// </summary>
        /// <param name="importConvention">The <see cref="IImportConvention"/> that the contract name should be retreived for.</param>
        /// <param name="member">The <see cref="MemberInfo"/> that is being imported.</param>
        /// <returns>A <see cref="string"/> containing the contract name for the import.</returns>
        public virtual string GetImportContractName(IImportConvention importConvention, MemberInfo member)
        {
            if (importConvention == null)
            {
                throw new ArgumentNullException("importConvention", "The import convention cannot be null.");
            }

            if (member == null)
            {
                throw new ArgumentNullException("member", "The member cannot be null.");
            }

            if (importConvention.ContractName != null)
            {
                return importConvention.ContractName.Invoke(member);
            }

            return importConvention.ContractType != null ?
                AttributedModelServices.GetTypeIdentity(importConvention.ContractType.Invoke(member)) :
                AttributedModelServices.GetTypeIdentity(member.GetContractMember());
        }

        /// <summary>
        /// Gets type identity for the provided <see cref="IImportConvention"/>.
        /// </summary>
        /// <param name="importConvention">The <see cref="IImportConvention"/> that the type identity should be retreived for.</param>
        /// <param name="member">The <see cref="MemberInfo"/> that is being imported.</param>
        /// <returns>A <see cref="string"/> containing the type identity for the imported.</returns>
        public virtual string GetImportTypeIdentity(IImportConvention importConvention, MemberInfo member)
        {
            if (importConvention.ContractType == null)
            {
                return AttributedModelServices.GetTypeIdentity(member.GetContractMember());
            }

            if (importConvention.ContractType.Invoke(member).Equals(typeof(object)))
            {
                return null;
            }

            var memberType =
                member.GetContractMember();

            return memberType.IsSubclassOf(typeof(Delegate)) ?
                AttributedModelServices.GetTypeIdentity(memberType.GetMethod("Invoke")) :
                AttributedModelServices.GetTypeIdentity(importConvention.ContractType.Invoke(member));
        }
    }
}