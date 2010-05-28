namespace MefContrib.Hosting.Conventions.Configuration
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides the ability to register conventions. The convention instances will not be created until they are
    /// retrieved from the registry.
    /// </summary>
    /// <typeparam name="TConventionInterface">The type of the convention that the registry can handle.</typeparam>
    public class ConventionRegistry<TConventionInterface> 
        : IConventionRegistry<TConventionInterface>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionRegistry{TConvention}"/> class.
        /// </summary>
        public ConventionRegistry()
        {
            this.ConventionBuilders = new List<IExpressionBuilder>();
        }

        /// <summary>
        /// Gets or sets the convention builders.
        /// </summary>
        /// <value>A <see cref="IList{T}"/> instance, containing the convention builders.</value>
        private List<IExpressionBuilder> ConventionBuilders { get; set; }

        /// <summary>
        /// Gets the conventions registered in the registry.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing convention of the type specified by the<typeparamref name="TConventionInterface"/> type parameter.</returns>
        public IEnumerable<TConventionInterface> GetConventions()
        {
            var conventions =
                from builder in this.ConventionBuilders
                select builder.Build();

            return conventions.Cast<TConventionInterface>();
        }

        /// <summary>
        /// Creates and tracks a new expression builder of the type specified by the <typeparamref name="TBuilder"/> type parameter.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the expression builder which should be created.</typeparam>
        /// <returns>A <see cref="ExpressionBuilder{T}"/> instance of the type specified by the <typeparamref name="TBuilder"/> type parameter.</returns>
        protected TBuilder CreateExpressionBuilder<TBuilder>()
            where TBuilder : IExpressionBuilder, new()
        {
            var builder =
                new TBuilder();

            this.ConventionBuilders.Add(builder);

            return builder;
        }
    }
}