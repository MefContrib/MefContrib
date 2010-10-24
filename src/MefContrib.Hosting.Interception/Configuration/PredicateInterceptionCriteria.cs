namespace MefContrib.Hosting.Interception.Configuration
{
    using System;
    using System.ComponentModel.Composition.Primitives;

    public class PredicateInterceptionCriteria : IPartInterceptionCriteria
    {
        public PredicateInterceptionCriteria(IExportedValueInterceptor interceptor, Func<ComposablePartDefinition, bool> predicate)
        {
            if (interceptor == null) throw new ArgumentNullException("interceptor");
            if (predicate == null) throw new ArgumentNullException("predicate");

            Interceptor = interceptor;
            Predicate = predicate;
        }

        public IExportedValueInterceptor Interceptor { get; private set; }

        public Func<ComposablePartDefinition, bool> Predicate { get; private set; }
    }
}