namespace MefContrib.Hosting.Conventions.Configuration
{
    public abstract class ConventionBuilder<TConvention>
        : ExpressionBuilder<TConvention> where TConvention : new()
    {
        /// <summary>
        /// Gets the convention instance built by the convention builder.
        /// </summary>
        /// <returns>An instance of the convention type that the convention builder can build.</returns>
        public TConvention GetConvention()
        {
            return (TConvention)this.Build();
        }
    }
}