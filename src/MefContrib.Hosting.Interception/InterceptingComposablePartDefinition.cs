namespace MefContrib.Hosting.Interception
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Primitives;

    public class InterceptingComposablePartDefinition : ComposablePartDefinition
    {
        private readonly IExportedValueInterceptor _valueInterceptor;

        public InterceptingComposablePartDefinition(ComposablePartDefinition interceptedPartDefinition, IExportedValueInterceptor valueInterceptor)
        {
            if (interceptedPartDefinition == null) throw new ArgumentNullException("interceptedPartDefinition");
            if (valueInterceptor == null) throw new ArgumentNullException("valueInterceptor");
            
            InterceptedPartDefinition = interceptedPartDefinition;
            _valueInterceptor = valueInterceptor;
        }

        public ComposablePartDefinition InterceptedPartDefinition { get; private set; }

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

        public override string ToString()
        {
            return InterceptedPartDefinition.ToString();
        }
    }
}
