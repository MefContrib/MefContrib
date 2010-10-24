namespace MefContrib.Hosting.Interception
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using System.Threading;
    using MefContrib.Hosting.Interception.Configuration;

    public class InterceptingCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        private readonly object _lock = new object();
        private readonly ComposablePartCatalog _interceptedCatalog;
        private readonly INotifyComposablePartCatalogChanged _interceptedCatalogNotifyChange;
        private readonly IInterceptionConfiguration _configuration;
        private IQueryable<ComposablePartDefinition> _innerPartsQueryable;
        
        public InterceptingCatalog(ComposablePartCatalog interceptedCatalog, IInterceptionConfiguration configuration)
        {
            if (interceptedCatalog == null) throw new ArgumentNullException("interceptedCatalog");
            if (configuration == null) throw new ArgumentNullException("configuration");

            _interceptedCatalog = interceptedCatalog;
            _interceptedCatalogNotifyChange = interceptedCatalog as INotifyComposablePartCatalogChanged;
            _configuration = configuration;
            
            InitializeHandlers();
        }

        private void InitializeHandlers()
        {
            foreach(var handler in _configuration.Handlers)
            {
                handler.Initialize(_interceptedCatalog);
            }
        }

        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            if (definition == null) throw new ArgumentNullException("definition");

            var exports = base.GetExports(definition);
            foreach (var handler in _configuration.Handlers)
            {
                exports = handler.GetExports(definition, exports);
            }

            return exports;
        }

        public override IQueryable<ComposablePartDefinition> Parts
        {
            get { return GetParts(); }
        }

        private IQueryable<ComposablePartDefinition> GetParts()
        {
            if (_innerPartsQueryable == null)
            {
                lock (_lock)
                {
                    if (_innerPartsQueryable == null)
                    {
#if SILVERLIGHT
                        var parts = _interceptedCatalog.Parts
                            .Select(p => new InterceptingComposablePartDefinition(p, GetInterceptor(p)))
                            .Cast<ComposablePartDefinition>()
                            .AsQueryable();
#else
                        var parts = _interceptedCatalog.Parts
                            .Select(p => new InterceptingComposablePartDefinition(p, GetInterceptor(p)))
                            .AsQueryable();
#endif
                        Thread.MemoryBarrier();
                        _innerPartsQueryable = parts;
                    }
                }
            }

            return _innerPartsQueryable;
        }

        private IExportedValueInterceptor GetInterceptor(ComposablePartDefinition partDefinition)
        {
            var interceptors = new List<IExportedValueInterceptor>();
            var catalogInterceptor = _configuration.Interceptor;
            var additionalInterceptors = from criteria in _configuration.InterceptionCriteria
                                         where criteria.Predicate(partDefinition)
                                         select criteria.Interceptor;

            if (catalogInterceptor != null)
            {
                interceptors.Add(catalogInterceptor);
            }

            interceptors.AddRange(additionalInterceptors);

            if (interceptors.Count == 0) return EmptyInterceptor.Default;
            if (interceptors.Count == 1) return interceptors[0];

            return new CompositeValueInterceptor(interceptors.ToArray());
        }

        #region INotifyComposablePartCatalogChanged Implementation

        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed
        {
            add
            {
                if (this._interceptedCatalogNotifyChange != null)
                    this._interceptedCatalogNotifyChange.Changed += value;
            }
            remove
            {
                if (this._interceptedCatalogNotifyChange != null)
                    this._interceptedCatalogNotifyChange.Changed -= value;
            }
        }

        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing
        {
            add
            {
                if (this._interceptedCatalogNotifyChange != null)
                    this._interceptedCatalogNotifyChange.Changing += value;
            }
            remove
            {
                if (this._interceptedCatalogNotifyChange != null)
                    this._interceptedCatalogNotifyChange.Changing -= value;
            }
        }

        #endregion

        private class EmptyInterceptor : IExportedValueInterceptor
        {
            public static readonly IExportedValueInterceptor Default = new EmptyInterceptor();

            public object Intercept(object value)
            {
                return value;
            }
        }
    }
}
