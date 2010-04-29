using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using MefContrib.Integration.Unity.Exporters;
using MefContrib.Integration.Unity.Properties;
using Microsoft.Practices.ObjectBuilder2;

namespace MefContrib.Integration.Unity.Strategies
{
    /// <summary>
    /// Represents a MEF composition strategy which tries to resolve desired
    /// component via MEF. If succeeded, build process is completed.
    /// </summary>
    public class CompositionStrategy : BuilderStrategy
    {
        public override void PreBuildUp(IBuilderContext context)
        {
            // If type is registered in the Unity container, don't even bother messing with MEF
            var policy = context.Policies.Get<IBuildKeyMappingPolicy>(context.OriginalBuildKey);
            if (policy != null)
                return;

            var container = context.Policies.Get<ICompositionContainerPolicy>(null).Container;
            var buildKey = context.BuildKey;

            try
            {
                var exports = container.GetExports(buildKey.Type, null, buildKey.Name);

                if (exports.Count() == 0)
                    return;

                if (exports.Count() > 1)
                    throw new CompositionException(Resources.TooManyInstances);

                var lazyExport = exports.First();
                var lazyExportMetadata = lazyExport.Metadata as IDictionary<string, object>;
                if (lazyExportMetadata != null &&
                    lazyExportMetadata.ContainsKey(ExporterConstants.IsExternallyProvidedMetadataName) &&
                    true.Equals(lazyExportMetadata[ExporterConstants.IsExternallyProvidedMetadataName]))
                {
                    return;
                }

                context.Existing = lazyExport.Value;
                context.BuildComplete = true;
            }
            catch (Exception)
            {
                context.BuildComplete = false;
                throw;
            }
        }
    }
}