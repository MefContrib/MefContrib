namespace MefContrib.Hosting.Conventions.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the functionality provided by the convention registry.
    /// </summary>
    /// <typeparam name="TConventionInterface">The type of the convention that the registry can handle.</typeparam>
    public interface IConventionRegistry<out TConventionInterface>
    {
        /// <summary>
        /// Gets the conventions registered in the registry.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing convention of the type specified by the <typeparamref name="TConventionInterface"/> type parameter.</returns>
        IEnumerable<TConventionInterface> GetConventions();
    }
}