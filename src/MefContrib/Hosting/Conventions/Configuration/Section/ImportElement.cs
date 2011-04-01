namespace MefContrib.Hosting.Conventions.Configuration.Section
{
    using System.ComponentModel.Composition;
    using System.Configuration;
    using System.Diagnostics;

    /// <summary>
    /// Represents a configuration element for an import.
    /// </summary>
    public class ImportElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets if default values are allowed.
        /// </summary>
        /// <value>A string containing the name of the contract.</value>
        /// <remarks>The default value is <see langword="false" />.</remarks>
        [ConfigurationProperty("allowDefault", DefaultValue = false, IsRequired = false)]
        public bool AllowDefault
        {
            [DebuggerStepThrough]
            get { return (bool)this["allowDefault"]; }

            [DebuggerStepThrough]
            set { this["allowDefault"] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the contract.
        /// </summary>
        /// <value>A string containing the name of the contract.</value>
        [ConfigurationProperty("contractName", IsRequired = false)]
        [StringValidator(InvalidCharacters = " ~!@#$%^&*()[]{}/;'\"|\\")]
        public string ContractName
        {
            [DebuggerStepThrough]
            get { return this["contractName"] as string; }

            [DebuggerStepThrough]
            set { this["contractName"] = value; }
        }

        /// <summary>
        /// Gets or sets the type of the contract.
        /// </summary>
        /// <value>A string containing the type of the contract.</value>
        [ConfigurationProperty("contractType", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\")]
        public string ContractType
        {
            [DebuggerStepThrough]
            get { return this["contractType"] as string; }

            [DebuggerStepThrough]
            set { this["contractType"] = value; }
        }

        /// <summary>
        /// Gets or sets required import policy.
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
        /// Gets or sets if the import is recomposable.
        /// </summary>
        [ConfigurationProperty("isRecomposable", DefaultValue = false, IsRequired = false)]
        public bool IsRecomposable
        {
            [DebuggerStepThrough]
            get { return (bool)this["isRecomposable"]; }

            [DebuggerStepThrough]
            set { this["isRecomposable"] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the member.
        /// </summary>
        /// <value>A string containing the name of the member.</value>
        [ConfigurationProperty("member", IsRequired = true)]
        [StringValidator(InvalidCharacters = " ~!@#$%^&*()[]{}/;'\"|\\")]
        public string Member
        {
            [DebuggerStepThrough]
            get { return this["member"] as string; }

            [DebuggerStepThrough]
            set { this["member"] = value; }
        }

        /// <summary>
        /// Gets a collection of all the metadata defined for the <see cref="ImportElement"/>.
        /// </summary>
        /// <value>A <see cref="MetadataElementCollection"/> object.</value>
        [ConfigurationProperty("required-metadata", IsDefaultCollection = false, IsRequired = false)]
        public MetadataElementCollection RequiredMetadata
        {
            [DebuggerStepThrough]
            get { return (MetadataElementCollection)base["required-metadata"]; }
        }
    }
}