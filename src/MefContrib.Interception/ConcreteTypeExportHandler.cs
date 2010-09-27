using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using MefContrib.Interception.Generics;

namespace MefContrib.Interception
{
    public class ConcreteTypeExportHandler : IExportHandler
    {
        private AggregateCatalog _catalog;

        public ConcreteTypeExportHandler()
        {
            _catalog = new AggregateCatalog();
        }

        public void Initialize(System.ComponentModel.Composition.Primitives.ComposablePartCatalog catalog)
        {
            
        }

        public IEnumerable<Tuple<System.ComponentModel.Composition.Primitives.ComposablePartDefinition, System.ComponentModel.Composition.Primitives.ExportDefinition>> GetExports(System.ComponentModel.Composition.Primitives.ImportDefinition definition, IEnumerable<Tuple<System.ComponentModel.Composition.Primitives.ComposablePartDefinition, System.ComponentModel.Composition.Primitives.ExportDefinition>> exports)
        {
            if (!exports.Any())
                exports = _catalog.GetExports(definition);
                        
            if (exports.Any())
                return exports;
            
            var contractDef = (ContractBasedImportDefinition)definition;
            List<Tuple<ComposablePartDefinition, ExportDefinition>> returnedExports = new List<Tuple<ComposablePartDefinition, ExportDefinition>>();
            
            Type importDefinitionType = TypeHelper.GetImportDefinitionType(definition);
            
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
