namespace MefContrib.Hosting.Interception
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using MefContrib.Hosting.Interception.Generics;

    public class ConcreteTypeExportHandler : IExportHandler
    {
        private readonly AggregateCatalog _catalog;

        public ConcreteTypeExportHandler()
        {
            _catalog = new AggregateCatalog();
        }

        public void Initialize(ComposablePartCatalog catalog)
        {
        }

        public IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition, IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> exports)
        {
            if (!exports.Any())
                exports = _catalog.GetExports(definition);
                        
            if (exports.Any())
                return exports;
            
            var returnedExports = new List<Tuple<ComposablePartDefinition, ExportDefinition>>();
            var importDefinitionType = TypeHelper.GetImportDefinitionType(definition);
            
            if (!importDefinitionType.IsAbstract && !importDefinitionType.IsInterface)
            {
                var catalog = new TypeCatalog(importDefinitionType);
                _catalog.Catalogs.Add(catalog);
                var tempExport = _catalog.GetExports(definition);
                returnedExports.AddRange(tempExport);
            }
            
            return returnedExports;
        }
    }
}
