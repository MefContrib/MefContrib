using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using MefContrib.Tests;
using Moq;
using NUnit.Framework;

namespace MefContrib.Interception.Tests
{
    namespace Given_an_InterceptingComposablePartDefinition
    {
        [TestFixture]
        public class when_accessing_import_definitions
            : InterceptingComposablePartDefinitionContext
        {
            [Test]
            public void it_should_pull_from_the_inner_part_definition()
            {
                InterceptingPartDefinition.ImportDefinitions.ShouldEqual(InterceptedPartDefinition.ImportDefinitions);
            }
        }

        [TestFixture]
        public class when_accessing_export_definitions 
            : InterceptingComposablePartDefinitionContext
        {
            [Test]
            public void it_should_pull_from_the_inner_part_definition()
            {
                InterceptingPartDefinition.ExportDefinitions.ShouldEqual(InterceptedPartDefinition.ExportDefinitions);
            }
        }
        
        [TestFixture]
        public class when_calling_create
            : InterceptingComposablePartDefinitionContext
        {
            [Test]
            public void it_should_create_an_intercepting_part()
            {
                part.ShouldBeOfType<InterceptingComposablePart>();
            }

            public override void Context()
            {
                part = InterceptingPartDefinition.CreatePart();
            }

            private ComposablePart part;
        }


        public class InterceptingComposablePartDefinitionContext
        {
            public InterceptingComposablePartDefinitionContext()
            {
                InterceptedPartDefinition = AttributedModelServices.CreatePartDefinition(typeof(OrderProcessor), null);
                MockInterceptor = new Mock<IExportedValueInterceptor>();
                InterceptingPartDefinition = new InterceptingComposablePartDefinition(InterceptedPartDefinition, MockInterceptor.Object);
                Context();
            }

            public virtual void Context()
            {
            }

            public ComposablePartDefinition InterceptedPartDefinition;
            public InterceptingComposablePartDefinition InterceptingPartDefinition;
            public Mock<IExportedValueInterceptor> MockInterceptor;
        }

    }
}
