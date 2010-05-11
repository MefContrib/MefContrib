using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Linq;

namespace MefContrib.Integration.Exporters
{
    /// <summary>
    /// Represents an external export provider.
    /// </summary>
    /// <remarks>
    /// This class can be used to build custom <see cref="ExportProvider"/> which
    /// provides exports from various data sources.
    /// </remarks>
    public class ExternalExportProvider : ExportProvider
    {
        private readonly Func<Type, string, object> m_FactoryMethod;
        private readonly List<ExternalExportDefinition> m_Definitions;

        /// <summary>
        /// Initializes a new instance of <see cref="ExternalExportProvider"/> class.
        /// </summary>
        /// <param name="factoryMethod">Method that is called when an instance os specific type
        /// is requested, optionally with given registration name.</param>
        public ExternalExportProvider(Func<Type, string, object> factoryMethod)
        {
            if (factoryMethod == null)
                throw new ArgumentNullException("factoryMethod");

            m_Definitions = new List<ExternalExportDefinition>();
            m_FactoryMethod = factoryMethod;
        }

        protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
        {
            if (definition.Cardinality == ImportCardinality.ExactlyOne || definition.Cardinality == ImportCardinality.ZeroOrOne)
            {
                return GetExportsCore(ReadOnlyDefinitions, definition.Constraint.Compile());
            }

            if (definition.ContractName != null)
            {
                var list = new List<Export>();
                foreach (var externalExportDefinition in ReadOnlyDefinitions)
                {
                    if (AttributedModelServices.GetContractName(externalExportDefinition.ServiceType) == definition.ContractName)
                    {
                        var exportDefinition = externalExportDefinition;
                        var export = new Export(externalExportDefinition, () => GetExportedObject(exportDefinition.ServiceType, exportDefinition.RegistrationName));

                        list.Add(export);
                    }
                }

                return list;
            }

            return Enumerable.Empty<Export>();
        }

        private IEnumerable<Export> GetExportsCore(
            IEnumerable<ExternalExportDefinition> exportDefinitions,
            Func<ExportDefinition, bool> constraint)
        {
            Debug.Assert(exportDefinitions != null);
            Debug.Assert(constraint != null);

            return (from exportDefinition in exportDefinitions
                    where constraint(exportDefinition)
                    select CreateExport(exportDefinition)).ToList();
        }

        private Export CreateExport(ExternalExportDefinition export)
        {
            return new Export(export, () => GetExportedObject(
                export.ServiceType, export.RegistrationName));
        }

        private object GetExportedObject(Type type, string contractName)
        {
            return m_FactoryMethod.Invoke(type, contractName);
        }

        /// <summary>
        /// Adds a new export definition.
        /// </summary>
        /// <param name="type">Type that is being exported.</param>
        public void AddExportDefinition(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            AddExportDefinition(type, null);
        }

        /// <summary>
        /// Adds a new export definition.
        /// </summary>
        /// <param name="type">Type that is being exported.</param>
        /// <param name="registrationName">Registration name under which <paramref name="type"/>
        /// is being exported.</param>
        public void AddExportDefinition(Type type, string registrationName)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            var definitions = ReadOnlyDefinitions.Where(t => t.ServiceType == type &&
                                                             t.RegistrationName == registrationName);

            // We cannot add an export definition with the same type and registration name
            // since we will introduce cardinality problems
            if (definitions.Count() == 0)
            {
                m_Definitions.Add(new ExternalExportDefinition(type, registrationName));
            }
        }

        /// <summary>
        /// Gets a read only list of definitions known to the export provider.
        /// </summary>
        public IEnumerable<ExternalExportDefinition> ReadOnlyDefinitions
        {
            get { return m_Definitions.AsReadOnly(); }
        }
    }
}