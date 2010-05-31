namespace MefContrib.Hosting.Conventions
{
    using System.Reflection;

    /// <summary>
    /// Defines the methods used to retrive contract name and type identity for imports and exports.
    /// </summary>
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
}
