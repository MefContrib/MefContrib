namespace MefContrib.Hosting.Conventions.Configuration
{
    /// <summary>
    /// A convention registry for types implementing the <see cref="IPartConvention"/> interface.
    /// </summary>
    public class PartRegistry :
        ConventionRegistry<IPartConvention>, IPartRegistry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartRegistry"/> class.
        /// </summary>
        public PartRegistry()
        {
        }

        /// <summary>
        /// Creates a convention builde for <see cref="PartConvention"/> conventions.
        /// </summary>
        /// <returns>A <see cref="IPartConventionBuilder{TPartConvention}"/> instance for the <see cref="PartConvention"/> type.</returns>
        public IPartConventionBuilder<PartConvention> Part()
        {
            return this.CreateExpressionBuilder<PartConvention, PartConventionBuilder<PartConvention>>();
        }

        /// <summary>
        /// Create a convention builder for the <typeparamref name="TConvention"/> convention type.
        /// </summary>
        /// <typeparam name="TConvention">The type of a class which implements the <see cref="IPartConvention"/> interface.</typeparam>
        /// <returns>A <see cref="IPartConventionBuilder{TPartConvention}"/> instance for the part convention type specified by the <typeparamref name="TConvention"/> type parameter.</returns>
        public IPartConventionBuilder<TConvention> Part<TConvention>() where TConvention : IPartConvention, new()
        {
            return this.CreateExpressionBuilder<TConvention, PartConventionBuilder<TConvention>>();
        }
    }
}