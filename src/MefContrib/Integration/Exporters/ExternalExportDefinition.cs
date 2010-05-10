using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace MefContrib.Integration.Exporters
{
    /// <summary>
    /// Represents an external export definition that has a type and a registration name.
    /// </summary>
    public class ExternalExportDefinition : ExportDefinition
    {
        private readonly string m_ContractName;
        private readonly IDictionary<string, object> m_Metadata;

        /// <summary>
        /// Initializes a new instance of <see cref="ExternalExportDefinition"/> class.
        /// </summary>
        /// <param name="type">Type this export defines.</param>
        public ExternalExportDefinition(Type type)
            : this(type, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ExternalExportDefinition"/> class.
        /// </summary>
        /// <param name="type">Type this export defines.</param>
        /// <param name="registrationName">Registration name under which <paramref name="type"/>
        /// has been registered.</param>
        public ExternalExportDefinition(Type type, string registrationName)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            m_Metadata = new Dictionary<string, object>();
            m_ContractName = registrationName ?? AttributedModelServices.GetContractName(type);

            ServiceType = type;
            RegistrationName = registrationName;

            m_Metadata.Add(
                CompositionConstants.ExportTypeIdentityMetadataName,
                AttributedModelServices.GetContractName(type));

            m_Metadata.Add(
                ExporterConstants.IsExternallyProvidedMetadataName, true);
        }

        public override string ContractName
        {
            get { return m_ContractName; }
        }

        public override IDictionary<string, object> Metadata
        {
            get { return m_Metadata; }
        }

        /// <summary>
        /// Gets a type this export defines.
        /// </summary>
        public Type ServiceType { get; private set; }

        /// <summary>
        /// Gets a registration name under which <see cref="ServiceType"/> has been registered.
        /// </summary>
        public string RegistrationName { get; private set; }
    }
}