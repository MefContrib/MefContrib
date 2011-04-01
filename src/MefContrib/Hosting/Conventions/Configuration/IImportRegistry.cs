namespace MefContrib.Hosting.Conventions.Configuration
{
    /// <summary>
    /// Defines the functionality of a convention registry for conventions implementing the <see cref="IImportConvention"/> interface.
    /// </summary>
    public interface IImportRegistry
        : IConventionRegistry<IImportConvention>, IHideObjectMembers
    {
        /// <summary>
        /// Creates a convention builde for <see cref="ImportConvention"/> conventions.
        /// </summary>
        /// <returns>A <see cref="ImportConventionBuilder{TPartConvention}"/> instance for the <see cref="ImportConvention"/> type.</returns>
        ImportConventionBuilder<ImportConvention> Import();

        /// <summary>
        /// Create a convention builder for the <typeparamref name="TConvention"/> convention type.
        /// </summary>
        /// <typeparam name="TConvention">The type of a class which implements the <see cref="IImportConvention"/> interface.</typeparam>
        /// <returns>A <see cref="ImportConventionBuilder{TPartConvention}"/> instance for the import convention type specified by the <typeparamref name="TConvention"/> type parameter.</returns>
        ImportConventionBuilder<TConvention> ImportWithConvention<TConvention>() where TConvention : IImportConvention, new();
    }
}