namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;

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
        /// Gets or sets the type scanner used to create parts out of the conventions in the registry.
        /// </summary>
        /// <value>An <see cref="ITypeScanner"/> instance.</value>
        ITypeScanner TypeScanner { get; set; }

        /// <summary>
        /// Creates a convention builde for <see cref="PartConvention"/> conventions.
        /// </summary>
        /// <returns>A <see cref="PartConventionBuilder{TPartConvention}"/> instance for the <see cref="PartConvention"/> type.</returns>
        PartConventionBuilder<PartConvention> Part();

        /// <summary>
        /// Create a convention builder for the <typeparamref name="TConvention"/> convention type.
        /// </summary>
        /// <typeparam name="TConvention">The type of a class which implements the <see cref="IPartConvention"/> interface.</typeparam>
        /// <returns>A <see cref="PartConventionBuilder{TPartConvention}"/> instance for the part convention type specified by the <typeparamref name="TConvention"/> type parameter.</returns>
        PartConventionBuilder<TConvention> Part<TConvention>() where TConvention : IPartConvention, new();
    }
}