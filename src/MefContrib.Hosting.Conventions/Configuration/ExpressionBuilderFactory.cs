namespace MefContrib.Hosting.Conventions.Configuration
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides the ability to register conventions. The convention instances will not be created until they are
    /// retrieved from the registry.
    /// </summary>
    /// <typeparam name="TConvention">The type of the convention that the registry can handle.</typeparam>
    public abstract class ExpressionBuilderFactory<TConvention>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionBuilderFactory{TConvention}"/> class.
        /// </summary>
        protected ExpressionBuilderFactory()
        {
            this.ExpressionBuilders = new List<IExpressionBuilder>();
        }

        /// <summary>
        /// Gets or sets the convention builders.
        /// </summary>
        /// <value>A <see cref="IList{T}"/> instance, containing the convention builders.</value>
        private List<IExpressionBuilder> ExpressionBuilders { get; set; }

        /// <summary>
        /// Builds the conventions defines in the expression builders.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing convention of the type specified by the<typeparamref name="TConvention"/> type parameter.</returns>
        protected IEnumerable<TConvention> BuildConventions()
        {
            var conventions =
                from builder in this.ExpressionBuilders
                select builder.Build();

            return conventions.Cast<TConvention>();
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

            this.ExpressionBuilders.Add(builder);

            return builder;
        }
    }
}