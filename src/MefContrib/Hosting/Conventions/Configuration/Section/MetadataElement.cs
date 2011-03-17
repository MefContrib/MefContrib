namespace MefContrib.Hosting.Conventions.Configuration.Section
{
    using System.Configuration;
    using System.Diagnostics;

    public class MetadataElement : ConfigurationElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportElement"/> class.
        /// </summary>
        public MetadataElement()
        {
        }

        /// <summary>
        /// Gets or sets the name of the metadata.
        /// </summary>
        /// <value>A string containing the name of the metadata.</value>
        [ConfigurationProperty("name", IsRequired = true),
            StringValidator(InvalidCharacters = " ~!@#$%^&*()[]{}/;'\"|\\")]
        public string Name
        {
            [DebuggerStepThrough]
            get { return this["name"] as string; }
            [DebuggerStepThrough]
            set { this["name"] = value; }
        }

        /// <summary>
        /// Gets or sets the value of the metadata.
        /// </summary>
        /// <value>A string containing the value of the metadata.</value>
        [ConfigurationProperty("value", IsRequired = false)]
        public string Value
        {
            [DebuggerStepThrough]
            get { return this["value"] as string; }
            [DebuggerStepThrough]
            set { this["value"] = value; }
        }

        /// <summary>
        /// Gets or sets the value of the metadata.
        /// </summary>
        /// <value>A string containing the value of the metadata.</value>
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            [DebuggerStepThrough]
            get { return this["type"] as string; }
            [DebuggerStepThrough]
            set { this["type"] = value; }
        }
    }
}