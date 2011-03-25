namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
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
                var type = GetType(part.Type);
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
                            Condition = t => t == GetType(innerPart.Type),
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

        #region Private methods

        private Type GetType(string typeName)
        {
            var type = Type.GetType(typeName, true);
            return type;
        }

        private MemberInfo[] GetMembers(Type type, string memberName)
        {
            var members = string.IsNullOrEmpty(memberName)
                              ? new MemberInfo[] { type }
                              : type.GetMember(memberName);

            return members;
        }

        private string GetContractName(MemberInfo member, string givenContractName)
        {
            var contractName = string.IsNullOrEmpty(givenContractName) ? null : givenContractName;
            return contractName;
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
                                ContractName = member => GetContractName(member, innerExport.ContractName),
                                ContractType = member => GetType(innerExport.ContractType),
                                Members = type => GetMembers(type, innerExport.Member),
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
                            ContractName = member => GetContractName(member, innerImport.ContractName),
                            ContractType = member => GetType(innerImport.ContractType),
                            Members = type => GetMembers(type, innerImport.Member),
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
                Type targetType = GetType(configurationMetadata.Type);
                TypeConverter typeConverter = TypeDescriptor.GetConverter(targetType);
                if (typeConverter == null)
                {
                    throw new InvalidOperationException(string.Format("Type {0} does not provide a type converter.", targetType.Name));
                }

                var value = typeConverter.ConvertFromString(configurationMetadata.Value);
                var item = new MetadataItem(configurationMetadata.Name, value);
                items.Add(item);
            }

            return items;
        }

        private IList<RequiredMetadataItem> CreateRequiredMetadataItems(MetadataElementCollection elementCollection)
        {
            var items = new List<RequiredMetadataItem>();
            foreach (MetadataElement configurationMetadata in elementCollection)
            {
                var item = new RequiredMetadataItem(configurationMetadata.Name, GetType(configurationMetadata.Type));
                items.Add(item);
            }

            return items;
        }

        #endregion
    }
}