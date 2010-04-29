namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Reflection;

    public class FakeImportConvetion : IImportConvention
    {
        public bool AllowDefaultValue { get; set; }
        public Func<MemberInfo, string> ContractName { get; set; }
        public Func<MemberInfo, Type> ContractType { get; set; }
        public CreationPolicy CreationPolicy { get; set; }
        public Func<Type, MemberInfo[]> Members { get; set; }
        public bool Recomposable { get; set; }
        public IList<RequiredMetadataItem> RequiredMetadata { get; set; }
    }
}