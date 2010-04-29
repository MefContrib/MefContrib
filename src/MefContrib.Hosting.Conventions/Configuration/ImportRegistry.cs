namespace MefContrib.Hosting.Conventions.Configuration
{
    /// <summary>
    /// A convention registry for types implementing the <see cref="IPartConvention"/> interface.
    /// </summary>
    public class ImportRegistry :
        ConventionRegistry<IImportConvention>, IImportRegistry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportRegistry"/> class.
        /// </summary>
        public ImportRegistry()
        {
        }

        /// <summary>
        /// Creates a convention builde for <see cref="ImportConvention"/> conventions.
        /// </summary>
        /// <returns>A <see cref="IImportConventionBuilder{TPartConvention}"/> instance for the <see cref="ImportConvention"/> type.</returns>
        public IImportConventionBuilder<ImportConvention> Import()
        {
            return this.CreateExpressionBuilder<ImportConvention, ImportConventionBuilder<ImportConvention>>();
        }

        /// <summary>
        /// Create a convention builder for the <typeparamref name="TConvention"/> convention type.
        /// </summary>
        /// <typeparam name="TConvention">The type of a class which implements the <see cref="IImportConvention"/> interface.</typeparam>
        /// <returns>A <see cref="IImportConventionBuilder{TPartConvention}"/> instance for the import convention type specified by the <typeparamref name="TConvention"/> type parameter.</returns>
        public IImportConventionBuilder<TConvention> Import<TConvention>() where TConvention : IImportConvention, new()
        {
            return this.CreateExpressionBuilder<TConvention, ImportConventionBuilder<TConvention>>();
        }
    }
}