using System;

namespace MefContrib.Hosting.Conventions.Tests.Integration
{
    public class SimpleExport
    {
        
    }

    public class ExportWithPropertyImport
    {
        public SimpleExport SimpleImport { get; set; }
    }

    public class SimpleExportWithMetadata
    {

    }

    public interface ISimpleMetadata
    {
        int IntValue { get; }

        string StrValue { get; }
    }

    public interface ISimpleContract { }

    public class SimpleContract1 : ISimpleContract { }

    public class SimpleContract2 : ISimpleContract { }

    public class SimpleContractImporter
    {
        public Lazy<ISimpleContract, ISimpleMetadata>[] SimpleContracts { get; set; }

        public ISimpleContract[] SimpleContractsNoMetadataInterface { get; set; }
    }

}