using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
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
    }
}
