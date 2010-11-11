namespace MefContrib.Hosting.Interception.Handlers
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    [InheritedExport]
    public interface IGenericContractRegistry
    {
        IEnumerable<GenericContractTypeMapping> GetMappings();
    }
}