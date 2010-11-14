namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Reflection;

    /// <summary>
    /// Defines the functionality of a convention used to define imports.
    /// </summary>
    public interface IImportConvention
    {
        /// <summary>
        /// Gets or sets a value indicating whether default values are allowed for the importing member.
        /// </summary>
        /// <value><see langword="true"/> if the import is allowed to have default values; otherwise, <see langword="false"/>.</value>
        bool AllowDefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the name of the contract.
        /// </summary>
        /// <value></value>
        Func<MemberInfo, string> ContractName { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Type"/> of the contract.
        /// </summary>
        /// <value></value>
        Func<MemberInfo, Type> ContractType { get; set; }

        /// <summary>
        /// Gets or sets the creation policy of the import.
        /// </summary>
        /// <value>A <see cref="CreationPolicy"/> enum value.</value>
        CreationPolicy CreationPolicy { get; set; }

        /// <summary>
        /// Gets or sets a function that returns the members on a given type that should be treated as imports.
        /// </summary>
        /// <value>A <see cref="Func{T,TResult}"/> instance that returnes a collection of <see cref="MemberInfo"/> instanced base on a provided <see cref="Type"/>.</value>
        Func<Type, MemberInfo[]> Members { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the import can be recomposed.
        /// </summary>
        /// <value><see langword="true"/> if the import can be recomposed; otherwise, <see langword="false"/>.</value>
        bool Recomposable { get; set; }

        /// <summary>
        /// Gets or sets the required metadata items.
        /// </summary>
        /// <value>A <see cref="IList{T}"/> containing <see cref="RequiredMetadataItem"/> instances.</value>
        IList<RequiredMetadataItem> RequiredMetadata { get; set; }
    }
}