using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace MefContrib.Hosting.Generics
{
    public class GenericCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        private ComposablePartCatalog _decoratedCatalog;
        private AggregateCatalog _catalog = new AggregateCatalog();
        private IDictionary<Type, Type> _genericTypes = new Dictionary<Type, Type>();

        public GenericCatalog(ComposablePartCatalog catalog)
        {
            _decoratedCatalog = catalog;
            _catalog.Catalogs.Add(_decoratedCatalog);
            _catalog.Changing += (s, e) => { OnChanging(e); };
            LoadTypeLocators(_genericTypes);
        }

        private void OnChanging(ComposablePartCatalogChangeEventArgs e)
        {
            var handler = Changing;
            if (handler != null)
                handler(this, e);

            //changed is not used at all
        }

        public override IQueryable<ComposablePartDefinition> Parts
        {
            get { return _catalog.Parts; }
        }

        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            var exports = _catalog.GetExports(definition);

            if (!exports.Any() && TypeHelper.IsReflectionMemberImportDefinition(definition)) 
                exports = CreateDynamicExport(definition);

            return exports;
        }

        private void LoadTypeLocators(IDictionary<Type, Type> genericTypes)
        {
            using (var ep = new CatalogExportProvider(_decoratedCatalog))
            {
                ep.SourceProvider = ep;
                var locators = ep.GetExportedValues<GenericContractTypeMapping>();
                foreach (var locator in locators)
                    genericTypes.Add(locator.GenericContractTypeDefinition, locator.GenericImplementationTypeDefinition);
            }
        }

        private IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> CreateDynamicExport(ImportDefinition definition)
        {
            IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> exports = null;
            var contractDef = (ContractBasedImportDefinition) definition;
            Type importDefinitionType = TypeHelper.GetImportDefinitionType(definition);
            
            if (importDefinitionType.IsGenericType)
            {
                if (TypeHelper.ShouldCreateClosedGenericPart(contractDef))
                {
                    CreateGenericPart(importDefinitionType);
                    exports = _catalog.GetExports(definition);
                    if (exports.Any())
                        return exports;    
                }
            }

            if (importDefinitionType.IsClass)
            {
                CreateConcretePart(importDefinitionType);
                exports = _catalog.GetExports(definition);
            }
            return exports;
        }

        private void CreateConcretePart(Type importDefinitionType)
        {
            var typeCatalog = new TypeCatalog(importDefinitionType);
            _catalog.Catalogs.Add(typeCatalog);
        }

        private void CreateGenericPart(Type importDefinitionType)
        {
            var type = TypeHelper.BuildGenericType(importDefinitionType, _genericTypes);
            var typeCatalog = new TypeCatalog(type);
            _catalog.Catalogs.Add(typeCatalog);
        }

        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;
    }
}