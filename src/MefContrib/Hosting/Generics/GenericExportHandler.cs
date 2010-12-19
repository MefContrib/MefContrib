namespace MefContrib.Hosting.Generics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using MefContrib.Hosting.Interception;

    /// <summary>
    /// Defines an export handler which enables open generics support.
    /// </summary>
    public class GenericExportHandler : IExportHandler
    {
        private ComposablePartCatalog decoratedCatalog;
        private readonly AggregateCatalog aggregateCatalog;
        private readonly IDictionary<Type, ISet<Type>> genericTypeMapping;
        private readonly List<Type> manufacturedParts;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericExportHandler"/> class.
        /// </summary>
        /// <param name="registries">A collection of <see cref="IGenericContractRegistry"/> instances.</param>
        public GenericExportHandler(params IGenericContractRegistry[] registries)
        {
            this.aggregateCatalog = new AggregateCatalog();
            this.genericTypeMapping = new Dictionary<Type, ISet<Type>>();
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
                    var contractType = mapping.GenericContractTypeDefinition;
                    var implementationType = mapping.GenericImplementationTypeDefinition;

                    ISet<Type> implementationTypes;
                    if (!this.genericTypeMapping.TryGetValue(contractType, out implementationTypes))
                    {
                        implementationTypes = new HashSet<Type>();
                        this.genericTypeMapping.Add(contractType, implementationTypes);
                    }

                    implementationTypes.Add(implementationType);
                }
            }
        }

        private void CreateGenericParts(Type importDefinitionType)
        {
            var genericImportTypeDefinition = importDefinitionType.GetGenericTypeDefinition();
            var genericImportTypeImplementations = new List<Type>();
            if (!this.genericTypeMapping.ContainsKey(genericImportTypeDefinition))
            {
                if (importDefinitionType.IsClass && !importDefinitionType.IsAbstract)
                {
                    genericImportTypeImplementations.Add(genericImportTypeDefinition);
                }
                else
                {
                    throw new MappingNotFoundException(
                        genericImportTypeDefinition,
                        string.Format("Implementation type for {0} has not been found.",
                            genericImportTypeDefinition.Name));
                }
            }
            else
            {
                genericImportTypeImplementations.AddRange(
                    this.genericTypeMapping[genericImportTypeDefinition]);
            }
            
            var concreteParts = TypeHelper.BuildGenericTypes(importDefinitionType, genericImportTypeImplementations);
            foreach (var type in concreteParts)
            {
                this.aggregateCatalog.Catalogs.Add(new GenericTypeCatalog(type, genericImportTypeDefinition));
            }
            this.manufacturedParts.Add(importDefinitionType);
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

            if (!CompositionServices.IsReflectionImportDefinition(definition))
            {
                return Enumerable.Empty<Tuple<ComposablePartDefinition, ExportDefinition>>();
            }

            var returnedExports = new List<Tuple<ComposablePartDefinition, ExportDefinition>>();
            var importDefinitionType = CompositionServices.GetImportDefinitionType(definition);
            
            if (TypeHelper.IsGenericCollection(importDefinitionType))
            {
                importDefinitionType = TypeHelper.GetGenericCollectionParameter(importDefinitionType);
            }

            if (this.manufacturedParts.Contains(importDefinitionType))
            {
                returnedExports.AddRange(this.aggregateCatalog.GetExports(definition));
            }
            else if (TypeHelper.ShouldCreateClosedGenericPart(importDefinitionType))
            {
                CreateGenericParts(importDefinitionType);
                returnedExports.AddRange(this.aggregateCatalog.GetExports(definition));
            }
            
            return returnedExports;
        }

        #endregion
    }
}
