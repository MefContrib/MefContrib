namespace MefContrib.Hosting.Generics
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// Defines a base class for <see cref="IGenericContractRegistry"/> implementations.
    /// </summary>
    public abstract class GenericContractRegistryBase : IGenericContractRegistry
    {
        private readonly List<GenericContractTypeMapping> mappings;

        private int isInitialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericContractRegistryBase"/> class.
        /// </summary>
        protected GenericContractRegistryBase()
        {
            this.mappings = new List<GenericContractTypeMapping>();
        }

        /// <summary>
        /// Maps open generics type to its open generics implementation.
        /// </summary>
        /// <param name="genericContractTypeDefinition">Open generics contract type.</param>
        /// <param name="genericImplementationTypeDefinition">Open generics implementation type.</param>
        protected void Register(Type genericContractTypeDefinition, Type genericImplementationTypeDefinition)
        {
            if (genericContractTypeDefinition == null)
                throw new ArgumentNullException("genericContractTypeDefinition");

            if (genericImplementationTypeDefinition == null)
                throw new ArgumentNullException("genericImplementationTypeDefinition");

            this.mappings.Add(new GenericContractTypeMapping(
                genericContractTypeDefinition, genericImplementationTypeDefinition));
        }

        /// <summary>
        /// Method which is responsible for initializing this registry.
        /// </summary>
        /// <remarks>
        /// Use this method to register type mappings using the <see cref="Register"/> method.
        /// </remarks>
        protected abstract void Initialize();

        /// <summary>
        /// Retrieves open generics type mappings.
        /// </summary>
        /// <returns>An instance of <see cref="IEnumerable{T}"/> containing <see cref="GenericContractTypeMapping"/> instances.</returns>
        public IEnumerable<GenericContractTypeMapping> GetMappings()
        {
            if (Interlocked.CompareExchange(ref this.isInitialized, 1, 0) == 0)
            {
                Initialize();
            }

            return this.mappings.AsReadOnly();
        }
    }
}