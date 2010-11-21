using System;
using NUnit.Framework;

namespace MefContrib.Hosting.Tests
{
    [TestFixture]
    public class FactoryExportDefinitionTests
    {
        public interface IComponent {}

        [Test]
        public void Cannot_pass_null_type_to_the_ctor()
        {
            Assert.That(delegate
            {
                new FactoryExportDefinition(null, null, ep => null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Contract_type_and_names_are_properly_set()
        {
            var exportDefinition = new FactoryExportDefinition(typeof(IComponent), "ContractName", ep => null);
            Assert.That(exportDefinition.ContractType, Is.EqualTo(typeof(IComponent)));
            Assert.That(exportDefinition.RegistrationName, Is.EqualTo("ContractName"));
            Assert.That(exportDefinition.ContractName, Is.EqualTo("ContractName"));
        }

        [Test]
        public void When_passing_null_registration_name_the_contract_name_is_properly_set()
        {
            var exportDefinition = new FactoryExportDefinition(typeof(IComponent), null, ep => null);
            Assert.That(exportDefinition.ContractType, Is.EqualTo(typeof(IComponent)));
            Assert.That(exportDefinition.RegistrationName, Is.Null);
            Assert.That(exportDefinition.ContractName, Is.EqualTo("MefContrib.Hosting.Tests.FactoryExportDefinitionTests+IComponent"));
        }
    }
}