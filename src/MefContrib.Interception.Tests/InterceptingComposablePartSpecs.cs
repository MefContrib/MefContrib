using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using MefContrib.Interception.Tests;
using MefContrib.Tests;
using Moq;
using NUnit.Framework;

namespace MefContrib.Interception.Tests
{
    namespace Given_an_InterceptingComposablePart
    {
        [TestFixture]
        public class when_accessing_import_definitions
            : InterceptingComposablePartContext
        {
            [Test]
            public void it_should_pull_from_the_inner_part()
            {
                InterceptingPart.ImportDefinitions.ShouldEqual(InterceptedPart.ImportDefinitions);
            }
        }

        [TestFixture]
        public class when_accessing_export_definitions 
            : InterceptingComposablePartContext
        {
            [Test]
            public void it_should_pull_from_the_inner_part()
            {
                InterceptingPart.ExportDefinitions.ShouldEqual(InterceptedPart.ExportDefinitions);
            }
        }

        [TestFixture]
        public class when_setting_an_import
            : InterceptingComposablePartContext
        {
            [Test]
            public void it_should_set_the_import_on_intercepted_part()
            {
                MockPart.Verify(p => p.SetImport(LoggerImportDefinition, Exports));
            }

            public override void Context()
            {
                InterceptedPart = MockPart.Object;
                InterceptingPart = new InterceptingComposablePart(InterceptedPart, MockInterceptor.Object);
                InterceptingPart.SetImport(LoggerImportDefinition, Exports);
            }
        }

        [TestFixture]
        public class when_retrieving_an_exported_value
            : InterceptingComposablePartContext
        {
            [Test]
            public void it_should_pass_the_value_to_the_value_interceptor()
            {
                MockInterceptor.Verify(p => p.Intercept(interceptedOrderProcessor));
            }

            [Test]
            public void it_should_return_the_intercepted_value()
            {
                retrievedOrderProcessor.ShouldEqual(interceptingOrderProcessor);
            }

            public override void Context()
            {
                interceptedOrderProcessor = new OrderProcessor();
                MockPart.Setup(p => p.GetExportedValue(OrderProcessorExportDefinition)).Returns(
                    interceptedOrderProcessor);
                InterceptedPart = MockPart.Object;
                MockInterceptor.Setup(p => p.Intercept(interceptedOrderProcessor)).Returns(interceptingOrderProcessor);
                InterceptingPart = new InterceptingComposablePart(InterceptedPart, MockInterceptor.Object);
                retrievedOrderProcessor = InterceptingPart.GetExportedValue(OrderProcessorExportDefinition);
            }

            private OrderProcessor interceptingOrderProcessor = new OrderProcessor();
            private OrderProcessor interceptedOrderProcessor = new OrderProcessor();
            private object retrievedOrderProcessor;
        }

        [TestFixture]
        public class when_retrieving_an_exported_value_twice
            : InterceptingComposablePartContext
        {
           
            [Test]
            public void it_should_only_invoke_the_interceptor_once()
            {
                MockInterceptor.Verify(p => p.Intercept(interceptedOrderProcessor), Times.Once());
            }

            [Test]
            public void it_should_return_the_intercepted_value()
            {
                retrievedOrderProcessor.ShouldEqual(interceptingOrderProcessor);
            }

            public override void Context()
            {
                interceptedOrderProcessor = new OrderProcessor();
                MockPart.Setup(p => p.GetExportedValue(OrderProcessorExportDefinition)).Returns(
                    interceptedOrderProcessor);
                InterceptedPart = MockPart.Object;
                InterceptingPart = new InterceptingComposablePart(InterceptedPart, MockInterceptor.Object);
                MockInterceptor.Setup(p => p.Intercept(interceptedOrderProcessor)).Returns(interceptingOrderProcessor);
                InterceptingPart.GetExportedValue(OrderProcessorExportDefinition);
                retrievedOrderProcessor = InterceptingPart.GetExportedValue(OrderProcessorExportDefinition);

            }

            private OrderProcessor interceptingOrderProcessor = new OrderProcessor();
            private OrderProcessor interceptedOrderProcessor = new OrderProcessor();
            private object retrievedOrderProcessor;

        }

        public class InterceptingComposablePartContext
        {
            public InterceptingComposablePartContext()
            {
                MockPart = new Mock<ComposablePart>();
                InterceptedPart = AttributedModelServices.CreatePart(new OrderProcessor());
                LoggerImportDefinition = InterceptedPart.ImportDefinitions.First();
                OrderProcessorExportDefinition = InterceptedPart.ExportDefinitions.First();
                MockInterceptor = new Mock<IExportedValueInterceptor>();
                Exports = new List<Export> {new Export(OrderProcessorExportDefinition, () => _loggerInstance)};
                InterceptingPart = new InterceptingComposablePart(InterceptedPart, MockInterceptor.Object);
                Context();
            }

            public virtual void Context()
            {
            }

            public ComposablePart InterceptedPart;
            public InterceptingComposablePart InterceptingPart;
            public ImportDefinition LoggerImportDefinition;
            public ExportDefinition OrderProcessorExportDefinition;
            private ILogger _loggerInstance = new Logger();
            public IEnumerable<Export> Exports;
            public Mock<IExportedValueInterceptor> MockInterceptor;
            public Mock<ComposablePart> MockPart;
        }


    }
}
