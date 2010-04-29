namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides the functionality to define a convention which will be used to create imports.
    /// </summary>
    public class ImportConvention : IImportConvention, IEquatable<ImportConvention>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportConvention"/> class.
        /// </summary>
        public ImportConvention()
        {
            this.RequiredMetadata = new List<RequiredMetadataItem>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether default values are allowed for the importing member.
        /// </summary>
        /// <value><see langword="true"/> if the import is allowed to have default values; otherwise, <see langword="false"/>.</value>
        public bool AllowDefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the contract name used to identify the import.
        /// </summary>
        /// <value></value>
        public Func<MemberInfo, string> ContractName { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Type"/> that defines the context of the <see cref="ContractName"/>.
        /// </summary>
        /// <value></value>
        public Func<MemberInfo, Type> ContractType { get; set; }

        /// <summary>
        /// Gets or sets the creation policy of the import.
        /// </summary>
        /// <value>A <see cref="CreationPolicy"/> enum value.</value>
        public CreationPolicy CreationPolicy { get; set; }

        /// <summary>
        /// Gets or sets a function that returns the members on a given type that should be treated as imports.
        /// </summary>
        /// <value>A <see cref="Func{T,TResult}"/> instance that returnes a collection of <see cref="MemberInfo"/> instanced base on a provided <see cref="Type"/>.</value>
        public Func<Type, MemberInfo[]> Members { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the import can be recomposed.
        /// </summary>
        /// <value><see langword="true"/> if the import can be recomposed; otherwise, <see langword="false"/>.</value>
        public bool Recomposable { get; set; }

        /// <summary>
        /// Gets or sets the required metadata items.
        /// </summary>
        /// <value>A <see cref="IEnumerable{T}"/> containing <see cref="RequiredMetadataItem"/> instances.</value>
        public IList<RequiredMetadataItem> RequiredMetadata { get; set; }
        
        /// <summary>
        /// Indicates whether the current <see cref="ImportConvention"/> is equal to another <see cref="ImportConvention"/> of the same type.
        /// </summary>
        /// <param name="convention">An <see cref="ImportConvention"/> to compare with this <see cref="ImportConvention"/>.</param>
        /// <returns><see langword="true"/> if the current <see cref="ImportConvention"/> is equal to the <paramref name="convention"/> parameter; otherwise, <see langword="false"/>.</returns>
        public bool Equals(ImportConvention convention)
        {
            if (ReferenceEquals(this, convention))
            {
                return true;
            }

            return (convention != null) &&
                   Equals(this.ContractName, convention.ContractName) &&
                   Equals(this.CreationPolicy, convention.CreationPolicy) &&
                   Equals(this.ContractType, convention.ContractType) &&
                   Equals(this.Recomposable, convention.Recomposable) &&
                   this.RequiredMetadata.SequenceEqual(convention.RequiredMetadata);
        }
    }
}