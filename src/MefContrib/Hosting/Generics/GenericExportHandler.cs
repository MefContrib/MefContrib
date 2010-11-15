namespace MefContrib.Hosting.Generics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using MefContrib.Hosting.Interception;
    using MefContrib.Hosting.Interception.Handlers;

    /// <summary>
    /// Defines an export handler which enables open generics support.
    /// </summary>
    public class GenericExportHandler : IExportHandler
    {
        private ComposablePartCatalog decoratedCatalog;
        private readonly AggregateCatalog aggregateCatalog;
        private readonly IDictionary<Type, Type> genericTypeMapping;
        private readonly List<Type> manufacturedParts;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericExportHandler"/> class.
        /// </summary>
        /// <param name="registries">A collection of <see cref="IGenericContractRegistry"/> instances.</param>
        public GenericExportHandler(params IGenericContractRegistry[] registries)
        {
            this.aggregateCatalog = new AggregateCatalog();
            this.genericTypeMapping = new Dictionary<Type, Type>();
            this.manufacturedParts = new List<Type>();

            LoadTypeMappings(registries);
        }

        #region IExportHandler Members

        /// <summary>
        /// Initializes this export handler.
        /// </summary>
        /// <param name="interceptedCatalog">The <see cref="ComposablePartCatalog"/> being intercepted.</param>
        public void Initialize(ComposablePartCatalog interceptedCatalog)
        {
            this.decoratedCatalog = interceptedCatalog;
            LoadRegistriesFromCatalog();
        }

        private void LoadRegistriesFromCatalog()
        {
            using (var ep = new CatalogExportProvider(this.decoratedCatalog))
            {
                ep.SourceProvider = ep;
                var registries = ep.GetExportedValues<IGenericContractRegistry>();
                
                LoadTypeMappings(registries);
            }
        }
        
        private void LoadTypeMappings(IEnumerable<IGenericContractRegistry> registries)
        {
            if (registries != null)
            {
                foreach (var mapping in registries.SelectMany(registry => registry.GetMappings()))
                {
                    this.genericTypeMapping.Add(
                        mapping.GenericContractTypeDefinition,
                        mapping.GenericImplementationTypeDefinition);
                }
            }
        }

        private void CreateGenericPart(Type importDefinitionType)
        {
            var type = TypeHelper.BuildGenericType(importDefinitionType, this.genericTypeMapping);

            this.manufacturedParts.Add(importDefinitionType);
            this.aggregateCatalog.Catalogs.Add(new TypeCatalog(type));
        }

        /// <summary>
        /// Method which can filter exports for given <see cref="ImportDefinition"/> or produce new exports.
        /// </summary>
        /// <param name="definition"><see cref="ImportDefinition"/> instance.</param>
        /// <param name="exports">A collection of <see cref="ExportDefinition"/>
        /// instances along with their <see cref="ComposablePartDefinition"/> instances which match given <see cref="ImportDefinition"/>.</param>
        /// <returns>A collection of <see cref="ExportDefinition"/>
        /// instances along with their <see cref="ComposablePartDefinition"/> instances which match given <see cref="ImportDefinition"/>.</returns>
        public IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition, IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> exports)
        {
            if (exports.Any())
            {
                return exports;
            }

            if (!TypeHelper.IsReflectionImportDefinition(definition))
            {
                return Enumerable.Empty<Tuple<ComposablePartDefinition, ExportDefinition>>();
            }

            var contractDef = (ContractBasedImportDefinition)definition;
            var returnedExports = new List<Tuple<ComposablePartDefinition, ExportDefinition>>();
            var importDefinitionType = TypeHelper.GetImportDefinitionType(definition);
            
            if (this.manufacturedParts.Contains(importDefinitionType))
            {
                returnedExports.AddRange(this.aggregateCatalog.GetExports(definition));
            }
            else if (TypeHelper.ShouldCreateClosedGenericPart(contractDef, importDefinitionType))
            {
                CreateGenericPart(importDefinitionType);
                returnedExports.AddRange(this.aggregateCatalog.GetExports(definition));
            }
            
            return returnedExports;
        }

        #endregion
    }
}
