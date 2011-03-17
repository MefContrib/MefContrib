namespace MefContrib.Hosting.Conventions.Configuration.Section
{
    using System.Configuration;
    using System.Diagnostics;

    /// <summary>
    /// Represents configuration section for configuring parts for <see cref="ConventionCatalog"/>.
    /// </summary>
    public class ConventionConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// Gets a collection of <see cref="PartElement"/> instances.
        /// </summary>
        [ConfigurationProperty("parts", IsDefaultCollection = true)]
        public PartElementCollection Parts
        {
            [DebuggerStepThrough]
            get { return (PartElementCollection)base["parts"]; }
        }
    }
}