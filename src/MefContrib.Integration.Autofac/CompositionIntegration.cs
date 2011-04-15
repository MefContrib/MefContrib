#region Header

// -----------------------------------------------------------------------------
//  Copyright (c) Edenred (Incentives & Motivation) Ltd.  All rights reserved.
// -----------------------------------------------------------------------------

#endregion

namespace MefContrib.Integration.Autofac
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using Containers;
    using global::Autofac;
    using global::Autofac.Core;
    using global::Autofac.Core.Lifetime;
    using global::Autofac.Core.Resolving;
    using global::Autofac.Util;

    /// <summary>
    ///   Represents an Autofac extension that adds integration with
    ///   Managed Extensibility Framework.
    /// </summary>
    public sealed class CompositionIntegration : Disposable, IStartable
    {
        /// <summary>
        /// The name of the <see cref="CompositionContainer"/> used for composition integration.
        /// </summary>
        public const string AutofacCompositionContainerName = "Autofac";

        #region Fields

        private readonly ILifetimeScope _context;

        private AggregateCatalog _aggregateCatalog = new AggregateCatalog();
        private CompositionContainer _compositionContainer;
        private ExportProvider[] _providers;
        private bool _started;
        
        #endregion

        #region Constructors

        /// <summary>
        ///   Initializes a new instance of <see cref = "CompositionIntegration" /> class.
        /// </summary>
        public CompositionIntegration(ILifetimeScope context, IEnumerable<ComposablePartCatalog> catalogs)
        {
            _context = context;
            if (catalogs != null && catalogs.Any())
                foreach (var catalog in catalogs.Where(catalog => !_aggregateCatalog.Catalogs.Contains(catalog)))
                    _aggregateCatalog.Catalogs.Add(catalog);
            ConfigureMef();
            ConfigureAutofac();
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Returns true if underlying <see cref = "CompositionContainer" /> should be registered
        ///   in the <see cref = "ILifetimeScope" /> container.
        /// </summary>
        public bool Register { get; private set; }

        /// <summary>
        ///   Gets a collection of catalogs MEF is able to access.
        /// </summary>
        public ICollection<ComposablePartCatalog> Catalogs
        {
            get { return _aggregateCatalog.Catalogs; }
        }

        /// <summary>
        ///   Gets a read-only collection of <see cref = "ExportProvider" />s registered in this extension.
        /// </summary>
        public IEnumerable<ExportProvider> Providers
        {
            get { return new List<ExportProvider>(_providers); }
        }

        /// <summary>
        ///   Gets <see cref = "CompositionContainer" /> used by the extension.
        /// </summary>
        public CompositionContainer CompositionContainer
        {
            get { return _compositionContainer ?? (_compositionContainer = PrepareCompositionContainer()); }
        }

        /// <summary>
        ///   Gets a value indicating whether this <see cref = "CompositionIntegration" /> is started.
        /// </summary>
        /// <value>
        ///   <see langword = "true" /> if started; otherwise, <see langword = "false" />.
        /// </value>
        internal bool Started
        {
            get { return _started; }
        }

        #endregion

        #region Implementation of IStartable

        /// <summary>
        ///   Perform once-off startup processing.
        /// </summary>
        public void Start()
        {
            if (_started) return;
            _context.ResolveOperationBeginning += ResolveOperationBeginning;
            _context.ChildLifetimeScopeBeginning += LifetimeScopeBeginning;
            _context.CurrentScopeEnding += LifetimeScopeEnding;
            _started = true;
        }

        #endregion

        #region Methods

        private static void ActivatingComponent(object sender, ActivatingEventArgs<object> e)
        {
            var type = e.Instance.GetType();
            var isFromMef = (e.Component.Metadata.ContainsKey("Source") &&
                             e.Component.Metadata["Source"] as string == "MEF");
            var isNotComposable = type.GetCustomAttributes(typeof (PartNotComposableAttribute), false).Any();
            if (isNotComposable || isFromMef) return;

            var container = e.Context.ResolveNamed<CompositionContainer>(AutofacCompositionContainerName);
            container.SatisfyImportsOnce(e.Instance);
        }

        private void ConfigureAutofac()
        {
            var registrationSource = new MefRegistrationSource(CompositionContainer);
            var builder = new ContainerBuilder();
            builder.RegisterSource(registrationSource);
            builder.Update(_context.ComponentRegistry);
        }

        private void ConfigureMef()
        {
            var adapter = new AutofacContainerAdapter(_context);
            var containerExportProvider = new ContainerExportProvider(adapter);
            var scope = _context as ISharingLifetimeScope;
            if (scope != null)
            {
                var parentScope = scope.ParentLifetimeScope;
                var parent = parentScope != null ? parentScope.ResolveOptional<CompositionIntegration>() : null;
                if (parent != null)
                {
                    // Get the parent ContainerExportProvider
                    var parentContainerExportProvider = (ContainerExportProvider) parent.Providers.Where(
                        ep => typeof (ContainerExportProvider).IsAssignableFrom(ep.GetType())).First();

                    // Collect all the exports provided by the parent container and add
                    // them to the child export provider
                    foreach (var definition in parentContainerExportProvider.FactoryExportProvider.ReadOnlyDefinitions)
                    {
                        containerExportProvider.FactoryExportProvider.Register(
                            definition.ContractType,
                            definition.RegistrationName);
                    }

                    // Grab all the parent export providers except the container ones
                    var exportProviders = new List<ExportProvider>(
                        parent.Providers.Where(
                            ep => !typeof (ContainerExportProvider).IsAssignableFrom(ep.GetType())))
                                              {containerExportProvider};

                    var catalog = new AggregateCatalog(parent.Catalogs);
                        //NOTE: this might result in duplicate exports... need to investigate.

                    Register = true;
                    _providers = exportProviders.ToArray();
                    Catalogs.Add(catalog);
                }
                else
                {
                    Register = true;
                    _providers = new ExportProvider[] {containerExportProvider};
                }
            }
        }

        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name = "disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_compositionContainer != null)
                _compositionContainer.Dispose();

            if (_aggregateCatalog != null)
                _aggregateCatalog.Dispose();

            _compositionContainer = null;
            _aggregateCatalog = null;
            _providers = null;
        }

        private static void InstanceLookupBeginning(object sender, InstanceLookupBeginningEventArgs e)
        {
            e.InstanceLookup.ComponentRegistration.Activating += ActivatingComponent;
        }

        private static void LifetimeScopeBeginning(object sender, LifetimeScopeBeginningEventArgs e)
        {
            e.LifetimeScope.ResolveOperationBeginning += ResolveOperationBeginning;
        }

        private static void LifetimeScopeEnding(object sender, LifetimeScopeEndingEventArgs e)
        {
            e.LifetimeScope.ResolveOperationBeginning -= ResolveOperationBeginning;
            e.LifetimeScope.ChildLifetimeScopeBeginning -= LifetimeScopeBeginning;
            e.LifetimeScope.CurrentScopeEnding -= LifetimeScopeEnding;
        }

        private CompositionContainer PrepareCompositionContainer()
        {
            // Create the MEF container based on the catalog
            var container = new CompositionContainer(_aggregateCatalog, _providers);

            // If desired, register an instance of CompositionContainer and Autofac container in MEF,
            // this will also make CompositionContainer available to the Autofac
            if (Register)
            {
                // Create composition batch and add the MEF container and the Autofac
                // container to the MEF
                var batch = new CompositionBatch();
                batch.AddExportedValue(AutofacCompositionContainerName, container);
                batch.AddExportedValue(_context);
                container.Compose(batch);

                // Create a container builder and register the composition container in Autofac.
                var builder = new ContainerBuilder();
                builder.RegisterInstance(container).OwnedByLifetimeScope();
                builder.Update(_context.ComponentRegistry);
            }

            return container;
        }

        private static void ResolveOperationBeginning(object sender, ResolveOperationBeginningEventArgs e)
        {
            e.ResolveOperation.InstanceLookupBeginning += InstanceLookupBeginning;
        }

        #endregion
    }
}