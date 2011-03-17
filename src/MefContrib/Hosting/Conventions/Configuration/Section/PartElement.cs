using System.ComponentModel.Composition;

namespace MefContrib.Hosting.Conventions.Configuration.Section
{
    using System.Configuration;
    using System.Diagnostics;

    /// <summary>
    /// 
    /// </summary>
    public class PartElement : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartElement"/> class.
        /// </summary>
        public PartElement()
        {
        }

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
        /// Gets or sets the name of the type.
        /// </summary>
        /// <value>A string containing the name of the type.</value>
        [ConfigurationProperty("type", IsRequired = true),
            StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\")]
        public string Type
        {
            [DebuggerStepThrough]
            get { return this["type"] as string; }
            [DebuggerStepThrough]
            set { this["type"] = value; }
        }

        /// <summary>
        /// Gets or sets if default values are allowed.
        /// </summary>
        /// <value>A string containing the name of he contract.</value>
        /// <remarks>The default value is <see langword="false" />.</remarks>
        [ConfigurationProperty("creationPolicy", DefaultValue = CreationPolicy.Any, IsRequired = false)]
        public CreationPolicy CreationPolicy
        {
            [DebuggerStepThrough]
            get { return (CreationPolicy)this["creationPolicy"]; }
            [DebuggerStepThrough]
            set { this["creationPolicy"] = value; }
        }

        /// <summary>
        /// Gets a collection of all the metadata defined for the <see cref="ImportElement"/>.
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