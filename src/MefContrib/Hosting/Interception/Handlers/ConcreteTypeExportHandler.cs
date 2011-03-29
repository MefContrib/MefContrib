namespace MefContrib.Hosting.Interception.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;

    public class ConcreteTypeExportHandler : IExportHandler
    {
        private readonly AggregateCatalog catalog;

        public ConcreteTypeExportHandler()
        {
            this.catalog = new AggregateCatalog();
        }

        public void Initialize(ComposablePartCatalog interceptedCatalog)
        {
        }

        public IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition, IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> exports)
        {
            if (!exports.Any())
                exports = this.catalog.GetExports(definition);
                        
            if (exports.Any())
                return exports;
            
            var returnedExports = new List<Tuple<ComposablePartDefinition, ExportDefinition>>();
            var importDefinitionType = ContractServices.GetImportDefinitionType(definition);
            
            if (!importDefinitionType.IsAbstract && !importDefinitionType.IsInterface)
            {
                var typeCatalog = new TypeCatalog(importDefinitionType);
                this.catalog.Catalogs.Add(typeCatalog);
                var currentExports = this.catalog.GetExports(definition);
                returnedExports.AddRange(currentExports);
            }
            
            return returnedExports;
        }
    }
}
