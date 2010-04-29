namespace MefContrib.Hosting.Conventions.Configuration
{
    /// <summary>
    /// A convention registry for types implementing the <see cref="IPartConvention"/> interface.
    /// </summary>
    public class ExportRegistry :
        ConventionRegistry<IExportConvention>, IExportRegistry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportRegistry"/> class.
        /// </summary>
        public ExportRegistry()
        {
        }

        /// <summary>
        /// Creates a convention builde for <see cref="ExportConvention"/> conventions.
        /// </summary>
        /// <returns>A <see cref="IExportConventionBuilder{TPartConvention}"/> instance for the <see cref="ExportConvention"/> type.</returns>
        public IExportConventionBuilder<ExportConvention> Export()
        {
            return this.CreateExpressionBuilder<ExportConvention, ExportConventionBuilder<ExportConvention>>();
        }

        /// <summary>
        /// Create a convention builder for the <typeparamref name="TConvention"/> convention type.
        /// </summary>
        /// <typeparam name="TConvention">The type of a class which implements the <see cref="IExportConvention"/> interface.</typeparam>
        /// <returns>A <see cref="IExportConventionBuilder{TPartConvention}"/> instance for the export convention type specified by the <typeparamref name="TConvention"/> type parameter.</returns>
        public IExportConventionBuilder<TConvention> Export<TConvention>() where TConvention : IExportConvention, new()
        {
            return this.CreateExpressionBuilder<TConvention, ExportConventionBuilder<TConvention>>();
        }
    }
}
