using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;

namespace MefContrib.Interception
{
    public class InterceptingComposablePartDefinition : ComposablePartDefinition
    {
        public ComposablePartDefinition InterceptedPartDefinition {get; private set;}
        private readonly IExportedValueInterceptor _valueInterceptor;

        public InterceptingComposablePartDefinition(ComposablePartDefinition interceptedPartDefinition, IExportedValueInterceptor valueInterceptor)
        {
            interceptedPartDefinition.ShouldNotBeNull("interceptedPartDefinition");
            valueInterceptor.ShouldNotBeNull("valueInterceptor");

            InterceptedPartDefinition = interceptedPartDefinition;
            _valueInterceptor = valueInterceptor;
        }

        public override ComposablePart CreatePart()
        {
            var interceptedPart = InterceptedPartDefinition.CreatePart();
            return new InterceptingComposablePart(interceptedPart, _valueInterceptor);
        }

        public override IEnumerable<ExportDefinition> ExportDefinitions
        {
            get { return InterceptedPartDefinition.ExportDefinitions; }
        }

        public override IEnumerable<ImportDefinition> ImportDefinitions
        {
            get { return InterceptedPartDefinition.ImportDefinitions; }
        }
    }
}
