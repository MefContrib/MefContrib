namespace MefContrib.Interception
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using System.Threading;

    public class InterceptingCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        private readonly object _lock = new object();
        private readonly ComposablePartCatalog _interceptedCatalog;
        private readonly IExportedValueInterceptor _valueInterceptor;
        private readonly IList<IExportHandler> _handlers;
        private IQueryable<ComposablePartDefinition> _innerPartsQueryable;

        public InterceptingCatalog(ComposablePartCatalog interceptedCatalog, IExportedValueInterceptor valueInterceptor)
            : this(interceptedCatalog, valueInterceptor, new List<IExportHandler>())
        {
        }

        public InterceptingCatalog(ComposablePartCatalog interceptedCatalog, IExportedValueInterceptor valueInterceptor, IList<IExportHandler> handlers)
        {
            if (interceptedCatalog == null) throw new ArgumentNullException("interceptedCatalog");
            if (valueInterceptor == null) throw new ArgumentNullException("valueInterceptor");
            if (handlers == null) throw new ArgumentNullException("handlers");
            
            _interceptedCatalog = interceptedCatalog;
            _valueInterceptor = valueInterceptor;
            _handlers = handlers;
            
            InitializeHandlers();
        }

        private void InitializeHandlers()
        {
            foreach(var handler in _handlers)
            {
                handler.Initialize(this);
            }
        }

        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            if (definition == null) throw new ArgumentNullException("definition");

            var exports = base.GetExports(definition);
            foreach (var handler in _handlers)
            {
                exports = handler.GetExports(definition, exports);
            }

            return exports;
        }

        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                return GetParts();
            }
        }

        private IQueryable<ComposablePartDefinition> GetParts()
        {
            if (_innerPartsQueryable == null)
            {
                lock (_lock)
                {
                    if (_innerPartsQueryable == null)
                    {
                        var parts = _interceptedCatalog.Parts
                            .Select(p => new InterceptingComposablePartDefinition(p, _valueInterceptor))
                            .AsQueryable();
                        Thread.MemoryBarrier();
                        _innerPartsQueryable = parts;
                    }
                }
            }

            return _innerPartsQueryable;
        }

        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;

        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;
    }
}
