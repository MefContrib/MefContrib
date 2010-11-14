namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Defines the functionality of a convention used to define exports.
    /// </summary>
    public interface IExportConvention
    {
        /// <summary>
        /// Gets or sets the contract name used to identify the export.
        /// </summary>
        /// <value></value>
        Func<MemberInfo, string> ContractName { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Type"/> of the contract.
        /// </summary>
        /// <value></value>
        Func<MemberInfo, Type> ContractType { get; set; }
        
        /// <summary>
        /// Gets or sets a function that returns the members on a given type that should be treated as exports.
        /// </summary>
        /// <value>A <see cref="Func{T,TResult}"/> instance that returnes a collection of <see cref="MemberInfo"/> instanced base on a provided <see cref="Type"/>.</value>
        Func<Type, MemberInfo[]> Members { get; set; }

        /// <summary>
        /// Gets or sets the metadata which should be associated with the export.
        /// </summary>
        /// <value>An <see cref="IList{T}"/> instance, containing <see cref="Metadata"/> objects.</value>
        IList<MetadataItem> Metadata { get; set; }
    }
}