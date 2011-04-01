namespace MefContrib.Hosting.Conventions.Configuration.Section
{
    using System.Configuration;
    using System.Diagnostics;

    /// <summary>
    /// Represents a configuration element for an export.
    /// </summary>
    public class ExportElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the name of the contract.
        /// </summary>
        [ConfigurationProperty("contractName", IsRequired = false)]
        [StringValidator(InvalidCharacters = " ~!@#$%^&*()[]{}/;'\"|\\")]
        public string ContractName
        {
            [DebuggerStepThrough]
            get { return (string) this["contractName"]; }
            
            [DebuggerStepThrough]
            set { this["contractName"] = value; }
        }

        /// <summary>
        /// Gets or sets the type of the contract.
        /// </summary>
        [ConfigurationProperty("contractType", DefaultValue = null, IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\")]
        public string ContractType
        {
            [DebuggerStepThrough]
            get { return this["contractType"] as string; }

            [DebuggerStepThrough]
            set { this["contractType"] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the member.
        /// </summary>
        [ConfigurationProperty("member", DefaultValue = null, IsRequired = false)]
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
        [ConfigurationProperty("metadata", IsDefaultCollection = false, IsRequired = false)]
        public MetadataElementCollection Metadata
        {
            [DebuggerStepThrough]
            get { return (MetadataElementCollection)base["metadata"]; }
        }
    }
}