namespace MefContrib.Hosting.Interception.Configuration
{
    using System;
    using System.ComponentModel.Composition.Primitives;

    public class PredicatePartInterceptor : IPartInterceptor
    {
        public PredicatePartInterceptor(IExportedValueInterceptor interceptor, Func<ComposablePartDefinition, bool> predicate)
        {
            Interceptor = interceptor;
            Predicate = predicate;
        }

        public IExportedValueInterceptor Interceptor { get; private set; }

        public Func<ComposablePartDefinition, bool> Predicate { get; private set; }
    }
}