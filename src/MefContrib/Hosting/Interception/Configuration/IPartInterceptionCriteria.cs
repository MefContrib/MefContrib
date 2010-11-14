namespace MefContrib.Hosting.Interception.Configuration
{
    using System;
    using System.ComponentModel.Composition.Primitives;

    /// <summary>
    /// Defines the <see cref="ComposablePartDefinition"/> interception criteria.
    /// All <see cref="ComposablePartDefinition"/> instances which satisfy given
    /// <see cref="Predicate"/> whill have their value intercepted <see cref="Interceptor"/> instance.
    /// </summary>
    public interface IPartInterceptionCriteria
    {
        /// <summary>
        /// Gets the <see cref="IExportedValueInterceptor"/> instance.
        /// </summary>
        IExportedValueInterceptor Interceptor { get; }

        /// <summary>
        /// Gets the predicate.
        /// </summary>
        Func<ComposablePartDefinition, bool> Predicate { get; }
    }
}