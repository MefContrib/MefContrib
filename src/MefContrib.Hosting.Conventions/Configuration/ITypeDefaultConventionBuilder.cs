namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;

    /// <summary>
    /// Defines the functionality for a default value convention builder.
    /// </summary>
    public interface ITypeDefaultConventionBuilder : IHideObjectMembers
    {
        /// <summary>
        /// Defines the contract name that will be used as the default contract name for the configured type.
        /// </summary>
        /// <param name="contractName">A <see cref="string"/> containing the name of the contract which should be used as the default contract name for the configured type.</param>
        /// <returns>Returns a reference to the same <see cref="ImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        ITypeDefaultConventionBuilder ContractName(string contractName);

        /// <summary>
        /// Defines the contract type that will be added to the imports created by the convention.
        /// </summary>
        /// <typeparam name="TContractType">A <see cref="Type"/> that should be used as the contract type of the created imports.</typeparam>
        /// <returns>Returns a reference to the same <see cref="ImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        ITypeDefaultConventionBuilder ContractType<TContractType>();
    }
}