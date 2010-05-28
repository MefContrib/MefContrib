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

        IEnumerable<ITypeDefaultConvention> ITypeDefaultConventionProvider.TypeDefaultConventions { get; set; }

        public void Defaults(Action<ITypeDefaultConventionConfigurator> configurer)
        {
            
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

    public interface ITypeDefaultConventionConfigurator
    {
        IEnumerable<ITypeDefaultConvention> GetTypeDefaultConventions();
    }
}