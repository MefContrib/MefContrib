namespace MefContrib.Hosting.Generics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.ComponentModel.Composition.ReflectionModel;
    using System.Linq;

    public class GenericTypeCatalog : ComposablePartCatalog
    {
        private readonly object synchRoot = new object();
        private IQueryable<ComposablePartDefinition> innerPartsQueryable;

        public GenericTypeCatalog(Type exportingType)
            : this(exportingType, exportingType.GetGenericTypeDefinition())
        {
        }

        public GenericTypeCatalog(Type exportingType, Type contractType)
        {
            if (exportingType == null)
            {
                throw new ArgumentNullException("exportingType");
            }

            if (contractType == null)
            {
                throw new ArgumentNullException("contractType");
            }

            if (!exportingType.IsGenericType)
            {
                throw new ArgumentException("Parameter exportingType is not a generic type.");
            }
            
            if (exportingType.IsGenericTypeDefinition)
            {
                throw new ArgumentException("Parameter exportingType cannot be an open generic type.");
            }

            if (!contractType.IsGenericType)
            {
                throw new ArgumentException("Parameter contractType is not a generic type.");
            }

            if (!contractType.IsGenericTypeDefinition)
            {
                throw new ArgumentException("Parameter contractType is not an open generic type.");
            }

            ExportingType = exportingType;
            ContractType = contractType;
        }

        public Type ExportingType { get; private set; }

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
                            .Where(t => t != null)
                            .ToList()
                            .AsQueryable();
                    }
                }
            }

            return this.innerPartsQueryable;
        }

        private ComposablePartDefinition RewritePart(ComposablePartDefinition definition)
        {
            var exportDefinition = definition.ExportDefinitions.FirstOrDefault(t => CompositionServices.GetExportDefinitionType(t) == ExportingType);
            if (exportDefinition != null)
            {
                var closedGenericContractType = ContractType.MakeGenericType(ExportingType.GetGenericArguments());
                var typeIdentity = AttributedModelServices.GetTypeIdentity(closedGenericContractType);
                if (typeIdentity == (string) exportDefinition.Metadata[CompositionConstants.ExportTypeIdentityMetadataName])
                {
                    return definition;
                }

                var contractName = AttributedModelServices.GetContractName(ContractType);

                // If both are equal, contract name has to be rewritten
                contractName = contractName == exportDefinition.ContractName ? AttributedModelServices.GetContractName(closedGenericContractType) : exportDefinition.ContractName;

                var metadata = new Dictionary<string,object>();
                foreach (var key in exportDefinition.Metadata.Keys.Where(key => key != CompositionConstants.ExportTypeIdentityMetadataName))
                {
                    metadata.Add(key, exportDefinition.Metadata[key]);
                }
                metadata.Add(CompositionConstants.ExportTypeIdentityMetadataName, typeIdentity);

                var rewrittenExport = ReflectionModelServices.CreateExportDefinition(
                    new LazyMemberInfo(ExportingType),
                    contractName,
                    new Lazy<IDictionary<string, object>>(() => metadata),
                    null);
                
                var exports = new List<ExportDefinition>();
                exports.Add(rewrittenExport);
                exports.AddRange(definition.ExportDefinitions.Skip(1));

                var part = ReflectionModelServices.CreatePartDefinition(
                    new Lazy<Type>(() => ExportingType),
                    ReflectionModelServices.IsDisposalRequired(definition),
                    new Lazy<IEnumerable<ImportDefinition>>(() => definition.ImportDefinitions),
                    new Lazy<IEnumerable<ExportDefinition>>(() => exports),
                    new Lazy<IDictionary<string, object>>(() => definition.Metadata),
                    null);

                return part;
            }
            
            return definition;
        }
    }
}