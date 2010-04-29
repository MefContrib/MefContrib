using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using MefContrib.Integration.Unity.Extensions;
using MefContrib.Integration.Unity.Properties;
using Microsoft.Practices.Unity;

namespace MefContrib.Integration.Unity.Exporters
{
    /// <summary>
    /// Exposes all types registered in associated <see cref="IUnityContainer"/> container
    /// to MEF using <see cref="ExternalExportProvider"/> class.
    /// </summary>
    public sealed class UnityExportProvider : ExportProvider
    {
        private IUnityContainer m_UnityContainer;

        private readonly object m_SyncRoot = new object();
        private readonly Func<IUnityContainer> m_UnityContainerResolver;
        private readonly ExternalExportProvider m_ExternalExportProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="UnityExportProvider"/> class.
        /// </summary>
        /// <param name="unityContainerResolver">Delegate called when the container is needed for
        /// the first time.</param>
        public UnityExportProvider(Func<IUnityContainer> unityContainerResolver)
        {
            if (unityContainerResolver == null)
                throw new ArgumentNullException("unityContainerResolver");

            m_UnityContainerResolver = unityContainerResolver;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="UnityExportProvider"/> class.
        /// </summary>
        /// <param name="unityContainer">An instance of the <see cref="IUnityContainer"/> container.</param>
        public UnityExportProvider(IUnityContainer unityContainer)
        {
            if (unityContainer == null)
                throw new ArgumentNullException("unityContainer");

            m_UnityContainer = unityContainer;
            m_ExternalExportProvider = new ExternalExportProvider(UnityFactoryMethod);

            ConfigureUnityContainer();
        }

        private void ConfigureUnityContainer()
        {
            TypeRegistrationTrackerExtension.RegisterIfMissing(m_UnityContainer);
            
            var tracker = m_UnityContainer.Configure<TypeRegistrationTrackerExtension>();

            foreach (var entry in tracker.Entries)
            {
                AddExportDefinition(entry.Type, entry.Name);
            }

            tracker.Registering += (s, e) =>
                AddExportDefinition(e.TypeFrom ?? e.TypeTo, e.Name);

            tracker.RegisteringInstance += (s, e) =>
                AddExportDefinition(e.RegisteredType, e.Name);
        }

        private object UnityFactoryMethod(Type requestedType, string registrationName)
        {
            return UnityContainer.Resolve(requestedType, registrationName);
        }

        protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
        {
            return m_ExternalExportProvider.GetExports(definition);
        }

        /// <summary>
        /// Adds a new export definition.
        /// </summary>
        /// <param name="type">Type that is being exported.</param>
        /// <param name="registrationName">Registration name under which <paramref name="type"/>
        /// is being exported.</param>
        public void AddExportDefinition(Type type, string registrationName)
        {
            m_ExternalExportProvider.AddExportDefinition(type, registrationName);
        }

        /// <summary>
        /// Gets associated <see cref="IUnityContainer"/> container.
        /// </summary>
        public IUnityContainer UnityContainer
        {
            get
            {
                if (m_UnityContainer == null)
                {
                    lock (m_SyncRoot)
                    {
                        if (m_UnityContainer == null)
                        {
                            m_UnityContainer = m_UnityContainerResolver.Invoke();

                            if (m_UnityContainer == null)
                                throw new Exception(Resources.UnityNullException);

                            ConfigureUnityContainer();
                        }
                    }
                }

                return m_UnityContainer;
            }
        }

        /// <summary>
        /// Gets a read only list of definitions known to the export provider.
        /// </summary>
        public IList<ExternalExportDefinition> ReadOnlyDefinitions
        {
            get { return m_ExternalExportProvider.ReadOnlyDefinitions; }
        }
    }
}