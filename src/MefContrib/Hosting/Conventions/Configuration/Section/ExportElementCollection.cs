namespace MefContrib.Hosting.Conventions.Configuration.Section
{
    using System;
    using System.Configuration;
    using System.Diagnostics;

    /// <summary>
    /// Represents a collection of <see cref="ExportElement"/> instances.
    /// </summary>
    public class ExportElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ExportElement();
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            [DebuggerStepThrough]
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            [DebuggerStepThrough]
            get { return "export"; }
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var part = (ExportElement) element;
            return part.Member;
        }
    }
}