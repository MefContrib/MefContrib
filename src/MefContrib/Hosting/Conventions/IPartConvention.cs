namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    /// <summary>
    /// Defines the functionality of a convention used to define parts.
    /// </summary>
    public interface IPartConvention
    {
        /// <summary>
        /// Gets or sets the condition used to idenfity the types that is used by the convention to create parts.
        /// </summary>
        /// <value>A <see cref="Predicate{T}"/> instance.</value>
        Predicate<Type> Condition { get; set; }

        /// <summary>
        /// Gets or sets the creation policy of the created parts.
        /// </summary>
        /// <value>A <see cref="CreationPolicy"/> enum value.</value>
        CreationPolicy CreationPolicy { get; set; }

        /// <summary>
        /// Gets or sets the exports that should be assigned to parts created by the convention.
        /// </summary>
        /// <value>An <see cref="IExportConvention"/> instance.</value>
        IList<IExportConvention> Exports { get; set; }

        /// <summary>
        /// Gets or sets the imports that should be assigned to parts created by the convention.
        /// </summary>
        /// <value>An <see cref="IImportConvention"/> instance.</value>
        IList<IImportConvention> Imports { get; set; }

        /// <summary>
        /// Gets or sets the metadata which should be associated with the part.
        /// </summary>
        /// <value>An <see cref="IList{T}"/> instance, containing <see cref="Metadata"/> objects.</value>
        IList<MetadataItem> Metadata { get; set; }
    }
}