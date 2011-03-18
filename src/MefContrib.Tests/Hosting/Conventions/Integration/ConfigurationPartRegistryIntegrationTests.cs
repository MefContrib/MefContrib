using System.ComponentModel.Composition.Hosting;
using MefContrib.Hosting.Conventions.Configuration;
using NUnit.Framework;

namespace MefContrib.Hosting.Conventions.Tests.Integration
{
    [TestFixture]
    public class ConfigurationPartRegistryIntegrationTests
    {
        [Test]
        public void SimpleExport_part_is_properly_exported()
        {
            var registry = new ConfigurationPartRegistry("mef.configuration");
            var catalog = new ConventionCatalog(registry);
            var container = new CompositionContainer(catalog);

            var simpleExport = container.GetExportedValue<SimpleExport>();
            Assert.That(simpleExport, Is.Not.Null);
        }

        [Test]
        public void ExportWithPropertyImport_part_is_properly_exported_and_its_imports_are_satisfied()
        {
            var registry = new ConfigurationPartRegistry("mef.configuration");
            var catalog = new ConventionCatalog(registry);
            var container = new CompositionContainer(catalog);

            var exportWithPropertyImport = container.GetExportedValue<ExportWithPropertyImport>();
            Assert.That(exportWithPropertyImport, Is.Not.Null);
            Assert.That(exportWithPropertyImport.SimpleImport, Is.Not.Null);
        }
    }
}