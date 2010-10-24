namespace MefContrib.Hosting.Interception.Configuration
{
    using System;
    using System.ComponentModel.Composition.Primitives;

    public interface IPartInterceptionCriteria
    {
        IExportedValueInterceptor Interceptor { get; }

        Func<ComposablePartDefinition, bool> Predicate { get; }
    }
}