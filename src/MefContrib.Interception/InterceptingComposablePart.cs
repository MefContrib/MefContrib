using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MefContrib.Interception
{
    public class InterceptingComposablePart : ComposablePart
    {
        public ComposablePart InterceptedPart { get; private set; }
        private readonly IExportedValueInterceptor _valueInterceptor;
        private IDictionary<ExportDefinition, object> _values;

        public InterceptingComposablePart(ComposablePart interceptedPart, IExportedValueInterceptor valueInterceptor)
        {
            interceptedPart.ShouldNotBeNull("interceptedPart");
            valueInterceptor.ShouldNotBeNull("valueInterceptor");

            InterceptedPart = interceptedPart;
            _valueInterceptor = valueInterceptor;
            _values = new Dictionary<ExportDefinition, object>();
        }

        public override object GetExportedValue(ExportDefinition exportDefinition)
        {
            exportDefinition.ShouldNotBeNull("exportDefinition");

            if (_values.ContainsKey(exportDefinition))
                return _values[exportDefinition];

            var value = InterceptedPart.GetExportedValue(exportDefinition);
            var interceptingValue = _valueInterceptor.Intercept(value);
            _values.Add(exportDefinition, interceptingValue);
            return interceptingValue;
        }

        public override void SetImport(ImportDefinition definition, IEnumerable<Export> exports)
        {
            definition.ShouldNotBeNull("definition");
            exports.ShouldNotBeNull("exports");
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
    }
}
