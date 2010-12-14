namespace MefContrib.Hosting.Generics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.ComponentModel.Composition.ReflectionModel;
    using System.Linq;

    /// <summary>
    /// Defines a catalog which produces closed-generic parts.
    /// </summary>
    /// <remarks>
    /// The catalog can rewrite <see cref="ComposablePartDefinition"/> and its <see cref="ExportDefinition"/>
    /// instances so that the type identity and optionally the contract name is always closed-generic.
    /// </remarks>
    public class GenericTypeCatalog : ComposablePartCatalog
    {
        private readonly object synchRoot = new object();
        private IQueryable<ComposablePartDefinition> innerPartsQueryable;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericTypeCatalog"/> class.
        /// </summary>
        /// <param name="exportingType">The exporting type. This has to be closed-generic type.</param>
        public GenericTypeCatalog(Type exportingType)
        {
            ValidateExportingType(exportingType);

            var contractType = exportingType.GetGenericTypeDefinition();
            ValidateContractType(contractType);

            ExportingType = exportingType;
            ContractType = contractType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericTypeCatalog"/> class.
        /// </summary>
        /// <param name="exportingType">The exporting type. This has to be closed-generic type.</param>
        /// <param name="contractType">The contract type. This has to be open-generic type.</param>
        public GenericTypeCatalog(Type exportingType, Type contractType)
        {
            ValidateExportingType(exportingType);
            ValidateContractType(contractType);

            ExportingType = exportingType;
            ContractType = contractType;
        }

        private static void ValidateExportingType(Type exportingType)
        {
            if (exportingType == null)
            {
                throw new ArgumentNullException("exportingType");
            }

            if (!exportingType.IsGenericType)
            {
                throw new ArgumentException("Parameter exportingType is not a generic type.");
            }

            if (exportingType.IsGenericTypeDefinition)
            {
                throw new ArgumentException("Parameter exportingType cannot be an open generic type.");
            }
        }

        private static void ValidateContractType(Type contractType)
        {
            if (contractType == null)
            {
                throw new ArgumentNullException("contractType");
            }

            if (!contractType.IsGenericType)
            {
                throw new ArgumentException("Parameter contractType is not a generic type.");
            }

            if (!contractType.IsGenericTypeDefinition)
            {
                throw new ArgumentException("Parameter contractType is not an open generic type.");
            }
        }

        /// <summary>
        /// Gets the part type which declares the exports. This is always a closed-generic type.
        /// </summary>
        public Type ExportingType { get; private set; }

        /// <summary>
        /// Gets the contract type. This is always open-generic type.
        /// </summary>
        public Type ContractType { get; private set; }
        
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get { return GetParts(); }
        }

        private IQueryable<ComposablePartDefinition> GetParts()
        {
            if (this.innerPartsQueryable == null)
            {
                lock (this.synchRoot)
                {
                    if (this.innerPartsQueryable == null)
                    {
                        var catalog = new TypeCatalog(ExportingType);
                        this.innerPartsQueryable = catalog.Parts
                            .Select(RewritePart)
                            .ToList()
                            .AsQueryable();
                    }
                }
            }

            return this.innerPartsQueryable;
        }

        private ComposablePartDefinition RewritePart(ComposablePartDefinition definition)
        {
            var closedGenericContractType = ContractType.MakeGenericType(ExportingType.GetGenericArguments());
            var closedGenericTypeIdentity = AttributedModelServices.GetTypeIdentity(closedGenericContractType);
            var openGenericTypeIdentity = AttributedModelServices.GetTypeIdentity(ContractType);
            var openGenericContractName = AttributedModelServices.GetContractName(ContractType);
            var exports = new List<ExportDefinition>();
            var hasRewrittenExports = false;

            foreach (var exportDefinition in definition.ExportDefinitions)
            {
                // Rewrite only exports having open-generics type identity of the contract type we do care about
                if (openGenericTypeIdentity == (string)exportDefinition.Metadata[CompositionConstants.ExportTypeIdentityMetadataName])
                {
                    // If both open-generic contract name and the present contract name are equal,
                    // contract name has to be rewritten to form a closed-generic contract
                    var contractName = openGenericContractName == exportDefinition.ContractName
                                           ? AttributedModelServices.GetContractName(closedGenericContractType)
                                           : exportDefinition.ContractName;

                    // Preserve all the metadata except the type identity as it has to be rewritten
                    var metadata = new Dictionary<string, object>
                    {
                        { CompositionConstants.ExportTypeIdentityMetadataName, closedGenericTypeIdentity }
                    };
                    foreach (var key in exportDefinition.Metadata.Keys.Where(key => key != CompositionConstants.ExportTypeIdentityMetadataName))
                    {
                        metadata.Add(key, exportDefinition.Metadata[key]);
                    }
                    
                    // Rewrite the export
                    var rewrittenExport = ReflectionModelServices.CreateExportDefinition(
                        ReflectionModelServices.GetExportingMember(exportDefinition),
                        contractName,
                        new Lazy<IDictionary<string, object>>(() => metadata),
                        exportDefinition as ICompositionElement);
                    
                    exports.Add(rewrittenExport);
                    hasRewrittenExports = true;
                }
                else
                {
                    // This export is ok, copy the original
                    exports.Add(exportDefinition);
                }
            }

            // If the part has any rewritten exports, we have to rewrite the part itself
            if (hasRewrittenExports)
            {
                return ReflectionModelServices.CreatePartDefinition(
                    ReflectionModelServices.GetPartType(definition),
                    ReflectionModelServices.IsDisposalRequired(definition),
                    new Lazy<IEnumerable<ImportDefinition>>(() => definition.ImportDefinitions),
                    new Lazy<IEnumerable<ExportDefinition>>(() => exports),
                    new Lazy<IDictionary<string, object>>(() => definition.Metadata),
                    definition as ICompositionElement);
            }

            return definition;
        }
    }
}