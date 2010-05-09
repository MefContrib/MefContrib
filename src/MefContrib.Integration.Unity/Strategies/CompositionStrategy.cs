using System;
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
            var lazyExport = ContainerServices.Resolve(container, buildKey.Type, buildKey.Name);
            if (lazyExport != null)
            {
                context.Existing = lazyExport.Value;
                context.BuildComplete = true;
            }
        }
    }
}