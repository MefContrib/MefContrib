namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A convention registry for types implementing the <see cref="IPartConvention"/> interface.
    /// </summary>
    public class PartRegistry :
        ExpressionBuilderFactory<IPartConvention>, IPartRegistry<DefaultConventionContractService>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartRegistry"/> class.
        /// </summary>
        public PartRegistry()
        {
            this.ContractService = new DefaultConventionContractService();
        }

        /// <summary>
        /// Gets or sets the contract service used by the registry.
        /// </summary>
        /// <value>An <see cref="DefaultConventionContractService"/> instance.</value>
        public DefaultConventionContractService ContractService { get; private set; }

        /// <summary>
        /// Gets or sets the type scanner used to create parts out of the conventions in the registry.
        /// </summary>
        /// <value>An <see cref="ITypeScanner"/> instance.</value>
        public ITypeScanner TypeScanner { get; set; }

        /// <summary>
        /// Scans the specified closure.
        /// </summary>
        /// <param name="closure">The closure.</param>
        public void Scan(Action<ITypeScannerConfigurator> closure)
        {
            if (closure == null)
            {
                throw new ArgumentNullException("closure", "The closure cannot be null.");
            }

            var configurator =
                new TypeScannerConfigurator();

            closure.Invoke(configurator);

            this.TypeScanner =
                configurator.GetTypeScanner();
        }

        /// <summary>
        /// Gets the conventions registered in the registry.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IPartConvention"/> instances.</returns>
        public IEnumerable<IPartConvention> GetConventions()
        {
            return this.BuildConventions();
        }

        /// <summary>
        /// Creates a convention builder for <see cref="PartConvention"/> conventions.
        /// </summary>
        /// <returns>A <see cref="PartConventionBuilder{TPartConvention}"/> instance for the <see cref="PartConvention"/> type.</returns>
        public PartConventionBuilder<PartConvention> Part()
        {
            return this.CreateExpressionBuilder<PartConventionBuilder<PartConvention>>();
        }

        /// <summary>
        /// Create a convention builder for the <typeparamref name="TConvention"/> convention type.
        /// </summary>
        /// <typeparam name="TConvention">The type of a class which implements the <see cref="IPartConvention"/> interface.</typeparam>
        /// <returns>A <see cref="PartConventionBuilder{TPartConvention}"/> instance for the part convention type specified by the <typeparamref name="TConvention"/> type parameter.</returns>
        public PartConventionBuilder<TConvention> Part<TConvention>() where TConvention : IPartConvention, new()
        {
            return this.CreateExpressionBuilder<PartConventionBuilder<TConvention>>();
        }
    }
}