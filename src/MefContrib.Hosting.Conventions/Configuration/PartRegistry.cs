namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A convention registry for types implementing the <see cref="IPartConvention"/> interface.
    /// </summary>
    public class PartRegistry :
        ConventionRegistry<IPartConvention>, IPartRegistry, ITypeDefaultConventionProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartRegistry"/> class.
        /// </summary>
        public PartRegistry()
        {
        }

        public void Defaults(Action<ITypeDefaultConventionConfigurator> closure)
        {
            var configurator =
                new TypeDefaultConventionConfigurator();

            closure.Invoke(configurator);

            this.DefaultConventions = configurator.GetTypeDefaultConventions();
        }

        private IEnumerable<ITypeDefaultConvention> DefaultConventions { get; set; }

        IEnumerable<ITypeDefaultConvention> ITypeDefaultConventionProvider.GetTypeDefaultConventions()
        {
            return this.DefaultConventions;
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