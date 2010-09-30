namespace MefContrib.Integration.Exporters
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;

    /// <summary>
    /// Represents a contract-based export definition that has a type and
    /// an option registration name.
    /// </summary>
    public class ContractBasedExportDefinition : ExportDefinition
    {
        private readonly string contractName;
        private readonly IDictionary<string, object> metadata;

        /// <summary>
        /// Initializes a new instance of <see cref="ContractBasedExportDefinition"/> class.
        /// </summary>
        /// <param name="type">Type this export defines.</param>
        public ContractBasedExportDefinition(Type type)
            : this(type, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ContractBasedExportDefinition"/> class.
        /// </summary>
        /// <param name="type">Type this export defines.</param>
        /// <param name="registrationName">Registration name under which <paramref name="type"/>
        /// has been registered.</param>
        public ContractBasedExportDefinition(Type type, string registrationName)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            this.metadata = new Dictionary<string, object>();
            this.contractName = registrationName ?? AttributedModelServices.GetContractName(type);

            ContractType = type;
            RegistrationName = registrationName;

            this.metadata.Add(
                CompositionConstants.ExportTypeIdentityMetadataName,
                AttributedModelServices.GetTypeIdentity(type));

            this.metadata.Add(
                ExporterConstants.IsContractBasedExportMetadataName, true);
        }

        /// <summary>
        /// Gets a type this export defines.
        /// </summary>
        public Type ContractType { get; private set; }

        /// <summary>
        /// Gets a registration name under which <see cref="ContractType"/> has been registered.
        /// </summary>
        public string RegistrationName { get; private set; }

        #region Overrides

        public override string ContractName
        {
            get { return contractName; }
        }

        public override IDictionary<string, object> Metadata
        {
            get { return metadata; }
        }

        #endregion
    }
}