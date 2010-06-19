namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.ComponentModel.Composition.ReflectionModel;
    using System.Linq;
    using System.Reflection;
    using MefContrib.Hosting.Conventions.Configuration;

    /// <summary>
    /// 
    /// </summary>
    public class ConventionPartCreator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionPartCreator"/> class.
        /// </summary>
        /// <param name="registry">The <see cref="IPartRegistry{T}"/> instance that should be used by the part creator.</param>
        /// <exception cref="ArgumentNullException">The provided <paramref name="registry"/> was <see langword="null"/>.</exception>
        public ConventionPartCreator(IPartRegistry<IContractService> registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry", "The registry cannot be null.");
            }

            this.Registry = registry;
        }

        /// <summary>
        /// Gets the <see cref="IPartRegistry{T}"/> instance used by the part creator.
        /// </summary>
        /// <value>An <see cref="IPartRegistry{T}"/> instance.</value>
        public IPartRegistry<IContractService> Registry { get; private set; }

        /// <summary>
        /// Gets the <see cref="ITypeLoader"/> used by the <see cref="Registry"/>.
        /// </summary>
        /// <value>An <see cref="ITypeLoader"/> instance.</value>
        private ITypeLoader Loader
        {
            get { return this.Registry.TypeLoader; }
        }

        /// <summary>
        /// Gets the <see cref="IContractService"/> used by the <see cref="Registry"/>.
        /// </summary>
        /// <value>An <see cref="IContractService"/> instance.</value>
        private IContractService ContractService
        {
            get { return this.Registry.ContractService; }
        }

        /// <summary>
        /// Creates <see cref="ComposablePartDefinition"/> instances from the <see cref="IPartConvention"/> and types.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/>, containing <see cref="ComposablePartDefinition"/> instances.</returns>
        public IEnumerable<ComposablePartDefinition> CreateParts()
        {
            if (this.Loader == null)
            {
                return Enumerable.Empty<ComposablePartDefinition>();
            }

            var definitionsFromConventions =
               from convention in this.Registry.GetConventions()
               from type in this.Loader.GetTypes(convention.Condition)
               select CreatePartDefinition(convention, type);

            return definitionsFromConventions.ToList();
        }

        /// <summary>
        /// Creates <see cref="ExportDefinition"/> instance from the provided <see cref="IExportConvention"/> instances and type.
        /// </summary>
        /// <param name="exportConventions">An <see cref="IEnumerable{T}"/> of <see cref="IExportConvention"/> instances that should be used to create the <see cref="ExportDefinition"/> instances.</param>
        /// <param name="type">The <see cref="Type"/> for which the <see cref="ExportDefinition"/> instances should be created.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ExportDefinition"/> instances.</returns>
        private IEnumerable<ExportDefinition> CreateExportDefinitions(IEnumerable<IExportConvention> exportConventions, Type type)
        {
            var exportDefinitionsFromConvention =
                from exportConvention in exportConventions
                from member in exportConvention.Members.Invoke(type)
                select ReflectionModelServices.CreateExportDefinition(
                    member.ToLazyMemberInfo(),
                    this.ContractService.GetExportContractName(exportConvention, member),
                    new Lazy<IDictionary<string, object>>(() => GetExportDefinitionMetadata(exportConvention, member)),
                    null);

            return exportDefinitionsFromConvention.ToList();
        }

        /// <summary>
        /// Creates <see cref="ImportDefinition"/> instance from the provided <see cref="IImportConvention"/> instances and type.
        /// </summary>
        /// <param name="importConventions">An <see cref="IEnumerable{T}"/> of <see cref="IImportConvention"/> instances that should be used to create the <see cref="ImportDefinition"/> instances.</param>
        /// <param name="type">The <see cref="Type"/> for which the <see cref="ImportDefinition"/> instances should be created.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ImportDefinition"/> instances.</returns>
        private IEnumerable<ImportDefinition> CreateImportDefinitions(IEnumerable<IImportConvention> importConventions, Type type)
        {
            var importDefinitionsFromConvention = new List<ImportDefinition>();

            foreach (var importConvention in importConventions)
            {
                foreach (var member in importConvention.Members.Invoke(type))
                {
                    if (member is ConstructorInfo)
                    {
                        importDefinitionsFromConvention.AddRange(((ConstructorInfo)member).GetParameters().Select(parameter => GetImportParameterDefinition(importConvention, parameter)));
                    }
                    else
                    {
                        importDefinitionsFromConvention.Add(GetImportDefinition(importConvention, member));
                    }
                }
            }

            return importDefinitionsFromConvention.ToList();
        }

        /// <summary>
        /// Gets an <see cref="ImportDefinition"/> for a <see cref="ParameterInfo"/> instance using the provided <see cref="IImportConvention"/> instance.
        /// </summary>
        /// <param name="importConvention"><see cref="IImportConvention"/> instances that should be used to create the <see cref="ImportDefinition"/> instances.</param>
        /// <param name="parameter">The <see cref="ParameterInfo"/> for which the <see cref="ImportDefinition"/> instances should be created.</param>
        /// <returns>An <see cref="ImportDefinition"/> instance.</returns>
        private ImportDefinition GetImportParameterDefinition(IImportConvention importConvention, ParameterInfo parameter)
        {
            var actualType =
                parameter.ParameterType.GetActualType();

            return ReflectionModelServices.CreateImportDefinition(
                new Lazy<ParameterInfo>(() => parameter),
                this.ContractService.GetImportContractName(importConvention, actualType),
                this.ContractService.GetImportTypeIdentity(importConvention, actualType),
                null,
                parameter.ParameterType.GetCardinality(importConvention.AllowDefaultValue),
                importConvention.CreationPolicy,
                null);
        }

        /// <summary>
        /// Gets an <see cref="ImportDefinition"/> for a <see cref="MemberInfo"/> instance using the provided <see cref="IImportConvention"/> instance.
        /// </summary>
        /// <param name="importConvention"><see cref="IImportConvention"/> instances that should be used to create the <see cref="ImportDefinition"/> instances.</param>
        /// <param name="member">The <see cref="MemberInfo"/> for which the <see cref="ImportDefinition"/> instances should be created.</param>
        /// <returns>An <see cref="ImportDefinition"/> instance.</returns>
        private ImportDefinition GetImportDefinition(IImportConvention importConvention, MemberInfo member)
        {
            return ReflectionModelServices.CreateImportDefinition(
                member.ToLazyMemberInfo(),
                this.ContractService.GetImportContractName(importConvention, member),
                this.ContractService.GetImportTypeIdentity(importConvention, member),
                importConvention.RequiredMetadata.Select(x => new KeyValuePair<string, Type>(x.Name, x.Type)),
                member.GetCardinality(importConvention.AllowDefaultValue),
                importConvention.Recomposable,
                importConvention.CreationPolicy,
                null);
        }

        /// <summary>
        /// Create a <see cref="ComposablePartDefinition"/> for a specified type using the provided <see cref="IPartConvention"/>.
        /// </summary>
        /// <param name="convention">The <see cref="IPartConvention"/> instance which is used to create the <see cref="ComposablePartDefinition"/>.</param>
        /// <param name="type">The <see cref="Type"/> for which the <see cref="ComposablePartDefinition"/> should be created.</param>
        /// <returns>A <see cref="ComposablePartDefinition"/> instance.</returns>
        private ComposablePartDefinition CreatePartDefinition(IPartConvention convention, Type type)
        {
            return ReflectionModelServices.CreatePartDefinition(
                    new Lazy<Type>(() => type),
                    false,
                    new Lazy<IEnumerable<ImportDefinition>>(() => CreateImportDefinitions(convention.Imports, type)),
                    new Lazy<IEnumerable<ExportDefinition>>(() => CreateExportDefinitions(convention.Exports, type)),
                    new Lazy<IDictionary<string, object>>(() => GetPartDefinitionMetadata(convention)),
                    null);
        }

        /// <summary>
        /// Gets the metadata for the provided <see cref="IExportConvention"/>.
        /// </summary>
        /// <param name="exportConvention">The <see cref="IExportConvention"/> that the metadata should be retrieved for.</param>
        /// <param name="member">The <see cref="MemberInfo"/> that is being exported.</param>
        /// <returns>An <see cref="IDictionary{TKey,TValue}"/> containing the metadata for the export.</returns>
        private IDictionary<string, object> GetExportDefinitionMetadata(IExportConvention exportConvention, MemberInfo member)
        {
            var exportDefinitionMetadata =
                exportConvention.Metadata.ToMetadataDictionary();

            exportDefinitionMetadata.Add(
                CompositionConstants.ExportTypeIdentityMetadataName,
                this.ContractService.GetExportTypeIdentity(exportConvention, member));

            return exportDefinitionMetadata;
        }

        /// <summary>
        /// Gets the metadata for the provided <see cref="IPartConvention"/>.
        /// </summary>
        /// <param name="partConvention">The <see cref="IPartConvention"/> that the metadata should be retrieved for.</param>
        /// <returns>An <see cref="IDictionary{TKey,TValue}"/> containing the metadata for the part.</returns>
        private static IDictionary<string, object> GetPartDefinitionMetadata(IPartConvention partConvention)
        {
            var partDefinitionMetadata =
                partConvention.Metadata.ToMetadataDictionary();

            partDefinitionMetadata.Add(CompositionConstants.PartCreationPolicyMetadataName, partConvention.CreationPolicy);

            return partDefinitionMetadata;
        }
    }
}