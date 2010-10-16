namespace MefContrib.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;

    /// <summary>
    /// Represents a factory export definition that has a type and
    /// an optional registration name.
    /// </summary>
    public class FactoryExportDefinition : ExportDefinition
    {
        /// <summary>
        /// Represents a metadata which identifies a factory export.
        /// </summary>
        public const string IsFactoryExportMetadataName = "IsFactoryExport";

        /// <summary>
        /// Initializes a new instance of <see cref="FactoryExportDefinition"/> class.
        /// </summary>
        /// <param name="type">Type this export defines.</param>
        /// <param name="registrationName">Registration name under which <paramref name="type"/>
        /// has been registered.</param>
        /// <param name="factory">Export factory.</param>
        public FactoryExportDefinition(Type type, string registrationName, Func<ExportProvider, object> factory)
            : base(GetContractName(type, registrationName), GetMetadata(type))
        {
            if (type == null) throw new ArgumentNullException("type");
            if (factory == null) throw new ArgumentNullException("factory");

            ContractType = type;
            RegistrationName = registrationName;
            Factory = factory;
        }

        /// <summary>
        /// Gets a type this export defines.
        /// </summary>
        public Type ContractType { get; private set; }

        /// <summary>
        /// Gets a registration name under which <see cref="ContractType"/> has been registered.
        /// </summary>
        public string RegistrationName { get; private set; }

        /// <summary>
        /// Gets the factory used to create instances of export described by this <see cref="ExportDefinition"/>.
        /// </summary>
        public Func<ExportProvider, object> Factory { get; private set; }

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
                               IsFactoryExportMetadataName,
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