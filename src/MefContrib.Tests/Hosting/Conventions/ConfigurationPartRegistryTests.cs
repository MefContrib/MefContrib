using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using MefContrib.Hosting.Conventions.Configuration;
using MefContrib.Hosting.Conventions.Configuration.Section;
using NUnit.Framework;

namespace MefContrib.Hosting.Conventions.Tests
{
    [TestFixture]
    public class ConfigurationPartRegistryTests
    {
        [Test]
        public void Invoking_ctor_with_null_string_causes_an_exception()
        {
            Assert.That(delegate
            {
                new ConfigurationPartRegistry((string)null);
            }, Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void Invoking_ctor_with_null_section_causes_an_exception()
        {
            Assert.That(delegate
            {
                new ConfigurationPartRegistry((ConventionConfigurationSection)null);
            }, Throws.InstanceOf<ArgumentNullException>());
        }

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