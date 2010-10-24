namespace MefContrib.Hosting.Interception.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;

    public class GenericExportHandler : IExportHandler
    {
        private ComposablePartCatalog _decoratedCatalog;
        private readonly AggregateCatalog _catalog = new AggregateCatalog();
        private readonly IDictionary<Type, Type> _genericTypes = new Dictionary<Type, Type>();
        private readonly List<Type> _manufacturedParts = new List<Type>();

        #region IExportHandler Members

        public void Initialize(ComposablePartCatalog catalog)
        {
            _decoratedCatalog = catalog;
            LoadTypeMappings(_genericTypes);
        }

        private void LoadTypeMappings(IDictionary<Type, Type> genericTypes)
        {
            using (var ep = new CatalogExportProvider(_decoratedCatalog))
            {
                ep.SourceProvider = ep;
                var locators = ep.GetExportedValues<GenericContractTypeMapping>();
                foreach (var locator in locators)
                {
                    genericTypes.Add(
                        locator.GenericContractTypeDefinition,
                        locator.GenericImplementationTypeDefinition);
                }
            }
        }

        private void CreateGenericPart(Type importDefinitionType)
        {
            var type = TypeHelper.BuildGenericType(importDefinitionType, _genericTypes);
            
            _manufacturedParts.Add(type);
            _catalog.Catalogs.Add(new TypeCatalog(type));
        }

        public IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition, IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> exports)
        {
            var contractDef = (ContractBasedImportDefinition)definition;
            var returnedExports = new List<Tuple<ComposablePartDefinition, ExportDefinition>>();
            var importDefinitionType = TypeHelper.GetImportDefinitionType(definition);
            
            if (exports.Any())
                return exports;

            returnedExports.Concat(exports);
            
            if (_manufacturedParts.Contains(importDefinitionType))
                returnedExports.AddRange(_catalog.GetExports(definition));
            else if (TypeHelper.ShouldCreateClosedGenericPart(contractDef, importDefinitionType))
            {
                CreateGenericPart(importDefinitionType);
                returnedExports.AddRange(_catalog.GetExports(definition));
            }
            
            return returnedExports;
        }

        #endregion
    }
}
