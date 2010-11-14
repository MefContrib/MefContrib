namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;

    /// <summary>
    /// Defines the functionality for configuring type default conventions.
    /// </summary>
    public interface ITypeDefaultConventionConfigurator : IHideObjectMembers
    {
        /// <summary>
        /// Creates a <see cref="ITypeDefaultConventionBuilder"/> instance for the <see cref="Type"/> specified by
        /// the <typeparamref name="T"/> type parameter.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> that the <see cref="ITypeDefaultConventionBuilder"/> should be created for.</typeparam>
        /// <returns>An <see cref="ITypeDefaultConventionBuilder"/> instance.</returns>
        ITypeDefaultConventionBuilder ForType<T>();
    }
}