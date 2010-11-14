using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using MefContrib.Tests;
using Moq;
using NUnit.Framework;

namespace MefContrib.Hosting.Interception.Tests
{
    [TestFixture]
    public class InterceptingComposablePartDefinitionTests
    {
        private ComposablePartDefinition interceptedPartDefinition;
        private InterceptingComposablePartDefinition interceptingPartDefinition;

        [SetUp]
        public void TestSetUp()
        {
            var mockInterceptor = new Mock<IExportedValueInterceptor>();
            interceptedPartDefinition = AttributedModelServices.CreatePartDefinition(typeof(OrderProcessor), null);    
            interceptingPartDefinition = new InterceptingComposablePartDefinition(interceptedPartDefinition, mockInterceptor.Object);
        }

        [Test]
        public void Ctor_should_throw_argument_null_exception_if_called_with_null_composable_part_definition()
        {
            Assert.That(delegate
            {
                new InterceptingComposablePartDefinition(null, new FakeInterceptor());
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Ctor_should_throw_argument_null_exception_if_called_with_null_interceptor()
        {
            var partDefinition = new TypeCatalog(typeof (Part1)).Parts.First();
            Assert.That(delegate
            {

                new InterceptingComposablePartDefinition(partDefinition, null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void When_accessing_import_definitions_it_should_pull_from_the_inner_part_definition()
        {
            interceptingPartDefinition.ImportDefinitions.ShouldEqual(interceptedPartDefinition.ImportDefinitions);
        }

        [Test]
        public void When_accessing_export_definitions_it_should_pull_from_the_inner_part_definition()
        {
            interceptingPartDefinition.ExportDefinitions.ShouldEqual(interceptedPartDefinition.ExportDefinitions);
        }

        [Test]
        public void When_accessing_metadat_it_should_pull_from_the_inner_part_definition()
        {
            interceptingPartDefinition.Metadata.ShouldEqual(interceptedPartDefinition.Metadata);
        }

        [Test]
        public void When_calling_create_part_it_should_create_an_intercepting_part()
        {
            var part = interceptingPartDefinition.CreatePart();
            part.ShouldBeOfType<InterceptingComposablePart>();
        }

        [Test]
        public void When_calling_create_part_it_should_create_a_disposable_intercepting_part()
        {
            var mockInterceptor = new Mock<IExportedValueInterceptor>();
            interceptedPartDefinition = AttributedModelServices.CreatePartDefinition(typeof(DisposablePart), null);
            interceptingPartDefinition = new InterceptingComposablePartDefinition(interceptedPartDefinition, mockInterceptor.Object);
            var part = interceptingPartDefinition.CreatePart();
            part.ShouldBeOfType<DisposableInterceptingComposablePart>();
        }
    }
}
