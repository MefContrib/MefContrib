using System;
using System.Collections.Generic;
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
    public class _InterceptingComposablePartTests
    {
        [Test]
        public void Ctor_should_throw_argument_null_exception_if_called_with_null_composable_part()
        {
            Assert.That(delegate
            {
                new InterceptingComposablePart(null, new FakeInterceptor());
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Ctor_should_throw_argument_null_exception_if_called_with_null_interceptor()
        {
            var partDefinition = new TypeCatalog(typeof(Part1)).Parts.First();
            Assert.That(delegate
            {

                new InterceptingComposablePart(partDefinition.CreatePart(), null);
            }, Throws.TypeOf<ArgumentNullException>());
        }
    }

    namespace InterceptingComposablePartTests
    {
        [TestFixture]
        public class When_accessing_import_definitions
            : InterceptingComposablePartContext
        {
            [Test]
            public void It_should_pull_from_the_inner_part()
            {
                InterceptingPart.ImportDefinitions.ShouldEqual(InterceptedPart.ImportDefinitions);
            }
        }

        [TestFixture]
        public class When_accessing_export_definitions
            : InterceptingComposablePartContext
        {
            [Test]
            public void It_should_pull_from_the_inner_part()
            {
                InterceptingPart.ExportDefinitions.ShouldEqual(InterceptedPart.ExportDefinitions);
            }
        }

        [TestFixture]
        public class When_accessing_metadata
            : InterceptingComposablePartContext
        {
            [Test]
            public void It_should_pull_from_the_inner_part()
            {
                InterceptingPart.Metadata.ShouldEqual(InterceptedPart.Metadata);
            }
        }

        [TestFixture]
        public class When_setting_an_import
            : InterceptingComposablePartContext
        {
            [Test]
            public void It_should_set_the_import_on_intercepted_part()
            {
                InterceptedPart = MockPart.Object;
                InterceptingPart = new InterceptingComposablePart(InterceptedPart, MockInterceptor.Object);
                InterceptingPart.SetImport(LoggerImportDefinition, Exports);

                MockPart.Verify(p => p.SetImport(LoggerImportDefinition, Exports));
            }
        }

        [TestFixture]
        public class When_retrieving_an_exported_value
            : InterceptingComposablePartContext
        {
            [Test]
            public void It_should_pass_the_value_to_the_value_interceptor()
            {
                InterceptingPart.GetExportedValue(OrderProcessorExportDefinition);
                MockInterceptor.Verify(p => p.Intercept(interceptedOrderProcessor));
            }

            [Test]
            public void It_should_return_the_intercepted_value()
            {
                var retrievedOrderProcessor = InterceptingPart.GetExportedValue(OrderProcessorExportDefinition);
                retrievedOrderProcessor.ShouldEqual(interceptingOrderProcessor);
            }

            public override void TestSetUp()
            {
                MockPart.Setup(p => p.GetExportedValue(OrderProcessorExportDefinition)).Returns(interceptedOrderProcessor);
                InterceptedPart = MockPart.Object;
                MockInterceptor.Setup(p => p.Intercept(interceptedOrderProcessor)).Returns(interceptingOrderProcessor);
                InterceptingPart = new InterceptingComposablePart(InterceptedPart, MockInterceptor.Object);
            }

            private readonly OrderProcessor interceptingOrderProcessor = new OrderProcessor();
            private readonly OrderProcessor interceptedOrderProcessor = new OrderProcessor();
        }

        [TestFixture]
        public class When_retrieving_an_exported_value_twice
            : InterceptingComposablePartContext
        {

            [Test]
            public void It_should_only_invoke_the_interceptor_once()
            {
                MockInterceptor.Verify(p => p.Intercept(interceptedOrderProcessor), Times.Once());
            }

            [Test]
            public void It_should_return_the_intercepted_value()
            {
                var retrievedOrderProcessor = InterceptingPart.GetExportedValue(OrderProcessorExportDefinition);
                retrievedOrderProcessor.ShouldEqual(interceptingOrderProcessor);
            }

            public override void TestSetUp()
            {
                MockPart.Setup(p => p.GetExportedValue(OrderProcessorExportDefinition)).Returns(interceptedOrderProcessor);
                InterceptedPart = MockPart.Object;
                InterceptingPart = new InterceptingComposablePart(InterceptedPart, MockInterceptor.Object);
                MockInterceptor.Setup(p => p.Intercept(interceptedOrderProcessor)).Returns(interceptingOrderProcessor);
                InterceptingPart.GetExportedValue(OrderProcessorExportDefinition);
            }

            private readonly OrderProcessor interceptingOrderProcessor = new OrderProcessor();
            private readonly OrderProcessor interceptedOrderProcessor = new OrderProcessor();
        }

        public abstract class InterceptingComposablePartContext
        {
            protected InterceptingComposablePartContext()
            {
                MockPart = new Mock<ComposablePart>();
                InterceptedPart = AttributedModelServices.CreatePart(new OrderProcessor());
                LoggerImportDefinition = InterceptedPart.ImportDefinitions.First();
                OrderProcessorExportDefinition = InterceptedPart.ExportDefinitions.First();
                MockInterceptor = new Mock<IExportedValueInterceptor>();
                Exports = new List<Export> { new Export(OrderProcessorExportDefinition, () => new Logger()) };
                InterceptingPart = new InterceptingComposablePart(InterceptedPart, MockInterceptor.Object);
            }

            [SetUp]
            public virtual void TestSetUp()
            {
            }

            protected ComposablePart InterceptedPart;
            protected InterceptingComposablePart InterceptingPart;

            protected readonly ImportDefinition LoggerImportDefinition;
            protected readonly ExportDefinition OrderProcessorExportDefinition;
            protected readonly IEnumerable<Export> Exports;
            protected readonly Mock<IExportedValueInterceptor> MockInterceptor;
            protected readonly Mock<ComposablePart> MockPart;
        }
    }
}
