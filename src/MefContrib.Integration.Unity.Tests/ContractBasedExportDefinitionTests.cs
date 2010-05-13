using System;
using MefContrib.Integration.Exporters;
using NUnit.Framework;

namespace MefContrib.Integration.Unity.Tests
{
    [TestFixture]
    public class ContractBasedExportDefinitionTests
    {
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
            var exportDefinition = new ContractBasedExportDefinition(typeof(IUnityComponent), "ContractName");
            Assert.That(exportDefinition.ContractType, Is.EqualTo(typeof(IUnityComponent)));
            Assert.That(exportDefinition.RegistrationName, Is.EqualTo("ContractName"));
            Assert.That(exportDefinition.ContractName, Is.EqualTo("ContractName"));
        }

        [Test]
        public void ContractTypeAndNullNameAreProperlySetTest()
        {
            var exportDefinition = new ContractBasedExportDefinition(typeof(IUnityComponent));
            Assert.That(exportDefinition.ContractType, Is.EqualTo(typeof(IUnityComponent)));
            Assert.That(exportDefinition.RegistrationName, Is.Null);
            Assert.That(exportDefinition.ContractName, Is.EqualTo("MefContrib.Integration.Unity.Tests.IUnityComponent"));
        }
    }
}