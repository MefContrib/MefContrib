namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A convention registry for types implementing the <see cref="IPartConvention"/> interface.
    /// </summary>
    public class PartRegistry :
        ExpressionBuilderFactory<IPartConvention>, IPartRegistry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartRegistry"/> class.
        /// </summary>
        public PartRegistry()
        {
        }

        /// <summary>
        /// Gets or sets the contract service used by the registry.
        /// </summary>
        /// <value>An <see cref="IContractService"/> instance.</value>
        public IContractService ContractService { get; set; }

        /// <summary>
        /// Gets or sets the type loader used to create parts out of the conventions in the registry.
        /// </summary>
        /// <value>An <see cref="ITypeLoader"/> instance.</value>
        public ITypeLoader TypeLoader { get; set; }

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

    public interface ITypeDefaultConventionConfigurator : IHideObjectMembers
    {
        ITypeDefaultConventionBuilder ForType<T>();

        
    }

    public class TypeDefaultConventionConfigurator : ITypeDefaultConventionConfigurator
    {
        public TypeDefaultConventionConfigurator()
        {
        }

        public ITypeDefaultConventionBuilder ForType<T>()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITypeDefaultConvention> GetTypeDefaultConventions()
        {
            throw new NotImplementedException();
        }
    }

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
        ITypeDefaultConventionBuilder ContractType<TContractType>() where TContractType : new();
    }

    public class TypedExpressionBuilder<T>
        : ExpressionBuilder<T> where T : new()
    {
        public new T Build()
        {
            return (T)base.Build();
        }
    }

    public class TypeDefaultConventionBuilder
        : ConventionBuilder<TypeDefaultConvention>, ITypeDefaultConventionBuilder
    {
        public TypeDefaultConventionBuilder(Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType", "The target type cannot be null.");
            }

            this.ProvideValueFor(x => x.TargetType, targetType);
        }

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

        public ITypeDefaultConventionBuilder ContractType<TContractType>() where TContractType : new()
        {
            this.ProvideValueFor(x => x.ContractType, typeof(TContractType));

            return this;
        }
    }

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