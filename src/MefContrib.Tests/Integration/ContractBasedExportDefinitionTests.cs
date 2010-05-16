using System;
using MefContrib.Integration.Exporters;
using NUnit.Framework;

namespace MefContrib.Tests.Integration
{
    [TestFixture]
    public class ContractBasedExportDefinitionTests
    {
        public interface IComponent {}

        [Test]
        public void CannotPassNullTypeToTheCtorTest()
        {
            Assert.That(delegate
            {
                new ContractBasedExportDefinition(null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void ContractTypeAndExplicitNameAreProperlySetTest()
        {
            var exportDefinition = new ContractBasedExportDefinition(typeof(IComponent), "ContractName");
            Assert.That(exportDefinition.ContractType, Is.EqualTo(typeof(IComponent)));
            Assert.That(exportDefinition.RegistrationName, Is.EqualTo("ContractName"));
            Assert.That(exportDefinition.ContractName, Is.EqualTo("ContractName"));
        }

        [Test]
        public void ContractTypeAndNullNameAreProperlySetTest()
        {
            var exportDefinition = new ContractBasedExportDefinition(typeof(IComponent));
            Assert.That(exportDefinition.ContractType, Is.EqualTo(typeof(IComponent)));
            Assert.That(exportDefinition.RegistrationName, Is.Null);
            Assert.That(exportDefinition.ContractName, Is.EqualTo("MefContrib.Tests.Integration.ContractBasedExportDefinitionTests+IComponent"));
        }
    }
}