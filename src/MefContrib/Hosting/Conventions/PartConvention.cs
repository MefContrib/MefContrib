namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    /// <summary>
    /// Provides the functionality to define a convention which will be used to create parts.
    /// </summary>
    public class PartConvention : IPartConvention
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartConvention"/> class.
        /// </summary>
        public PartConvention()
        {
            this.Exports = new List<IExportConvention>();
            this.Imports = new List<IImportConvention>();
            this.Metadata = new List<MetadataItem>();
        }

        /// <summary>
        /// Gets or sets the condition used to idenfity the types that is used by the convention to create parts.
        /// </summary>
        /// <value>A <see cref="Predicate{T}"/> instance.</value>
        public Predicate<Type> Condition { get; set; }

        /// <summary>
        /// Gets or sets the creation policy of the created parts.
        /// </summary>
        /// <value>A <see cref="CreationPolicy"/> enum value.</value>
        public CreationPolicy CreationPolicy { get; set; }

        /// <summary>
        /// Gets or sets the exports that should be assigned to parts created by the convention.
        /// </summary>
        /// <value>An <see cref="IExportConvention"/> instance.</value>
        public IList<IExportConvention> Exports { get; set; }

        /// <summary>
        /// Gets or sets the imports that should be assigned to parts created by the convention.
        /// </summary>
        /// <value>An <see cref="IImportConvention"/> instance.</value>
        public IList<IImportConvention> Imports { get; set; }

        /// <summary>
        /// Gets or sets the metadata which should be associated with the part.
        /// </summary>
        /// <value>An <see cref="IList{T}"/> instance, containing <see cref="Metadata"/> objects.</value>
        public IList<MetadataItem> Metadata { get; set; }
    }
}