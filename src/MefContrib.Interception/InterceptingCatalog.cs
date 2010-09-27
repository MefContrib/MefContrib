using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading;

namespace MefContrib.Interception
{
    public class InterceptingCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
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
            interceptedCatalog.ShouldNotBeNull("interceptedCatalog");
            valueInterceptor.ShouldNotBeNull("valueInterceptor)");
            _interceptedCatalog = interceptedCatalog;
            _valueInterceptor = valueInterceptor;
            _handlers = handlers;
            InitializeHandlers();
        }

        private void InitializeHandlers()
        {
            foreach(var handler in _handlers)
                handler.Initialize(this);

        }

        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            definition.ShouldNotBeNull("definition");
            IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> exports;
            exports = base.GetExports(definition);
            foreach (var handler in _handlers)
                exports = handler.GetExports(definition, exports);
            return exports;
        }

        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                return GetParts();
            }
        }

        private object _lock = new object();

        private IQueryable<ComposablePartDefinition> GetParts()
        {
            if (_innerPartsQueryable == null)
            {
                lock (_lock)
                {
                    if (_innerPartsQueryable == null)
                    {
                        var parts = _interceptedCatalog.Parts.Select(
                                p =>
                                (ComposablePartDefinition)
                                new InterceptingComposablePartDefinition(p, _valueInterceptor)).
                                AsQueryable();
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
