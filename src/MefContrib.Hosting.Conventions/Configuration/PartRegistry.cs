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
        ITypeDefaultConventionBuilder ContractName(string name);

        ITypeDefaultConventionBuilder ContractType<T>() where T : new();
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
        : TypedExpressionBuilder<TypeDefaultConvention>, ITypeDefaultConventionBuilder
    {
        public TypeDefaultConventionBuilder(Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType", "The target type cannot be null.");
            }

            this.ProvideValueFor(x => x.TargetType, targetType);
        }

        public ITypeDefaultConventionBuilder ContractName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name", "The contract name cannot be null.");
            }

            if(name.Length == 0)
            {
                throw new ArgumentOutOfRangeException("name", "The lenght of the contract name cannot be zero.");
            }

            this.ProvideValueFor(x => x.ContractName, name);

            return this;
        }

        public ITypeDefaultConventionBuilder ContractType<T>() where T : new()
        {
            this.ProvideValueFor(x => x.ContractType, typeof(T));

            return this;
        }
    }

    public class TypeDefaultConvention : ITypeDefaultConvention
    {
        public string ContractName { get; set; }

        public Type ContractType { get; set; }

        public Type TargetType { get; set; }
    }
}