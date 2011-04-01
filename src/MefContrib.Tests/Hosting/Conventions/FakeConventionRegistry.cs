namespace MefContrib.Hosting.Conventions.Tests
{
    using System.ComponentModel.Composition;
    using MefContrib.Hosting.Conventions.Configuration;

    public class FakeConventionRegistry : PartRegistry
    {
        public FakeConventionRegistry()
        {
            this.TypeScanner = new TypeScanner();

            PartWithConvention<PartConvention>()
                .ForTypesMatching(x => true)
                .MakeNonShared()
                .AddMetadata("Foo", "Bar")
                .Imports(i => i.ImportWithConvention<ImportConvention>()
                    .ContractName("Contract")
                    .ContractType<IImportConvention>()
                    .Members(x => new[] 
                    {
                        ReflectionServices.GetProperty<IImportConvention>(z => z.ContractName), 
                        ReflectionServices.GetProperty<IImportConvention>(z => z.ContractType) 
                    }))
                .Exports(e => e.ExportWithConvention<ExportConvention>()
                    .ContractName(x => AttributedModelServices.GetContractName(typeof(FakeExportConvention)))
                    .ContractType<IExportConvention>()
                    .Members(x => new[] 
                    {
                        ReflectionServices.GetProperty<IExportConvention>(z => z.ContractName),
                        ReflectionServices.GetProperty<IExportConvention>(z => z.ContractType) 
                    }));

            PartWithConvention<PartConvention>()
                .ForTypesMatching(x => x.Equals(typeof(FakePart)));

            PartWithConvention<PartConvention>()
                .ForTypesMatching(x => false);
        }
    }
}