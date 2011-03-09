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
        private readonly IInterceptionConfiguration configuration;
        private readonly IDictionary<ComposablePartDefinition, InnerPartDefinition> innerParts;
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
            this.configuration = configuration;
            this.innerParts = new Dictionary<ComposablePartDefinition, InnerPartDefinition>();
            
            InitializeRecomposition();
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
                handler.Changed += HandlePartHandlerChanged;
            }
        }

        private void InitializeRecomposition()
        {
            var interceptedCatalogNotifyChange = interceptedCatalog as INotifyComposablePartCatalogChanged;
            if (interceptedCatalogNotifyChange != null)
            {
                interceptedCatalogNotifyChange.Changing += HandleInterceptedCatalogChanging;
            }
        }

        private void HandleInterceptedCatalogChanging(object sender, ComposablePartCatalogChangeEventArgs e)
        {
            Recompose(e.AddedDefinitions, e.RemovedDefinitions, e.AtomicComposition);
        }

        private void HandlePartHandlerChanged(object sender, PartHandlerChangedEventArgs e)
        {
            Recompose(e.AddedDefinitions, e.RemovedDefinitions, null);
        }

        private void EnsurePartsInitialized()
        {
            if (GetParts() == null)
            {
                throw new InvalidOperationException();
            }
        }

        private void Recompose(IEnumerable<ComposablePartDefinition> added, IEnumerable<ComposablePartDefinition> removed, AtomicComposition outerComposition)
        {
            EnsurePartsInitialized();

            var addedInnerPartDefinitions = added.Select(GetPart);
            var removedInnerPartDefinitions = removed.Select(def => innerParts[def]);

            using (var composition = new AtomicComposition(outerComposition))
            {
                var addedDefinitions = addedInnerPartDefinitions.Select(p => p.Definition).ToList();
                var removedDefinitions = removedInnerPartDefinitions.Select(p => p.Definition).ToList();

                composition.AddCompleteAction(() => OnChanged(
                    addedDefinitions,
                    removedDefinitions,
                    null));

                OnChanging(
                    addedDefinitions,
                    removedDefinitions,
                    composition);

                foreach (var innerPart in addedInnerPartDefinitions)
                {
                    innerParts.Add(innerPart.Original, innerPart);
                }

                foreach (var removedDefinition in removedInnerPartDefinitions)
                {
                    innerParts.Remove(removedDefinition.Original);
                }

                composition.Complete();
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

                        foreach (var innerPart in parts.Select(GetPart))
                        {
                            this.innerParts.Add(innerPart.Original, innerPart);
                        }

                        this.innerPartsQueryable = this.innerParts.Values.Select(p => p.Definition).AsQueryable();
                    }
                }
            }

            return this.innerPartsQueryable;
        }

        private InnerPartDefinition GetPart(ComposablePartDefinition partDefinition)
        {
            var interceptor = GetInterceptor(partDefinition);
            if (interceptor == null)
            {
                // If the part is not being intercepted, suppress interception
                // by returning the original part
                return new InnerPartDefinition(partDefinition);
            }

            var innerPart = new InnerPartDefinition(
                partDefinition,
                new InterceptingComposablePartDefinition(partDefinition, interceptor));

            return innerPart;
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
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;

        /// <summary>
        /// Occurs when a <see cref="ComposablePartCatalog"/> is changing.
        /// </summary>
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;

        /// <summary>
        /// Fires the <see cref="Changing"/> event.
        /// </summary>
        /// <param name="addedDefinitions">The collection of added <see cref="ComposablePartDefinition"/> instances.</param>
        /// <param name="removedDefinitions">The collection of removed <see cref="ComposablePartDefinition"/> instances.</param>
        /// <param name="composition"><see cref="AtomicComposition"/> instance.</param>
        protected virtual void OnChanging(IEnumerable<ComposablePartDefinition> addedDefinitions, IEnumerable<ComposablePartDefinition> removedDefinitions, AtomicComposition composition)
        {
            if (Changing != null)
            {
                if (addedDefinitions == null) addedDefinitions = Enumerable.Empty<ComposablePartDefinition>();
                if (removedDefinitions == null) removedDefinitions = Enumerable.Empty<ComposablePartDefinition>();

                var args = new ComposablePartCatalogChangeEventArgs(
                    addedDefinitions, removedDefinitions, composition);

                Changing(this, args);
            }
        }

        /// <summary>
        /// Fires the <see cref="Changed"/> event.
        /// </summary>
        /// <param name="addedDefinitions">The collection of added <see cref="ComposablePartDefinition"/> instances.</param>
        /// <param name="removedDefinitions">The collection of removed <see cref="ComposablePartDefinition"/> instances.</param>
        /// <param name="composition"><see cref="AtomicComposition"/> instance.</param>
        protected virtual void OnChanged(IEnumerable<ComposablePartDefinition> addedDefinitions, IEnumerable<ComposablePartDefinition> removedDefinitions, AtomicComposition composition)
        {
            if (Changed != null)
            {
                if (addedDefinitions == null) addedDefinitions = Enumerable.Empty<ComposablePartDefinition>();
                if (removedDefinitions == null) removedDefinitions = Enumerable.Empty<ComposablePartDefinition>();

                var args = new ComposablePartCatalogChangeEventArgs(
                    addedDefinitions, removedDefinitions, composition);

                Changed(this, args);
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

        private class InnerPartDefinition
        {
            public InnerPartDefinition(ComposablePartDefinition original)
            {
                Original = original;
            }

            public InnerPartDefinition(ComposablePartDefinition original, ComposablePartDefinition intercepting)
            {
                Original = original;
                Intercepting = intercepting;
            }

            public ComposablePartDefinition Original { get; private set; }

            private ComposablePartDefinition Intercepting { get; set; }

            public ComposablePartDefinition Definition
            {
                get { return Intercepting ?? Original; }
            }
            
            public bool Equals(InnerPartDefinition other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.Original, Original);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (InnerPartDefinition)) return false;
                return Equals((InnerPartDefinition) obj);
            }

            public override int GetHashCode()
            {
                return Original.GetHashCode();
            }
        }
    }
}
