namespace MefContrib.Hosting.Conventions
{
    using System.Collections.Generic;
    using MefContrib.Hosting.Conventions.Configuration;

    /// <summary>
    /// Defines the functionality available by a partregistry locator
    /// </summary>
    public interface IPartRegistryLocator
    {
        /// <summary>
        /// Locates <see cref="IPartRegistry{TContractService}"/> instances.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IPartRegistry{TContractService}"/> instances.</returns>
        IEnumerable<IPartRegistry<IContractService>> GetRegistries();
    }
}