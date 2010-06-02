namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reflection;
    using MefContrib.Hosting.Conventions.Configuration;

    /// <summary>
    /// Contains the methods used to retrive contract name and type identity for imports and exports. The
    /// class will first check and see if there are any default conventions stores for the contract name
    /// and contract type, of the request type, before falling back to the implementation defined by the
    /// base class <see cref="ConventionContractService"/>.
    /// </summary>
    public class DefaultConventionContractService : ConventionContractService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultConventionContractService"/> class.
        /// </summary>
        public DefaultConventionContractService()
        {
            this.DefaultConventions = new List<TypeDefaultConvention>();
        }

        /// <summary>
        /// Gets or sets the default conventions.
        /// </summary>
        /// <value>An <see cref="IList{T}"/> instance, containing <see cref="TypeDefaultConvention"/> instances.</value>
        public IList<TypeDefaultConvention> DefaultConventions { get; private set; }

        /// <summary>
        /// Gets contract name for the provided <see cref="IExportConvention"/>.
        /// </summary>
        /// <param name="exportConvention">The <see cref="IExportConvention"/> that the contract name should be retreived for.</param>
        /// <param name="member">The <see cref="MemberInfo"/> that is being exported.</param>
        /// <returns>A <see cref="string"/> containing the contract name for the export.</returns>
        public override string GetExportContractName(IExportConvention exportConvention, MemberInfo member)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member", "The member cannot be null.");
            }

            var contractMember =
                member.GetContractMember();

            var defaultContractName =
                this.DefaultConventions.Where(x => x.TargetType.Equals(contractMember))
                .Select(x => x.ContractName).LastOrDefault();

            return defaultContractName ?? base.GetExportContractName(exportConvention, member);
        }

        /// <summary>
        /// Gets type identity for the provided <see cref="IExportConvention"/>.
        /// </summary>
        /// <param name="exportConvention">The <see cref="IExportConvention"/> that the type identity should be retreived for.</param>
        /// <param name="member">The <see cref="MemberInfo"/> that is being exported.</param>
        /// <returns>A <see cref="string"/> containing the type identity for the export.</returns>
        public override string GetExportTypeIdentity(IExportConvention exportConvention, MemberInfo member)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member", "The member cannot be null.");
            }

            var contractMember =
                member.GetContractMember();

            var defaultContractType =
                this.DefaultConventions.Where(x => x.TargetType.Equals(contractMember))
                .Select(x => x.ContractType).LastOrDefault();

            return defaultContractType != null ? 
                AttributedModelServices.GetTypeIdentity(defaultContractType) : 
                base.GetExportTypeIdentity(exportConvention, member);
        }

        /// <summary>
        /// Gets contract name for the provided <see cref="IImportConvention"/>.
        /// </summary>
        /// <param name="importConvention">The <see cref="IImportConvention"/> that the contract name should be retreived for.</param>
        /// <param name="member">The <see cref="MemberInfo"/> that is being imported.</param>
        /// <returns>A <see cref="string"/> containing the contract name for the import.</returns>
        public override string GetImportContractName(IImportConvention importConvention, MemberInfo member)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member", "The member cannot be null.");
            }

            var contractMember =
                member.GetContractMember();

            var defaultContractName =
                this.DefaultConventions.Where(x => x.TargetType.Equals(contractMember))
                .Select(x => x.ContractName).LastOrDefault();

            return defaultContractName ?? base.GetImportContractName(importConvention, member);
        }

        /// <summary>
        /// Gets type identity for the provided <see cref="IImportConvention"/>.
        /// </summary>
        /// <param name="importConvention">The <see cref="IImportConvention"/> that the type identity should be retreived for.</param>
        /// <param name="member">The <see cref="MemberInfo"/> that is being imported.</param>
        /// <returns>A <see cref="string"/> containing the type identity for the imported.</returns>
        public override string GetImportTypeIdentity(IImportConvention importConvention, MemberInfo member)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member", "The member cannot be null.");
            }

            var contractMember =
                member.GetContractMember();

            var defaultContractType =
                this.DefaultConventions.Where(x => x.TargetType.Equals(contractMember))
                .Select(x => x.ContractType).LastOrDefault();

            return defaultContractType != null ? 
                AttributedModelServices.GetTypeIdentity(defaultContractType) : 
                base.GetImportTypeIdentity(importConvention, member);
        }
    }
}