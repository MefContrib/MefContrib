namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;

    /// <summary>
    /// Defines functionality that is shared between all convention builders.
    /// </summary>
    /// <typeparam name="TConvention">The <see cref="Type"/> of the convention that should be built by the convention builder.</typeparam>
    public interface IConventionBuilder<TConvention> : IHideObjectMembers
    {
        /// <summary>
        /// Gets the convention instance built by the convention builder.
        /// </summary>
        /// <returns>An instance of the convention type that the convention builder can build.</returns>
        TConvention GetBuiltInstance();
    }
}