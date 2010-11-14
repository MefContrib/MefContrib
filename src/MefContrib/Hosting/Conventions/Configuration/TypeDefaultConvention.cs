namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;

    /// <summary>
    /// Contains the default convention values for a <see cref="Type"/>.
    /// </summary>
    public class TypeDefaultConvention : ITypeDefaultConvention
    {
        /// <summary>
        /// Gets or sets the name of the contract.
        /// </summary>
        /// <value>A <see cref="string"/> containing the name of the contract.</value>
        public string ContractName { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Type"/> of the contract.
        /// </summary>
        /// <value>The <see cref="Type"/> of the contract.</value>
        public Type ContractType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Type"/> that the convention applies to.
        /// </summary>
        /// <value>The <see cref="Type"/> that the convention applies to.</value>
        public Type TargetType { get; set; }
    }
}