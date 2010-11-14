namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;

    /// <summary>
    /// Provides the default implementation of a default convention value builder.
    /// </summary>
    public class TypeDefaultConventionBuilder
        : ConventionBuilder<TypeDefaultConvention>, ITypeDefaultConventionBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeDefaultConventionBuilder"/> class.
        /// </summary>
        /// <param name="targetType">The <see cref="Type"/> that the default convention values are being defined for.</param>
        /// <exception cref="ArgumentNullException">The provided value for the <paramref name="targetType"/> parameter was <see langword="null"/></exception>
        public TypeDefaultConventionBuilder(Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType", "The target type cannot be null.");
            }

            this.ProvideValueFor(x => x.TargetType, targetType);
        }

        /// <summary>
        /// Defines the contract name that will be used as the default contract name for the configured type.
        /// </summary>
        /// <param name="contractName">A <see cref="string"/> containing the name of the contract which should be used as the default contract name for the configured type.</param>
        /// <returns>Returns a reference to the same <see cref="ImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        /// <exception cref="ArgumentNullException">The provided value for the <paramref name="contractName"/> parameter was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The length of the <paramref name="contractName"/> parameter was zero.</exception>
        public ITypeDefaultConventionBuilder ContractName(string contractName)
        {
            if (contractName == null)
            {
                throw new ArgumentNullException("contractName", "The contract name cannot be null.");
            }

            if (contractName.Length == 0)
            {
                throw new ArgumentOutOfRangeException("contractName", "The lenght of the contract name cannot be zero.");
            }

            this.ProvideValueFor(x => x.ContractName, contractName);

            return this;
        }

        /// <summary>
        /// Defines the contract type that will be added to the imports created by the convention.
        /// </summary>
        /// <typeparam name="TContractType">A <see cref="Type"/> that should be used as the contract type of the created imports.</typeparam>
        /// <returns>Returns a reference to the same <see cref="ImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        public ITypeDefaultConventionBuilder ContractType<TContractType>()
        {
            this.ProvideValueFor(x => x.ContractType, typeof(TContractType));

            return this;
        }
    }
}