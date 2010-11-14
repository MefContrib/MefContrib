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

        /// <summary>
        /// Gets the intercepted <see cref="ComposablePart"/> instance.
        /// </summary>
        public ComposablePart InterceptedPart { get; private set; }

        /// <summary>
        /// Gets the exported object described by the specified <see cref="ExportDefinition"/> object.
        /// </summary>
        /// <returns>
        /// The exported object described by <paramref name="definition"/>.
        /// </returns>
        /// <param name="definition">One of the <see cref="ExportDefinition"/> objects from the <see cref="ComposablePart.ExportDefinitions"/> property that describes the exported object to return.</param>
        /// <exception cref="ObjectDisposedException">The <see cref="ComposablePart"/> object has been disposed of.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="definition"/> is null.</exception>
        /// <exception cref="ComposablePartException">An error occurred getting the exported object described by the <see cref="ExportDefinition"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="definition"/> did not originate from the <see cref="ComposablePart.ExportDefinitions"/> property on the <see cref="ComposablePart"/>.</exception>
        /// <exception cref="InvalidOperationException">One or more prerequisite imports, indicated by <see cref="ImportDefinition.IsPrerequisite"/>, have not been set.</exception>
        public override object GetExportedValue(ExportDefinition definition)
        {
            if (definition == null) throw new ArgumentNullException("definition");

            if (this.values.ContainsKey(definition))
                return this.values[definition];

            var value = InterceptedPart.GetExportedValue(definition);
            var interceptingValue = this.valueInterceptor.Intercept(value);
            this.values.Add(definition, interceptingValue);
            
            return interceptingValue;
        }

        public override void Activate()
        {
            InterceptedPart.Activate();
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
