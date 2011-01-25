namespace MefContrib.Web.Mvc
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.ComponentModel.Composition.ReflectionModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;

    /// <summary>
    /// MvcApplicationCatalog
    /// </summary>
    public class MvcApplicationCatalog
        : ComposablePartCatalog, ICompositionElement
    {
        private List<Type> types = new List<Type>();
        private List<ComposablePartDefinition> parts = new List<ComposablePartDefinition>();
        private readonly object synclock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="MvcApplicationCatalog"/> class.
        /// </summary>
        public MvcApplicationCatalog()
        {
            RegisterReferencedAssemblies();
            RegisterReferencedControllers();
        }

        /// <summary>
        /// Registers referenced assemblies.
        /// </summary>
        protected void RegisterReferencedAssemblies()
        {
            lock (synclock)
            {
                DirectoryCatalog directoryCatalog
                    = new DirectoryCatalog(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin"));
                parts.AddRange(directoryCatalog.Parts);
            }
        }

        /// <summary>
        /// Registers the referenced controllers.
        /// </summary>
        protected void RegisterReferencedControllers()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                RegisterControllersInAssembly(assembly);
            }
        }

        /// <summary>
        /// Registers controllers in assembly.
        /// </summary>
        public void RegisterControllersInAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                RegisterControllersInType(type);
            }
        }

        /// <summary>
        /// Registers controllers in type.
        /// </summary>
        protected void RegisterControllersInType(Type type)
        {
            lock (synclock)
            {
                if (!type.IsAbstract && !type.IsInterface && typeof(IController).IsAssignableFrom(type))
                {
                    ComposablePartDefinition part = ReflectionModelServices.CreatePartDefinition(
                        new Lazy<Type>(() => type),
                        true,
                        new Lazy<IEnumerable<ImportDefinition>>(() => GetImportDefinitions(type)),
                        new Lazy<IEnumerable<ExportDefinition>>(() => GetExportDefinitions(type, type)),
                        new Lazy<IDictionary<string, object>>(() => GetMetadata(type)),
                        this
                    );
                    parts.Add(part);
                }
            }
        }

        /// <summary>
        /// Gets the import definitions.
        /// </summary>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <returns></returns>
        protected ImportDefinition[] GetImportDefinitions(Type implementationType)
        {
            // Find importing constructor
            var constructor = implementationType.GetConstructors()[0];
            var importingConstructor = implementationType.GetConstructors()
                .FirstOrDefault(c => c.GetCustomAttributes(typeof(ImportingConstructorAttribute), false).Length > 0);
            if (importingConstructor != null)
            {
                constructor = importingConstructor;
            }

            // Find imports
            var imports = new List<ImportDefinition>();

            foreach (var param in constructor.GetParameters())
            {
                var cardinality = GetCardinality(param);
                var paramInfo = param;
                var importType = cardinality == ImportCardinality.ZeroOrMore ? GetCollectionContractType(param.ParameterType) : param.ParameterType;

                imports.Add(
                    ReflectionModelServices.CreateImportDefinition(
                        new Lazy<ParameterInfo>(() => paramInfo),
                        AttributedModelServices.GetContractName(importType),
                        AttributedModelServices.GetTypeIdentity(importType),
                        Enumerable.Empty<KeyValuePair<string, Type>>(),
                        cardinality,
                        CreationPolicy.Any,
                        null));

            }
            return imports.ToArray();
        }

        /// <summary>
        /// Gets the cardinality.
        /// </summary>
        /// <param name="param">The param.</param>
        /// <returns></returns>
        private ImportCardinality GetCardinality(ParameterInfo param)
        {
            if (typeof(IEnumerable).IsAssignableFrom(param.ParameterType))
                return ImportCardinality.ZeroOrMore;
            else
                return ImportCardinality.ExactlyOne;
        }

        /// <summary>
        /// Gets the type of the collection contract.
        /// </summary>
        /// <param name="collectionType">Type of the collection.</param>
        /// <returns></returns>
        private Type GetCollectionContractType(Type collectionType)
        {
            var itemType = collectionType.GetGenericArguments().First();
            var contractType = itemType.GetGenericArguments().First();
            return contractType;
        }

        /// <summary>
        /// Gets the export definitions.
        /// </summary>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns></returns>
        private ExportDefinition[] GetExportDefinitions(Type implementationType, Type contractType)
        {
            var lazyMember = new LazyMemberInfo(implementationType);
            var contracName = AttributedModelServices.GetContractName(contractType);
            var metadata = new Lazy<IDictionary<string, object>>(() =>
            {
                var md = new Dictionary<string, object>();
                md.Add(CompositionConstants.ExportTypeIdentityMetadataName, AttributedModelServices.GetTypeIdentity(contractType));
                return md;
            });

            return new ExportDefinition[] { ReflectionModelServices.CreateExportDefinition(lazyMember, contracName, metadata, null) };
        }


        /// <summary>
        /// Gets the metadata.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private IDictionary<string, object> GetMetadata(Type type)
        {
            var metadata = new Dictionary<string, object>();
            metadata[CompositionConstants.ExportTypeIdentityMetadataName] = type.Name;
            metadata[CompositionConstants.PartCreationPolicyMetadataName] = CreationPolicy.NonShared;
            return metadata;
        }

        /// <summary>
        /// Gets the part definitions that are contained in the catalog.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartDefinition"/> contained in the <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartCatalog"/>.</returns>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePartCatalog"/> object has been disposed of.</exception>
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get { return parts.AsQueryable(); }
        }

        /// <summary>
        /// Gets the display name of the composition element.
        /// </summary>
        /// <value></value>
        /// <returns>The human-readable display name of the <see cref="T:System.ComponentModel.Composition.Primitives.ICompositionElement"/>.</returns>
        public string DisplayName
        {
            get { return "MvcApplicationCatalog"; }
        }

        /// <summary>
        /// Gets the composition element from which the current composition element originated.
        /// </summary>
        /// <value></value>
        /// <returns>The composition element from which the current <see cref="T:System.ComponentModel.Composition.Primitives.ICompositionElement"/> originated, or null if the <see cref="T:System.ComponentModel.Composition.Primitives.ICompositionElement"/> is the root composition element.</returns>
        public ICompositionElement Origin
        {
            get { return null; }
        }
    }
}
