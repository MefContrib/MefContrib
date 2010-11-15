using System;
using MefContrib.Hosting;
using NUnit.Framework;

namespace MefContrib.Hosting.Tests
{
    [TestFixture]
    public class FactoryExportDefinitionTests
    {
        public interface IComponent {}

        [Test]
        public void CannotPassNullTypeToTheCtorTest()
        {
            Assert.That(delegate
            {
                new FactoryExportDefinition(null, null, ep => null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void ContractTypeAndExplicitNameAreProperlySetTest()
        {
            var exportDefinition = new FactoryExportDefinition(typeof(IComponent), "ContractName", ep => null);
            Assert.That(exportDefinition.ContractType, Is.EqualTo(typeof(IComponent)));
            Assert.That(exportDefinition.RegistrationName, Is.EqualTo("ContractName"));
            Assert.That(exportDefinition.ContractName, Is.EqualTo("ContractName"));
        }

        [Test]
        public void ContractTypeAndNullNameAreProperlySetTest()
        {
            var exportDefinition = new FactoryExportDefinition(typeof(IComponent), null, ep => null);
            Assert.That(exportDefinition.ContractType, Is.EqualTo(typeof(IComponent)));
            Assert.That(exportDefinition.RegistrationName, Is.Null);
            Assert.That(exportDefinition.ContractName, Is.EqualTo("MefContrib.Hosting.Tests.FactoryExportDefinitionTests+IComponent"));
        }
    }
}