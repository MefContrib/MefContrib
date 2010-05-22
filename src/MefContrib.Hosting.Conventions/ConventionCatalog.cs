namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.ComponentModel.Composition.ReflectionModel;
    using System.Linq;
    using System.Reflection;
    using MefContrib.Hosting.Conventions.Configuration;

    /// <summary>
    /// Defines the class for composable part catalogs, which produce and return <see cref="ComposablePartDefinition"/> objects based on conventions.
    /// </summary>
    public class ConventionCatalog : ComposablePartCatalog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionCatalog"/> class, using the
        /// provided part convention registries and type loader.
        /// </summary>
        /// <param name="registries">An <see cref="IEnumerable{T}"/> instance, containing part convention registries.</param>
        /// <param name="typeLoader">An <see cref="ITypeLoader"/> instance.</param>
        public ConventionCatalog(IEnumerable<IConventionRegistry<IPartConvention>> registries, ITypeLoader typeLoader)
            : this(registries.SelectMany(x => x.GetConventions()), typeLoader)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionCatalog"/> class, using the
        /// provided part conventions and type loader.
        /// </summary>
        /// <param name="conventions">An <see cref="IEnumerable{T}"/> instance, containing part conventions.</param>
        /// <param name="typeLoader">An <see cref="ITypeLoader"/> instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="conventions"/> or <paramref name="typeLoader"/> parameter was null.</exception>
        public ConventionCatalog(IEnumerable<IPartConvention> conventions, ITypeLoader typeLoader)
        {
            if (conventions == null)
            {
                throw new ArgumentNullException("conventions", "The conventions cannot be null.");
            }

            if (typeLoader == null)
            {
                throw new ArgumentNullException("typeLoader", "The type loader cannot be null.");
            }

            this.Conventions = conventions;
            this.TypeLoader = typeLoader;
        }

        /// <summary>
        /// Gets the part definitions of the catalog.
        /// </summary>
        /// <value>A <see cref="IQueryable{T}"/> of <see cref="ComposablePartDefinition"/> objects of the <see cref="ConventionCatalog"/>.</value>
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get { return this.CreateParts().AsQueryable(); }
        }

        /// <summary>
        /// Gets the <see cref="IPartConvention"/> instancs that has been registered with the catalog.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> instance, containing <see cref="IPartConvention"/> objects.</value>
        public IEnumerable<IPartConvention> Conventions { get; private set; }

        /// <summary>
        /// Get the <see cref="ITypeLoader"/> instance that has been registered with the catalog.
        /// </summary>
        /// <value>An <see cref="ITypeLoader"/> instance.</value>
        public ITypeLoader TypeLoader { get; private set; }

        /// <summary>
        /// Creates <see cref="ExportDefinition"/> instance from the provided <see cref="IExportConvention"/> instances and type.
        /// </summary>
        /// <param name="exportConventions">An <see cref="IEnumerable{T}"/> of <see cref="IExportConvention"/> instances that should be used to create the <see cref="ExportDefinition"/> instances.</param>
        /// <param name="type">The <see cref="Type"/> for which the <see cref="ExportDefinition"/> instances should be created.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ExportDefinition"/> instances.</returns>
        private static IEnumerable<ExportDefinition> CreateExportDefinitions(IEnumerable<IExportConvention> exportConventions, Type type)
        {
            var exportDefinitionsFromConvention =
                from exportConvention in exportConventions
                from member in exportConvention.Members.Invoke(type)
                select ReflectionModelServices.CreateExportDefinition(
                    member.ToLazyMemberInfo(),
                    GetExportContractName(exportConvention, member),
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
        private static IEnumerable<ImportDefinition> CreateImportDefinitions(IEnumerable<IImportConvention> importConventions, Type type)
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
        private static ImportDefinition GetImportParameterDefinition(IImportConvention importConvention, ParameterInfo parameter)
        {
            return ReflectionModelServices.CreateImportDefinition(
                new Lazy<ParameterInfo>(() => parameter),
                AttributedModelServices.GetContractName(parameter.ParameterType),
                AttributedModelServices.GetTypeIdentity(parameter.ParameterType),
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
        private static ImportDefinition GetImportDefinition(IImportConvention importConvention, MemberInfo member)
        {
            return ReflectionModelServices.CreateImportDefinition(
                member.ToLazyMemberInfo(),
                GetImportContractName(importConvention, member),
                GetImportTypeIdentity(importConvention, member),
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
        private static ComposablePartDefinition CreatePartDefinition(IPartConvention convention, Type type)
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
        /// Creates <see cref="ComposablePartDefinition"/> instances from the <see cref="IPartConvention"/> and types.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/>, containing <see cref="ComposablePartDefinition"/> instances.</returns>
        private IEnumerable<ComposablePartDefinition> CreateParts()
        {
            var definitionsFromConventions =
                from convention in this.Conventions
                from type in this.TypeLoader.GetTypes(convention.Condition)
                select CreatePartDefinition(convention, type);

            return definitionsFromConventions.ToList();
        }

        /// <summary>
        /// Gets contract name for the provided <see cref="IExportConvention"/>.
        /// </summary>
        /// <param name="exportConvention">The <see cref="IExportConvention"/> that the contract name should be retreived for.</param>
        /// <param name="member">The <see cref="MemberInfo"/> that is being exported.</param>
        /// <returns>A <see cref="string"/> containing the contract name for the export.</returns>
        private static string GetExportContractName(IExportConvention exportConvention, MemberInfo member)
        {
            if (exportConvention.ContractName != null)
            {
                return exportConvention.ContractName.Invoke(member);
            }

            if (exportConvention.ContractType != null)
            {
                return AttributedModelServices.GetContractName(exportConvention.ContractType.Invoke(member));
            }

            return member.MemberType == MemberTypes.Method ?
                AttributedModelServices.GetTypeIdentity((MethodInfo)member) :
                AttributedModelServices.GetContractName(member.GetContractMember());
        }

        /// <summary>
        /// Gets the metadata for the provided <see cref="IExportConvention"/>.
        /// </summary>
        /// <param name="exportConvention">The <see cref="IExportConvention"/> that the metadata should be retrieved for.</param>
        /// <param name="member">The <see cref="MemberInfo"/> that is being exported.</param>
        /// <returns>An <see cref="IDictionary{TKey,TValue}"/> containing the metadata for the export.</returns>
        private static IDictionary<string, object> GetExportDefinitionMetadata(IExportConvention exportConvention, MemberInfo member)
        {
            var exportDefinitionMetadata =
                exportConvention.Metadata.ToMetadataDictionary();

            exportDefinitionMetadata.Add(
                CompositionConstants.ExportTypeIdentityMetadataName,
                GetExportTypeIdentity(exportConvention, member));

            return exportDefinitionMetadata;
        }

        /// <summary>
        /// Gets type identity for the provided <see cref="IExportConvention"/>.
        /// </summary>
        /// <param name="exportConvention">The <see cref="IExportConvention"/> that the type identity should be retreived for.</param>
        /// <param name="member">The <see cref="MemberInfo"/> that is being exported.</param>
        /// <returns>A <see cref="string"/> containing the type identity for the export.</returns>
        private static string GetExportTypeIdentity(IExportConvention exportConvention, MemberInfo member)
        {
            if (exportConvention.ContractType != null)
            {
                return AttributedModelServices.GetTypeIdentity(exportConvention.ContractType.Invoke(member));
            }

            return member.MemberType.Equals(MemberTypes.Method) ? 
                AttributedModelServices.GetTypeIdentity((MethodInfo)member) : 
                AttributedModelServices.GetTypeIdentity(member.GetContractMember());
        }

        /// <summary>
        /// Gets contract name for the provided <see cref="IImportConvention"/>.
        /// </summary>
        /// <param name="importConvention">The <see cref="IImportConvention"/> that the contract name should be retreived for.</param>
        /// <param name="member">The <see cref="MemberInfo"/> that is being imported.</param>
        /// <returns>A <see cref="string"/> containing the contract name for the import.</returns>
        private static string GetImportContractName(IImportConvention importConvention, MemberInfo member)
        {
            if (importConvention.ContractName != null)
            {
                return importConvention.ContractName.Invoke(member);
            }

            return importConvention.ContractType != null ?
                AttributedModelServices.GetTypeIdentity(importConvention.ContractType.Invoke(member)) :
                AttributedModelServices.GetTypeIdentity(member.GetContractMember());
        }

        /// <summary>
        /// Gets type identity for the provided <see cref="IImportConvention"/>.
        /// </summary>
        /// <param name="importConvention">The <see cref="IImportConvention"/> that the type identity should be retreived for.</param>
        /// <param name="member">The <see cref="MemberInfo"/> that is being imported.</param>
        /// <returns>A <see cref="string"/> containing the type identity for the imported.</returns>
        private static string GetImportTypeIdentity(IImportConvention importConvention, MemberInfo member)
        {
            if (importConvention.ContractType == null)
            {
                return AttributedModelServices.GetTypeIdentity(member.GetContractMember());
            }

            if (importConvention.ContractType.Invoke(member).Equals(typeof(object)))
            {
                return null;   
            }

            var memberType =
                member.GetContractMember();

            return memberType.IsSubclassOf(typeof(Delegate)) ? 
                AttributedModelServices.GetTypeIdentity(memberType.GetMethod("Invoke")) :
                AttributedModelServices.GetTypeIdentity(importConvention.ContractType.Invoke(member));
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