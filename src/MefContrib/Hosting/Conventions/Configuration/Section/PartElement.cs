namespace MefContrib.Hosting.Conventions.Configuration.Section
{
    using System.Configuration;
    using System.ComponentModel.Composition;
    using System.Diagnostics;

    /// <summary>
    /// Represents a configuration element for an export.
    /// </summary>
    public class PartElement : ConfigurationElement
    {
        /// <summary>
        /// Gets a collection of all the exports defined for the <see cref="PartElement"/>.
        /// </summary>
        /// <value>A <see cref="ExportElementCollection"/> object.</value>
        [ConfigurationProperty("exports", IsDefaultCollection = false, IsRequired = false)]
        public ExportElementCollection Exports
        {
            [DebuggerStepThrough]
            get { return (ExportElementCollection)base["exports"]; }
        }

        /// <summary>
        /// Gets a collection of all the imports defined for the <see cref="PartElement"/>.
        /// </summary>
        /// <value>A <see cref="ImportElementCollection"/> object.</value>
        [ConfigurationProperty("imports", IsDefaultCollection = false, IsRequired = false)]
        public ImportElementCollection Imports
        {
            [DebuggerStepThrough]
            get { return (ImportElementCollection)base["imports"]; }
        }

        /// <summary>
        /// Gets or sets the type of the part.
        /// </summary>
        /// <value>A string containing the type of the part.</value>
        [ConfigurationProperty("type", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\")]
        public string Type
        {
            [DebuggerStepThrough]
            get { return this["type"] as string; }

            [DebuggerStepThrough]
            set { this["type"] = value; }
        }

        /// <summary>
        /// Gets or sets the creation policy.
        /// </summary>
        [ConfigurationProperty("creationPolicy", DefaultValue = CreationPolicy.Any, IsRequired = false)]
        public CreationPolicy CreationPolicy
        {
            [DebuggerStepThrough]
            get { return (CreationPolicy)this["creationPolicy"]; }

            [DebuggerStepThrough]
            set { this["creationPolicy"] = value; }
        }

        /// <summary>
        /// Gets a collection of all the metadata defined for the <see cref="PartElement"/>.
        /// </summary>
        /// <value>A <see cref="MetadataElementCollection"/> object.</value>
        [ConfigurationProperty("metadata", IsDefaultCollection = false, IsRequired = false)]
        public MetadataElementCollection Metadata
        {
            [DebuggerStepThrough]
            get { return (MetadataElementCollection)base["metadata"]; }
        }
    }
}