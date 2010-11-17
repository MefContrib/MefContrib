namespace MefContrib.Hosting.Interception
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
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
            foreach(var handler in this.configuration.ExportHandlers)
            {
                handler.Initialize(this.interceptedCatalog);
            }

            foreach (var handler in this.configuration.PartHandlers)
            {
                handler.Initialize(this.interceptedCatalog);
            }
        }

        /// <summary>
        /// Gets a list of export definitions that match the constraint defined
        /// by the specified <see cref="ImportDefinition"/> object.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="T:System.Tuple`2"/> containing the <see cref="ExportDefinition"/>
        /// objects and their associated <see cref="ComposablePartDefinition"/> objects for objects that match the constraint specified by <paramref name="definition"/>.
        /// </returns>
        /// <param name="definition">The conditions of the <see cref="ExportDefinition"/>
        /// objects to be returned.</param>
        /// <exception cref="ObjectDisposedException">The <see cref="ComposablePartCatalog"/> object has been disposed of.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="definition"/> is null.</exception>
        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            if (definition == null) throw new ArgumentNullException("definition");

            var exports = base.GetExports(definition);
            foreach (var handler in this.configuration.ExportHandlers)
            {
                exports = handler.GetExports(definition, exports);
            }

            return exports;
        }

        /// <summary>
        /// Gets the part definitions that are contained in the catalog.
        /// </summary>
        /// <returns>
        /// The <see cref="ComposablePartDefinition"/> contained in the <see cref="ComposablePartCatalog"/>.
        /// </returns>
        /// <exception cref="ObjectDisposedException">The <see cref="ComposablePartCatalog"/> object has been disposed of.</exception>
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
                        IEnumerable<ComposablePartDefinition> parts =
                            new List<ComposablePartDefinition>(this.interceptedCatalog.Parts);

                        foreach (var handler in this.configuration.PartHandlers)
                        {
                            parts = handler.GetParts(parts);
                        }

                        this.innerPartsQueryable = parts
                            .Select(GetPart)
                            .ToList()
                            .AsQueryable();
                    }
                }
            }

            return this.innerPartsQueryable;
        }

        private ComposablePartDefinition GetPart(ComposablePartDefinition partDefinition)
        {
            var interceptor = GetInterceptor(partDefinition);
            if (interceptor == null)
            {
                // If the part is not being intercepted, suppress interception
                // by returning the original part
                return partDefinition;
            }

            return new InterceptingComposablePartDefinition(partDefinition, interceptor);
        }

        private IExportedValueInterceptor GetInterceptor(ComposablePartDefinition partDefinition)
        {
            var interceptors = new List<IExportedValueInterceptor>(this.configuration.Interceptors);
            var partInterceptors = from criteria in this.configuration.InterceptionCriteria
                                   where criteria.Predicate(partDefinition)
                                   select criteria.Interceptor;
            
            interceptors.AddRange(partInterceptors);

            if (interceptors.Count == 0) return null;
            if (interceptors.Count == 1) return interceptors[0];

            return GetCompositeInterceptor(interceptors);
        }

        #region INotifyComposablePartCatalogChanged Implementation

        /// <summary>
        /// Occurs when a <see cref="ComposablePartCatalog"/> has changed.
        /// </summary>
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

        /// <summary>
        /// Occurs when a <see cref="ComposablePartCatalog"/> is changing.
        /// </summary>
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
