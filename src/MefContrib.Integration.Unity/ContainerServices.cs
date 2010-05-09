using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using MefContrib.Integration.Unity.Exporters;
using MefContrib.Integration.Unity.Properties;

namespace MefContrib.Integration.Unity
{
    public static class ContainerServices
    {
        public static Lazy<object> Resolve(CompositionContainer compositionContainer, Type type, string name)
        {
            var exports = compositionContainer.GetExports(type, null, name);

            if (exports.Count() == 0)
                return null;

            if (exports.Count() > 1)
                throw new CompositionException(Resources.TooManyInstances);

            var lazyExport = exports.First();
            var lazyExportMetadata = lazyExport.Metadata as IDictionary<string, object>;
            if (lazyExportMetadata != null &&
                lazyExportMetadata.ContainsKey(ExporterConstants.IsExternallyProvidedMetadataName) &&
                true.Equals(lazyExportMetadata[ExporterConstants.IsExternallyProvidedMetadataName]))
            {
                return null;
            }

            return lazyExport;
        }

        public static IEnumerable<Lazy<object>> ResolveAll(CompositionContainer compositionContainer, Type type, string name)
        {
            var exports = compositionContainer.GetExports(type, null, name);

            if (exports.Count() == 0)
                return Enumerable.Empty<Lazy<object>>();
            
            var list = new List<Lazy<object>>();

            foreach (var export in exports)
            {
                var lazyExportMetadata = export.Metadata as IDictionary<string, object>;
                if (lazyExportMetadata != null &&
                    lazyExportMetadata.ContainsKey(ExporterConstants.IsExternallyProvidedMetadataName) &&
                    true.Equals(lazyExportMetadata[ExporterConstants.IsExternallyProvidedMetadataName]))
                {
                    continue;
                }

                list.Add(export);
            }

            return list;
        }
    }
}