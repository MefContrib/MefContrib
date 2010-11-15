namespace MefContrib.Hosting.Generics
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a generics contract type mapping registry.
    /// </summary>
    public interface IGenericContractRegistry
    {
        /// <summary>
        /// Retrieves open generics type mappings.
        /// </summary>
        /// <returns>An instance of <see cref="IEnumerable{T}"/> containing <see cref="GenericContractTypeMapping"/> instances.</returns>
        IEnumerable<GenericContractTypeMapping> GetMappings();
    }
}