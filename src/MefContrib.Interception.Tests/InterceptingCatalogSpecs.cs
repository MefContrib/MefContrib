using System;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.ComponentModel.Composition.Hosting;
using Composition.Interception;
using MefContrib.Interception.Tests;
using MefContrib.Tests;
using Moq;
using NUnit.Framework;

namespace MefContrib.Interception.Tests
{
    namespace Given_an_InterceptingCatalog
    {
        [TestFixture]
        public class when_querying_for_a_part : InterceptingCatalogContext
        {
            [Test]
            public void it_should_return_an_intercepting_part_definition()
            {
                PartDefinition.ShouldBeOfType<InterceptingComposablePartDefinition>();
            }

            public override void Context()
            {
                PartDefinition = Catalog.Parts.First();
            }
        }

        public class InterceptingCatalogContext
        {
            public InterceptingCatalog Catalog;
            public Mock<IExportedValueInterceptor> MockInterceptor;
            public ComposablePartDefinition PartDefinition;

            public InterceptingCatalogContext()
            {
                var innerCatalog = new TypeCatalog(typeof(Logger));
                MockInterceptor = new Mock<IExportedValueInterceptor>();
                Catalog = new InterceptingCatalog(innerCatalog, MockInterceptor.Object);
                Context();
            }

            public virtual void Context()
            {
            }
        }

    }
}