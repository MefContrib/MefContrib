using System;
using System.ComponentModel.Composition.Primitives;

namespace MefContrib.Hosting.Interception.Configuration
{
    public interface IPartInterceptor
    {
        IExportedValueInterceptor Interceptor { get; }

        Func<ComposablePartDefinition, bool> Predicate { get; }
    }
}