namespace MefContrib.Hosting.Conventions.Configuration
{
    /// <summary>
    /// Defines the functionality of a convention registry for conventions implementing the <see cref="IPartConvention"/> interface.
    /// </summary>
    public interface IPartRegistry<out TContractService>
        : IConventionRegistry<IPartConvention> where TContractService : IContractService
    {
        /// <summary>
        /// Gets or sets the contract service used by the registry.
        /// </summary>
        /// <value>An <see cref="IContractService"/> instance.</value>
        TContractService ContractService { get; }

        /// <summary>
        /// Gets the type scanner used to create parts out of the conventions in the registry.
        /// </summary>
        /// <value>An <see cref="ITypeScanner"/> instance.</value>
        ITypeScanner TypeScanner { get; }
    }
}