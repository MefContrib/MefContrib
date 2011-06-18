namespace MefContrib.Integration.Unity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using MefContrib.Integration.Unity.Extensions;
    using MefContrib.Integration.Unity.Strategies;
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.ObjectBuilder;

    /// <summary>
    /// Represents a Unity extension that adds integration with
    /// Managed Extensibility Framework.
    /// </summary>
    public sealed class CompositionIntegration : UnityContainerExtension, IDisposable
    {
        private readonly bool register;

        private AggregateCatalog aggregateCatalog;
        private ExportProvider[] providers;
        private CompositionContainer compositionContainer;

        /// <summary>
        /// Initializes a new instance of <see cref="CompositionIntegration"/> class.
        /// </summary>
        [InjectionConstructor]
        public CompositionIntegration()
            : this(true)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CompositionIntegration"/> class.
        /// </summary>
        /// <param name="providers">An array of export providers.</param>
        public CompositionIntegration(params ExportProvider[] providers)
            : this(true, providers)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CompositionIntegration"/> class.
        /// </summary>
        /// <param name="register">If true, <see cref="CompositionContainer"/> instance
        /// will be registered in the Unity container.</param>
        /// <param name="providers">An array of export providers.</param>
        public CompositionIntegration(bool register, params ExportProvider[] providers)
        {
            this.aggregateCatalog = new AggregateCatalog();
            this.register = register;
            this.providers = providers;
        }

        /// <summary>
        /// Initial the container with this extension's functionality.
        /// </summary>
        /// <remarks>
        /// When overridden in a derived class, this method will modify the given
        /// <see cref="ExtensionContext"/> by adding strategies, policies, etc.
        /// to install it's functions into the container.
        /// </remarks>
        protected override void Initialize()
        {
            TypeRegistrationTrackerExtension.RegisterIfMissing(Container);

            this.compositionContainer = PrepareCompositionContainer();

            // Main strategies
            Context.Strategies.AddNew<EnumerableResolutionStrategy>(UnityBuildStage.TypeMapping);
            Context.Strategies.AddNew<CompositionStrategy>(UnityBuildStage.TypeMapping);
            Context.Strategies.AddNew<ComposeStrategy>(UnityBuildStage.Initialization);

            // Policies
            Context.Policies.Set<IBuildPlanPolicy>(
                new LazyResolveBuildPlanPolicy(), typeof(Lazy<>));

            Context.Policies.SetDefault<ICompositionContainerPolicy>(
                new CompositionContainerPolicy(compositionContainer));
        }

        private CompositionContainer PrepareCompositionContainer()
        {
            // Create the MEF container based on the catalog and given providers
            // Important: the catalog is wrapped with CatalogExportProvider which is
            // then added as a LAST catalog, this ensures that when querying Unity/MEF
            // for a single component, Unity components will always take precedence
            var catalogExportProvider = new CatalogExportProvider(this.aggregateCatalog);
            var providerList = new List<ExportProvider>(this.providers);
            providerList.Add(catalogExportProvider);
            var container = new CompositionContainer(providerList.ToArray());
            catalogExportProvider.SourceProvider = container;
            
            // If desired, register an instance of CompositionContainer and Unity container in MEF,
            // this will also make CompositionContainer available to the Unity
            if (Register)
            {
                // Create composition batch and add the MEF container and the Unity
                // container to the MEF
                var batch = new CompositionBatch();
                batch.AddExportedValue(container);
                batch.AddExportedValue(Container);

                // Prepare container
                container.Compose(batch);
            }

            return container;
        }

        /// <summary>
        /// Returns true if underlying <see cref="CompositionContainer"/> should be registered
        /// in the <see cref="IUnityContainer"/> container.
        /// </summary>
        public bool Register
        {
            get { return register; }
        }

        /// <summary>
        /// Gets a collection of catalogs MEF is able to access.
        /// </summary>
        public ICollection<ComposablePartCatalog> Catalogs
        {
            get { return aggregateCatalog.Catalogs; }
        }

        /// <summary>
        /// Gets a read-only collection of <see cref="ExportProvider"/>s registered in this extension.
        /// </summary>
        public IEnumerable<ExportProvider> Providers
        {
            get { return new List<ExportProvider>(providers); }
        }

        /// <summary>
        /// Gets <see cref="CompositionContainer"/> used by the extension.
        /// </summary>
        public CompositionContainer CompositionContainer
        {
            get { return compositionContainer; }
        }

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing,
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (compositionContainer != null)
                compositionContainer.Dispose();

            if (aggregateCatalog != null)
                aggregateCatalog.Dispose();
            
            compositionContainer = null;
            aggregateCatalog = null;
            providers = null;
        }

        #endregion
    }
}