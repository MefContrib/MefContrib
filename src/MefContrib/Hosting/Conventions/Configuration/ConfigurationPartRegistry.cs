namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Reflection;
    using MefContrib.Hosting.Conventions.Configuration.Section;

    public class ConfigurationPartRegistry : IPartRegistry<DefaultConventionContractService>
    {
        public ConfigurationPartRegistry(string configurationSectionName)
            : this((ConventionConfigurationSection)ConfigurationManager.GetSection(configurationSectionName))
        {
        }

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
                typeScanner.Types.Add(Type.GetType(part.Type));
            }

            TypeScanner = typeScanner;
        }

        public IEnumerable<IPartConvention> GetConventions()
        {
            var conventions = new List<IPartConvention>();

            foreach (PartElement part in ConfigurationSection.Parts)
            {
                var innerPart = part;
                conventions.Add(
                    new PartConvention
                        {
                            Condition = t => t == Type.GetType(innerPart.Type),
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

        public DefaultConventionContractService ContractService { get; private set; }

        public ITypeScanner TypeScanner { get; set; }

        public PartConventionBuilder<PartConvention> Part()
        {
            throw new NotSupportedException();
        }

        public PartConventionBuilder<TConvention> Part<TConvention>() where TConvention : IPartConvention, new()
        {
            throw new NotSupportedException();
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
                            ContractType = m => Type.GetType(innerExport.ContractType),
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
                        ContractType = t => Type.GetType(innerImport.ContractType),
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
                var item = new RequiredMetadataItem(configurationMetadata.Name, Type.GetType(configurationMetadata.Type));
                items.Add(item);
            }

            return items;
        }
    }
}