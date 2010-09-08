using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace MefContrib.Integration.Exporters
{
    /// <summary>
    /// Represents an <see cref="ExportProvider"/> which can provide
    /// MEF with parts registered in any container.
    /// </summary>
    public class ContainerExportProvider : ExportProvider
    {
        private readonly IContainerAdapter m_ContainerAdapter;
        private readonly RegistrationBasedFactoryExportProvider m_FactoryProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="ContainerExportProvider"/> class.
        /// </summary>
        /// <param name="containerAdapter">An instance of the <see cref="IContainerAdapter"/> interface.</param>
        public ContainerExportProvider(IContainerAdapter containerAdapter)
        {
            if (containerAdapter == null)
                throw new ArgumentNullException("containerAdapter");

            m_FactoryProvider = new RegistrationBasedFactoryExportProvider(FactoryMethod);
            m_ContainerAdapter = containerAdapter;
            m_ContainerAdapter.RegisteringComponent += OnRegisteringComponentHandler;
            
            // Initialize the adapter
            m_ContainerAdapter.Initialize();
        }

        private void OnRegisteringComponentHandler(object sender, RegisterComponentEventArgs e)
        {
            m_FactoryProvider.Register(e.Type, e.Name);
        }
        
        private object FactoryMethod(Type requestedType, string registrationName)
        {
            return m_ContainerAdapter.Resolve(requestedType, registrationName);
        }

        protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
        {
            if (definition.Cardinality == ImportCardinality.ZeroOrMore)
            {
                return m_FactoryProvider.GetExports(definition, atomicComposition);
            }

            // If asked for "one or less", use the TryGetExports instead of GetExports to avoid
            // cardinality exceptions
            IEnumerable<Export> exports;
            var cardinalityCheckResult = m_FactoryProvider.TryGetExports(definition, atomicComposition, out exports);

            if (cardinalityCheckResult)
            {
                return exports;
            }

            // TryGetExports didn't find any valid exports, return empty
            return Enumerable.Empty<Export>();
        }

        /// <summary>
        /// Gets the underlying <see cref="RegistrationBasedFactoryExportProvider"/> instance.
        /// </summary>
        public RegistrationBasedFactoryExportProvider RegistrationBasedFactoryExportProvider
        {
            get { return m_FactoryProvider; }
        }
    }
}