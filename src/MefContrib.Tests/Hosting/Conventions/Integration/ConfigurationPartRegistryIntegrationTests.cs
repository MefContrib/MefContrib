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

        [Test]
        public void SimpleExport_part_with_metadata_is_properly_exported()
        {
            var registry = new ConfigurationPartRegistry("mef.configuration");
            var catalog = new ConventionCatalog(registry);
            var container = new CompositionContainer(catalog);

            var simpleExport = container.GetExport<SimpleExportWithMetadata, ISimpleMetadata>("simple-export");
            Assert.That(simpleExport, Is.Not.Null);
            Assert.That(simpleExport.Metadata.IntValue, Is.EqualTo(1234));
            Assert.That(simpleExport.Metadata.StrValue, Is.EqualTo("some string"));
        }

        [Test]
        public void SimpleContract_part_with_metadata_is_properly_imported()
        {
            var registry = new ConfigurationPartRegistry("mef.configuration");
            var catalog = new ConventionCatalog(registry);
            var container = new CompositionContainer(catalog);

            var simpleExport = container.GetExport<SimpleContractImporter>();
            Assert.That(simpleExport, Is.Not.Null);
            Assert.That(simpleExport.Value.SimpleContracts, Is.Not.Null);
            Assert.That(simpleExport.Value.SimpleContracts.Length, Is.EqualTo(1));
            Assert.That(simpleExport.Value.SimpleContracts[0].Metadata.IntValue, Is.EqualTo(1234));
            Assert.That(simpleExport.Value.SimpleContracts[0].Metadata.StrValue, Is.EqualTo("some string"));

            Assert.That(simpleExport.Value.SimpleContractsNoMetadataInterface, Is.Not.Null);
            Assert.That(simpleExport.Value.SimpleContractsNoMetadataInterface.Length, Is.EqualTo(1));
            Assert.That(simpleExport.Value.SimpleContractsNoMetadataInterface[0].GetType(), Is.EqualTo(typeof(SimpleContract1)));
        }
    }
}