namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Contains the methods used to retrive contract name and type identity for import and export conventions.
    /// </summary>
    public class ConventionContractService : IContractService
    {
        /// <summary>
        /// Gets contract name for the provided <see cref="IExportConvention"/>.
        /// </summary>
        /// <param name="exportConvention">The <see cref="IExportConvention"/> that the contract name should be retreived for.</param>
        /// <param name="member">The <see cref="MemberInfo"/> that is being exported.</param>
        /// <returns>A <see cref="string"/> containing the contract name for the export.</returns>
        /// <exception cref="ArgumentNullException">The value of the <paramref name="member"/> or <paramref name="exportConvention"/> parameter was <see langword="null"/>.</exception>
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

            return ContractService.GetExportContractName(
                (exportConvention.ContractName == null) ? null : exportConvention.ContractName.Invoke(member),
                (exportConvention.ContractType == null) ? null : exportConvention.ContractType.Invoke(member),
                member);
        }

        /// <summary>
        /// Gets type identity for the provided <see cref="IExportConvention"/>.
        /// </summary>
        /// <param name="exportConvention">The <see cref="IExportConvention"/> that the type identity should be retreived for.</param>
        /// <param name="member">The <see cref="MemberInfo"/> that is being exported.</param>
        /// <returns>A <see cref="string"/> containing the type identity for the export.</returns>
        /// <exception cref="ArgumentNullException">The value of the <paramref name="member"/> or <paramref name="exportConvention"/> parameter was <see langword="null"/>.</exception>
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

            return ContractService.GetExportTypeIdentity(
                (exportConvention.ContractType == null) ? null : exportConvention.ContractType.Invoke(member),
                member);
        }

        /// <summary>
        /// Gets contract name for the provided <see cref="IImportConvention"/>.
        /// </summary>
        /// <param name="importConvention">The <see cref="IImportConvention"/> that the contract name should be retreived for.</param>
        /// <param name="member">The <see cref="MemberInfo"/> that is being imported.</param>
        /// <returns>A <see cref="string"/> containing the contract name for the import.</returns>
        /// <exception cref="ArgumentNullException">The value of the <paramref name="member"/> or <paramref name="importConvention"/> parameter was <see langword="null"/>.</exception>
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

            return ContractService.GetImportContractName(
                (importConvention.ContractName == null) ? null : importConvention.ContractName.Invoke(member),
                (importConvention.ContractType == null) ? null : importConvention.ContractType.Invoke(member),
                member);
        }

        /// <summary>
        /// Gets type identity for the provided <see cref="IImportConvention"/>.
        /// </summary>
        /// <param name="importConvention">The <see cref="IImportConvention"/> that the type identity should be retreived for.</param>
        /// <param name="member">The <see cref="MemberInfo"/> that is being imported.</param>
        /// <returns>A <see cref="string"/> containing the type identity for the imported.</returns>
        /// <exception cref="ArgumentNullException">The value of the <paramref name="member"/> or <paramref name="importConvention"/> parameter was <see langword="null"/>.</exception>
        public virtual string GetImportTypeIdentity(IImportConvention importConvention, MemberInfo member)
        {
            if (importConvention == null)
            {
                throw new ArgumentNullException("importConvention", "The import convention cannot be null.");
            }

            if (member == null)
            {
                throw new ArgumentNullException("member", "The member cannot be null.");
            }

            return ContractService.GetImportTypeIdentity(
                (importConvention.ContractType == null) ? null : importConvention.ContractType.Invoke(member),
                member);
        }
    }
}