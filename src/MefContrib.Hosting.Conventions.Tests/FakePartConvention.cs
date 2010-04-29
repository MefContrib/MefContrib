namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using MefContrib.Hosting.Conventions;

    public class FakePartConvention : IPartConvention
    {
        public Predicate<Type> Condition { get; set; }
        public CreationPolicy CreationPolicy { get; set; }
        public IList<IExportConvention> Exports { get; set; }
        public IList<IImportConvention> Imports { get; set; }
        public IList<MetadataItem> Metadata { get; set; }
    }
}