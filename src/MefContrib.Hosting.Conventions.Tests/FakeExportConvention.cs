namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class FakeExportConvention : IExportConvention
    {
        public Func<MemberInfo, string> ContractName { get; set; }
        public Func<MemberInfo, Type> ContractType { get; set; }
        public Func<Type, MemberInfo[]> Members { get; set; }
        public IList<MetadataItem> Metadata { get; set; }
    }
}