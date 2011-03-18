namespace MefContrib.Hosting.Conventions.Tests.Integration
{
    public class SimpleExport
    {
        
    }

    public class ExportWithPropertyImport
    {
        public SimpleExport SimpleImport { get; set; }
    }
}