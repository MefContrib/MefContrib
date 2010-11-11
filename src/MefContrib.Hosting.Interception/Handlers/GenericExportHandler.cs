namespace MefContrib.Hosting.Interception.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;

    public class GenericExportHandler : IExportHandler
    {
        private ComposablePartCatalog decoratedCatalog;
        private readonly AggregateCatalog catalog;
        private readonly IDictionary<Type, Type> genericTypes;
        private readonly List<Type> manufacturedParts;

        public GenericExportHandler()
        {
            this.catalog = new AggregateCatalog();
            this.genericTypes = new Dictionary<Type, Type>();
            this.manufacturedParts = new List<Type>();
        }

        #region IExportHandler Members

        public void Initialize(ComposablePartCatalog interceptedCatalog)
        {
            this.decoratedCatalog = interceptedCatalog;
            LoadTypeMappings();
        }

        private void LoadTypeMappings()
        {
            using (var ep = new CatalogExportProvider(this.decoratedCatalog))
            {
                ep.SourceProvider = ep;
                var locators = ep.GetExportedValues<IGenericContractRegistry>();
                
                foreach (var mapping in locators.SelectMany(locator => locator.GetMappings()))
                {
                    this.genericTypes.Add(
                        mapping.GenericContractTypeDefinition,
                        mapping.GenericImplementationTypeDefinition);
                }
            }
        }

        private void CreateGenericPart(Type importDefinitionType)
        {
            var type = TypeHelper.BuildGenericType(importDefinitionType, this.genericTypes);
            
            this.manufacturedParts.Add(type);
            this.catalog.Catalogs.Add(new TypeCatalog(type));
        }

        public IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition, IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> exports)
        {
            var contractDef = (ContractBasedImportDefinition)definition;
            var returnedExports = new List<Tuple<ComposablePartDefinition, ExportDefinition>>();
            var importDefinitionType = TypeHelper.GetImportDefinitionType(definition);

            if (exports.Any())
            {
                return exports;
            }

            returnedExports.Concat(exports);

            if (this.manufacturedParts.Contains(importDefinitionType))
            {
                returnedExports.AddRange(this.catalog.GetExports(definition));
            }
            else if (TypeHelper.ShouldCreateClosedGenericPart(contractDef, importDefinitionType))
            {
                CreateGenericPart(importDefinitionType);
                returnedExports.AddRange(this.catalog.GetExports(definition));
            }
            
            return returnedExports;
        }

        #endregion
    }
}
