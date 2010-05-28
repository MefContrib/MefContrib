namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;
    using System.Collections.Generic;

    public interface ITypeDefaultConvention
    {
        /// <summary>
        /// Gets or sets the name of the contract.
        /// </summary>
        /// <value>A <see cref="string"/> containing the name of the contract.</value>
        string ContractName { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Type"/> of the contract.
        /// </summary>
        /// <value>The <see cref="Type"/> of the contract.</value>
        Type ContractType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Type"/> that the convention applies to.
        /// </summary>
        /// <value>The <see cref="Type"/> that the convention applies to.</value>
        Type TargetType { get; set; }
    }

    public interface ITypeDefaultConventionProvider
    {
        IEnumerable<ITypeDefaultConvention> TypeDefaultConventions { get; set; }
    }
}