namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.ComponentModel.Composition;
    using System.Reflection;

    public interface IContractService
    {
        /// <summary>
        /// Gets contract name for the provided <see cref="IExportConvention"/>.
        /// </summary>
        /// <param name="exportConvention">The <see cref="IExportConvention"/> that the contract name should be retreived for.</param>
        /// <param name="member">The <see cref="MemberInfo"/> that is being exported.</param>
        /// <returns>A <see cref="string"/> containing the contract name for the export.</returns>
        string GetExportContractName(IExportConvention exportConvention, MemberInfo member);

        /// <summary>
        /// Gets type identity for the provided <see cref="IExportConvention"/>.
        /// </summary>
        /// <param name="exportConvention">The <see cref="IExportConvention"/> that the type identity should be retreived for.</param>
        /// <param name="member">The <see cref="MemberInfo"/> that is being exported.</param>
        /// <returns>A <see cref="string"/> containing the type identity for the export.</returns>
        string GetExportTypeIdentity(IExportConvention exportConvention, MemberInfo member);

        /// <summary>
        /// Gets contract name for the provided <see cref="IImportConvention"/>.
        /// </summary>
        /// <param name="importConvention">The <see cref="IImportConvention"/> that the contract name should be retreived for.</param>
        /// <param name="member">The <see cref="MemberInfo"/> that is being imported.</param>
        /// <returns>A <see cref="string"/> containing the contract name for the import.</returns>
        string GetImportContractName(IImportConvention importConvention, MemberInfo member);

        /// <summary>
        /// Gets type identity for the provided <see cref="IImportConvention"/>.
        /// </summary>
        /// <param name="importConvention">The <see cref="IImportConvention"/> that the type identity should be retreived for.</param>
        /// <param name="member">The <see cref="MemberInfo"/> that is being imported.</param>
        /// <returns>A <see cref="string"/> containing the type identity for the imported.</returns>
        string GetImportTypeIdentity(IImportConvention importConvention, MemberInfo member);
    }

    public class DefaultContractService : IContractService
    {
        /// <summary>
        /// Gets contract name for the provided <see cref="IExportConvention"/>.
        /// </summary>
        /// <param name="exportConvention">The <see cref="IExportConvention"/> that the contract name should be retreived for.</param>
        /// <param name="member">The <see cref="MemberInfo"/> that is being exported.</param>
        /// <returns>A <see cref="string"/> containing the contract name for the export.</returns>
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