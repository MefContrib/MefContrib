namespace MefContrib.Hosting.Interception.Handlers
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    /// <summary>
    /// Defines a generics contract type mapping registry.
    /// </summary>
    [InheritedExport]
    public interface IGenericContractRegistry
    {
        /// <summary>
        /// Retrieves open generics type mappings.
        /// </summary>
        /// <returns>An instance of <see cref="IEnumerable{T}"/> containing <see cref="GenericContractTypeMapping"/> instances.</returns>
        IEnumerable<GenericContractTypeMapping> GetMappings();
    }
}