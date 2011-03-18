namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Reflection;
    using MefContrib.Hosting.Conventions.Configuration.Section;

    /// <summary>
    /// Represents a parts registry which uses <see cref="ConventionConfigurationSection"/> to provide parts.
    /// </summary>
    public class ConfigurationPartRegistry : IPartRegistry<DefaultConventionContractService>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationPartRegistry"/> class.
        /// </summary>
        /// <param name="configurationSectionName">Name of the section defined in the App.config file.</param>
        public ConfigurationPartRegistry(string configurationSectionName)
            : this((ConventionConfigurationSection)ConfigurationManager.GetSection(configurationSectionName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationPartRegistry"/> class.
        /// </summary>
        /// <param name="section">An <see cref="ConventionConfigurationSection"/> instance.</param>
        public ConfigurationPartRegistry(ConventionConfigurationSection section)
        {
            if (section == null)
            {
                throw new ArgumentNullException("section");
            }

            ContractService = new DefaultConventionContractService();
            ConfigurationSection = section;

            var typeScanner = new TypeScanner();
            foreach (PartElement part in ConfigurationSection.Parts)
            {
                var type = CreateType(part.Type);
                typeScanner.Types.Add(type);
            }

            TypeScanner = typeScanner;
        }

        /// <summary>
        /// Gets the conventions registered in the registry.
        /// </summary>
        public IEnumerable<IPartConvention> GetConventions()
        {
            var conventions = new List<IPartConvention>();

            foreach (PartElement part in ConfigurationSection.Parts)
            {
                var innerPart = part;
                conventions.Add(
                    new PartConvention
                        {
                            Condition = t => t == CreateType(innerPart.Type),
                            Exports = CreateExportConventions(innerPart.Exports),
                            Imports = CreateImportConventions(innerPart.Imports),
                            Metadata = CreateMetadataItems(innerPart.Metadata),
                            CreationPolicy = innerPart.CreationPolicy
                        });
            }

            return conventions;
        }

        /// <summary>
        /// Gets <see cref="ConventionConfigurationSection"/> instance.
        /// </summary>
        public ConventionConfigurationSection ConfigurationSection { get; private set; }

        /// <summary>
        /// Gets or sets the contract service used by the registry.
        /// </summary>
        /// <value>An <see cref="IContractService"/> instance.</value>
        public DefaultConventionContractService ContractService { get; private set; }

        /// <summary>
        /// Gets the type scanner used to create parts out of the conventions in the registry.
        /// </summary>
        /// <value>An <see cref="ITypeScanner"/> instance.</value>
        public ITypeScanner TypeScanner { get; private set; }

        private Type CreateType(string typeName)
        {
            var type = Type.GetType(typeName, true);
            return type;
        }

        private IList<IExportConvention> CreateExportConventions(ExportElementCollection elementCollection)
        {
            var exports = new List<IExportConvention>();

            foreach (ExportElement export in elementCollection)
            {
                if (export != null)
                {
                    var innerExport = export;
                    exports.Add(
                        new ExportConvention
                        {
                            ContractName = m => string.IsNullOrEmpty(innerExport.ContractName) ? null : innerExport.ContractName,
                            ContractType = m => CreateType(innerExport.ContractType),
                            Members = t => string.IsNullOrEmpty(innerExport.Member) ? new MemberInfo[] { t } : t.GetMember(innerExport.Member),
                            Metadata = CreateMetadataItems(export.Metadata)
                        }
                    );
                }

            }

            return exports;
        }

        private IList<IImportConvention> CreateImportConventions(ImportElementCollection elementCollection)
        {
            var imports = new List<IImportConvention>();

            foreach (ImportElement import in elementCollection)
            {
                var innerImport = import;
                imports.Add(
                    new ImportConvention
                    {
                        ContractName = t => string.IsNullOrEmpty(innerImport.ContractName) ? null : innerImport.ContractName,
                        ContractType = t => CreateType(innerImport.ContractType),
                        Members = t => string.IsNullOrEmpty(innerImport.Member) ? new MemberInfo[] { t } : t.GetMember(innerImport.Member),
                        AllowDefaultValue = import.AllowDefault,
                        CreationPolicy = import.CreationPolicy,
                        Recomposable = import.IsRecomposable,
                        RequiredMetadata = CreateRequiredMetadataItems(innerImport.RequiredMetadata)
                    }
                );
            }

            return imports;
        }

        private IList<MetadataItem> CreateMetadataItems(MetadataElementCollection elementCollection)
        {
            var items = new List<MetadataItem>();
            foreach (MetadataElement configurationMetadata in elementCollection)
            {
                var item = new MetadataItem(configurationMetadata.Name, configurationMetadata.Value);
                items.Add(item);
            }

            return items;
        }

        private IList<RequiredMetadataItem> CreateRequiredMetadataItems(MetadataElementCollection elementCollection)
        {
            var items = new List<RequiredMetadataItem>();
            foreach (MetadataElement configurationMetadata in elementCollection)
            {
                var item = new RequiredMetadataItem(configurationMetadata.Name, CreateType(configurationMetadata.Type));
                items.Add(item);
            }

            return items;
        }
    }
}