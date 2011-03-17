namespace MefContrib.Hosting.Conventions.Configuration.Section
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Represents a collection of <see cref="ExportElement"/> instances.
    /// </summary>
    public class MetadataElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new MetadataElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var metadataElement = (MetadataElement) element;
            return metadataElement.Name;
        }
    }
}