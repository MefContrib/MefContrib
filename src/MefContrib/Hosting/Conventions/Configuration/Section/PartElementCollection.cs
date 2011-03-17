namespace MefContrib.Hosting.Conventions.Configuration.Section
{
    using System.Configuration;

    /// <summary>
    /// Represents a collection of <see cref="PartElement"/> instances.
    /// </summary>
    public class PartElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new PartElement();
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get { return "part"; }
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var part = (PartElement) element;
            return part.Type;
        }
    }
}
