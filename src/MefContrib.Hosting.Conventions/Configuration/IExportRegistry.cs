namespace MefContrib.Hosting.Conventions.Configuration
{
    /// <summary>
    /// Defines the functionality of a convention registry for conventions implementing the <see cref="IExportConvention"/> interface.
    /// </summary>
    public interface IExportRegistry : IHideObjectMembers
    {
        /// <summary>
        /// Creates a convention builde for <see cref="ExportConvention"/> conventions.
        /// </summary>
        /// <returns>A <see cref="IExportConventionBuilder{TPartConvention}"/> instance for the <see cref="ExportConvention"/> type.</returns>
        IExportConventionBuilder<ExportConvention> Export();

        /// <summary>
        /// Create a convention builder for the <typeparamref name="TConvention"/> convention type.
        /// </summary>
        /// <typeparam name="TConvention">The type of a class which implements the <see cref="IExportConvention"/> interface.</typeparam>
        /// <returns>A <see cref="IExportConventionBuilder{TPartConvention}"/> instance for the export convention type specified by the <typeparamref name="TConvention"/> type parameter.</returns>
        IExportConventionBuilder<TConvention> Export<TConvention>() where TConvention : IExportConvention, new();
    }
}