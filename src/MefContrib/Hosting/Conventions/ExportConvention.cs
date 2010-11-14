namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides the functionality to define a convention which will be used to create exports.
    /// </summary>
    public class ExportConvention : IExportConvention, IEquatable<ExportConvention>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportConvention"/> class.
        /// </summary>
        public ExportConvention()
        {
            this.Metadata = new List<MetadataItem>();
        }

        /// <summary>
        /// Gets or sets the contract name used to identify the export.
        /// </summary>
        /// <value></value>
        public Func<MemberInfo, string> ContractName { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Type"/> of the contract.
        /// </summary>
        /// <value></value>
        public Func<MemberInfo, Type> ContractType { get; set; }

        /// <summary>
        /// Gets or sets a function that returns the members on a given type that should be treated as exports.
        /// </summary>
        /// <value>A <see cref="Func{T,TResult}"/> instance that returnes a collection of <see cref="MemberInfo"/> instanced base on a provided <see cref="Type"/>.</value>
        public Func<Type, MemberInfo[]> Members { get; set; }

        /// <summary>
        /// Gets or sets the metadata which should be associated with the export.
        /// </summary>
        /// <value>An <see cref="IList{T}"/> instance, containing <see cref="Metadata"/> objects.</value>
        public IList<MetadataItem> Metadata { get; set; }

        /// <summary>
        /// Indicates whether the current <see cref="ExportConvention"/> is equal to another <see cref="ExportConvention"/> of the same type.
        /// </summary>
        /// <param name="convention">An <see cref="ExportConvention"/> to compare with this <see cref="ExportConvention"/>.</param>
        /// <returns><see langword="true"/> if the current <see cref="ExportConvention"/> is equal to the <paramref name="convention"/> parameter; otherwise, <see langword="false"/>.</returns>
        public bool Equals(ExportConvention convention)
        {
            return (convention != null) &&
                   Equals(this.ContractName, convention.ContractName) &&
                   this.Metadata.SequenceEqual(convention.Metadata) &&
                   Equals(this.ContractType, convention.ContractType);
        }
    }
}