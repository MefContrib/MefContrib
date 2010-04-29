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

namespace MefContrib.Integration.Unity
{
    /// <summary>
    /// Represents a Unity extension that adds integration with
    /// Managed Extensibility Framework.
    /// </summary>
    public sealed class CompositionIntegration : UnityContainerExtension, IDisposable
    {
        private readonly bool m_Register;

        private AggregateCatalog m_AggregateCatalog;
        private ExportProvider[] m_Providers;
        private CompositionContainer m_CompositionContainer;

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
            m_AggregateCatalog = new AggregateCatalog();
            m_Register = register;
            m_Providers = providers;
        }

        protected override void Initialize()
        {
            TypeRegistrationTrackerExtension.RegisterIfMissing(Container);

            m_CompositionContainer = PrepareCompositionContainer();

            Context.Policies.SetDefault<ICompositionContainerPolicy>(new CompositionContainerPolicy(m_CompositionContainer));
            Context.Strategies.AddNew<CompositionStrategy>(UnityBuildStage.TypeMapping);
            Context.Strategies.AddNew<ComposeStrategy>(UnityBuildStage.Initialization);
        }

        private CompositionContainer PrepareCompositionContainer()
        {
            // Create the MEF container based on the catalog
            var compositionContainer = new CompositionContainer(m_AggregateCatalog, m_Providers);

            // If desired, register an instance of CompositionContainer and Unity container in MEF,
            // this will also make CompositionContainer available to the Unity
            if (Register)
            {
                // Create composition batch and add the MEF container and the Unity
                // container to the MEF
                var batch = new CompositionBatch();
                batch.AddExportedValue(compositionContainer);
                batch.AddExportedValue(Container);

                // Prepare container
                compositionContainer.Compose(batch);
            }

            return compositionContainer;
        }

        /// <summary>
        /// Returns true if underlying <see cref="CompositionContainer"/> should be registered
        /// in the <see cref="IUnityContainer"/> container.
        /// </summary>
        public bool Register
        {
            get { return m_Register; }
        }

        /// <summary>
        /// Gets a collection of catalogs MEF is able to access.
        /// </summary>
        public ICollection<ComposablePartCatalog> Catalogs
        {
            get { return m_AggregateCatalog.Catalogs; }
        }

        /// <summary>
        /// Gets a read-only collection of <see cref="ExportProvider"/>s registered in this extension.
        /// </summary>
        public IEnumerable<ExportProvider> Providers
        {
            get { return new List<ExportProvider>(m_Providers); }
        }

        /// <summary>
        /// Gets <see cref="CompositionContainer"/> used by the extension.
        /// </summary>
        public CompositionContainer CompositionContainer
        {
            get { return m_CompositionContainer; }
        }

        #region IDisposable

        public void Dispose()
        {
            if (m_CompositionContainer != null)
                m_CompositionContainer.Dispose();

            if (m_AggregateCatalog != null)
                m_AggregateCatalog.Dispose();
            
            m_CompositionContainer = null;
            m_AggregateCatalog = null;
            m_Providers = null;
        }

        #endregion
    }
}