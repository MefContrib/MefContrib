using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using MefContrib.Hosting.Conventions.Configuration;
using NUnit.Framework;

namespace MefContrib.Hosting.Conventions.Tests
{
    [TestFixture]
    public class ConfigurationPartRegistryTests
    {
        [Test]
        public void FakePart_is_exported_using_xml_configuration()
        {
            var registry = new ConfigurationPartRegistry("mef.configuration");
            var catalog = new ConventionCatalog(registry);

            var parts = new List<ComposablePartDefinition>(catalog.Parts);
            Assert.That(parts.Count, Is.Not.EqualTo(0));

            var exports = new List<ExportDefinition>(parts[0].ExportDefinitions);
            Assert.That(exports.Count, Is.EqualTo(1));

            var imports = new List<ImportDefinition>(parts[0].ImportDefinitions);
            Assert.That(imports.Count, Is.EqualTo(1));
            Assert.That(imports[0].ContractName, Is.EqualTo("somestring"));
            Assert.That(imports[0].IsRecomposable, Is.EqualTo(false));
            Assert.That(imports[0].IsPrerequisite, Is.EqualTo(false));
        }
    }
}