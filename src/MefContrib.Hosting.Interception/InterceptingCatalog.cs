namespace MefContrib.Hosting.Interception
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using System.Threading;
    using MefContrib.Hosting.Interception.Configuration;

    /// <summary>
    /// Defines a catalog which enables interception on its parts.
    /// </summary>
    public class InterceptingCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        private readonly object synchRoot = new object();
        private readonly ComposablePartCatalog interceptedCatalog;
        private readonly INotifyComposablePartCatalogChanged interceptedCatalogNotifyChange;
        private readonly IInterceptionConfiguration configuration;
        private IQueryable<ComposablePartDefinition> innerPartsQueryable;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptingCatalog"/> class.
        /// </summary>
        /// <param name="interceptedCatalog">Catalog to be intercepted.</param>
        /// <param name="configuration">Interception configuration.</param>
        public InterceptingCatalog(ComposablePartCatalog interceptedCatalog, IInterceptionConfiguration configuration)
        {
            if (interceptedCatalog == null) throw new ArgumentNullException("interceptedCatalog");
            if (configuration == null) throw new ArgumentNullException("configuration");

            this.interceptedCatalog = interceptedCatalog;
            this.interceptedCatalogNotifyChange = interceptedCatalog as INotifyComposablePartCatalogChanged;
            this.configuration = configuration;
            
            InitializeHandlers();
        }

        private void InitializeHandlers()
        {
            foreach(var handler in this.configuration.Handlers)
            {
                handler.Initialize(this.interceptedCatalog);
            }
        }

        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            if (definition == null) throw new ArgumentNullException("definition");

            var exports = base.GetExports(definition);
            foreach (var handler in this.configuration.Handlers)
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
            if (this.innerPartsQueryable == null)
            {
                lock (this.synchRoot)
                {
                    if (this.innerPartsQueryable == null)
                    {
#if SILVERLIGHT
                        var parts = this.interceptedCatalog.Parts
                            .Select(p => new InterceptingComposablePartDefinition(p, GetInterceptor(p)))
                            .Cast<ComposablePartDefinition>()
                            .ToList()
                            .AsQueryable();
#else
                        var parts = this.interceptedCatalog.Parts
                            .Select(p => new InterceptingComposablePartDefinition(p, GetInterceptor(p)))
                            .ToList()
                            .AsQueryable();
#endif
                        Thread.MemoryBarrier();
                        this.innerPartsQueryable = parts;
                    }
                }
            }

            return this.innerPartsQueryable;
        }

        private IExportedValueInterceptor GetInterceptor(ComposablePartDefinition partDefinition)
        {
            var interceptors = new List<IExportedValueInterceptor>(this.configuration.Interceptors);
            var partInterceptors = from criteria in this.configuration.InterceptionCriteria
                                   where criteria.Predicate(partDefinition)
                                   select criteria.Interceptor;
            
            interceptors.AddRange(partInterceptors);

            if (interceptors.Count == 0) return EmptyInterceptor.Default;
            if (interceptors.Count == 1) return interceptors[0];

            return GetCompositeInterceptor(interceptors);
        }

        #region INotifyComposablePartCatalogChanged Implementation

        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed
        {
            add
            {
                if (this.interceptedCatalogNotifyChange != null)
                    this.interceptedCatalogNotifyChange.Changed += value;
            }
            remove
            {
                if (this.interceptedCatalogNotifyChange != null)
                    this.interceptedCatalogNotifyChange.Changed -= value;
            }
        }

        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing
        {
            add
            {
                if (this.interceptedCatalogNotifyChange != null)
                    this.interceptedCatalogNotifyChange.Changing += value;
            }
            remove
            {
                if (this.interceptedCatalogNotifyChange != null)
                    this.interceptedCatalogNotifyChange.Changing -= value;
            }
        }

        #endregion

        /// <summary>
        /// Method called in order to aggregate two or more <see cref="IExportedValueInterceptor"/> instances
        /// into a single interceptor. By default, the <see cref="CompositeValueInterceptor"/> instance is created.
        /// </summary>
        /// <param name="interceptors">Interceptors to be aggregated.</param>
        /// <returns>New instance of the <see cref="IExportedValueInterceptor"/>.</returns>
        protected virtual IExportedValueInterceptor GetCompositeInterceptor(
            IEnumerable<IExportedValueInterceptor> interceptors)
        {
            return new CompositeValueInterceptor(interceptors.ToArray());
        }
    }
}
