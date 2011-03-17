namespace MefContrib.Hosting.Conventions.Configuration.Section
{
    using System;
    using System.Configuration;
    using System.Diagnostics;

    /// <summary>
    /// Represents a collection of <see cref="ImportElement"/> instances.
    /// </summary>
    public class ImportElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ImportElement();
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            [DebuggerStepThrough]
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            [DebuggerStepThrough]
            get { return "import"; }
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var part = (ImportElement)element;
            return part.Member;
        }
    }
}