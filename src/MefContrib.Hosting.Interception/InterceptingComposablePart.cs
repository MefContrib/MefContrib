namespace MefContrib.Hosting.Interception
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Primitives;

    public class InterceptingComposablePart : ComposablePart
    {
        private readonly IExportedValueInterceptor valueInterceptor;
        private readonly IDictionary<ExportDefinition, object> values;

        public InterceptingComposablePart(ComposablePart interceptedPart, IExportedValueInterceptor valueInterceptor)
        {
            if (interceptedPart == null) throw new ArgumentNullException("interceptedPart");
            if (valueInterceptor == null) throw new ArgumentNullException("valueInterceptor");

            InterceptedPart = interceptedPart;
            this.valueInterceptor = valueInterceptor;
            this.values = new Dictionary<ExportDefinition, object>();
        }

        public ComposablePart InterceptedPart { get; private set; }

        public override object GetExportedValue(ExportDefinition exportDefinition)
        {
            if (exportDefinition == null) throw new ArgumentNullException("exportDefinition");

            if (this.values.ContainsKey(exportDefinition))
                return this.values[exportDefinition];

            var value = InterceptedPart.GetExportedValue(exportDefinition);
            var interceptingValue = this.valueInterceptor.Intercept(value);
            this.values.Add(exportDefinition, interceptingValue);
            
            return interceptingValue;
        }

        public override void SetImport(ImportDefinition definition, IEnumerable<Export> exports)
        {
            if (definition == null) throw new ArgumentNullException("definition");
            if (exports == null) throw new ArgumentNullException("exports");

            InterceptedPart.SetImport(definition, exports);
        }

        public override IEnumerable<ExportDefinition> ExportDefinitions
        {
            get { return InterceptedPart.ExportDefinitions; }
        }

        public override IEnumerable<ImportDefinition> ImportDefinitions
        {
            get { return InterceptedPart.ImportDefinitions; }
        }

        public override IDictionary<string, object> Metadata
        {
            get { return InterceptedPart.Metadata; }
        }

        public override string ToString()
        {
            return InterceptedPart.ToString();
        }
    }
}
