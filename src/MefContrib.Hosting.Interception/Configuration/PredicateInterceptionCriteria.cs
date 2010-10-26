namespace MefContrib.Hosting.Interception.Configuration
{
    using System;
    using System.ComponentModel.Composition.Primitives;

    /// <summary>
    /// Defines predicate-based interception criteria.
    /// </summary>
    public class PredicateInterceptionCriteria : IPartInterceptionCriteria
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PredicateInterceptionCriteria"/> class.
        /// </summary>
        /// <param name="interceptor">The <see cref="IExportedValueInterceptor"/> instance.</param>
        /// <param name="predicate">The predicate.</param>
        public PredicateInterceptionCriteria(IExportedValueInterceptor interceptor, Func<ComposablePartDefinition, bool> predicate)
        {
            if (interceptor == null) throw new ArgumentNullException("interceptor");
            if (predicate == null) throw new ArgumentNullException("predicate");

            Interceptor = interceptor;
            Predicate = predicate;
        }

        /// <summary>
        /// Gets the <see cref="IExportedValueInterceptor"/> instance.
        /// </summary>
        public IExportedValueInterceptor Interceptor { get; private set; }

        /// <summary>
        /// Gets the predicate.
        /// </summary>
        public Func<ComposablePartDefinition, bool> Predicate { get; private set; }
    }
}