namespace MefContrib.Hosting
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
        /// <summary>
        /// Represents a metadata which identifies a contract based export.
        /// </summary>
        public const string IsContractBasedExportMetadataName = "IsContractBasedExport";

        /// <summary>
        /// Initializes a new instance of <see cref="ContractBasedExportDefinition"/> class.
        /// </summary>
        /// <param name="type">Type this export defines.</param>
        /// <param name="registrationName">Registration name under which <paramref name="type"/>
        /// has been registered.</param>
        public ContractBasedExportDefinition(Type type, string registrationName = null)
            : base(GetContractName(type, registrationName), GetMetadata(type))
        {
            if (type == null)
                throw new ArgumentNullException("type");
            
            ContractType = type;
            RegistrationName = registrationName;
        }

        /// <summary>
        /// Gets a type this export defines.
        /// </summary>
        public Type ContractType { get; private set; }

        /// <summary>
        /// Gets a registration name under which <see cref="ContractType"/> has been registered.
        /// </summary>
        public string RegistrationName { get; private set; }

        #region Private Implementation

        private static IDictionary<string,object> GetMetadata(Type type)
        {
            return new Dictionary<string, object>
                       {
                           {
                               CompositionConstants.ExportTypeIdentityMetadataName,
                               AttributedModelServices.GetTypeIdentity(type)
                               },
                           {
                               IsContractBasedExportMetadataName,
                               true
                               }
                       };
        }

        private static string GetContractName(Type type, string registrationName)
        {
            return registrationName ?? AttributedModelServices.GetContractName(type);
        }

        #endregion
    }
}