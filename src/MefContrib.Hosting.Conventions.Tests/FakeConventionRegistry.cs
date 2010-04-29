namespace MefContrib.Hosting.Conventions.Tests
{
    using System.ComponentModel.Composition;
    using MefContrib.Hosting.Conventions.Configuration;

    public class FakeConventionRegistry : PartRegistry
    {
        public FakeConventionRegistry()
        {
            Part<PartConvention>()
                .ForTypesMatching(x => true)
                .MakeNonShared()
                .AddMetadata("Foo", "Bar")
                .Imports(i => i.Import<ImportConvention>()
                    .ContractName("Contract")
                    .ContractType<IImportConvention>()
                    .Members(x => new[] 
                    {
                        ReflectionServices.GetProperty<IImportConvention>(z => z.ContractName), 
                        ReflectionServices.GetProperty<IImportConvention>(z => z.ContractType) 
                    }))
                .Exports(e => e.Export<ExportConvention>()
                    .ContractName(x => AttributedModelServices.GetContractName(typeof(FakeExportConvention)))
                    .ContractType<IExportConvention>()
                    .Members(x => new[] 
                    {
                        ReflectionServices.GetProperty<IExportConvention>(z => z.ContractName),
                        ReflectionServices.GetProperty<IExportConvention>(z => z.ContractType) 
                    }));
        }
    }
}