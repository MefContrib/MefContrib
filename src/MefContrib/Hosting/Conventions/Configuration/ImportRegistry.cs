namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A convention registry for types implementing the <see cref="IPartConvention"/> interface.
    /// </summary>
    public class ImportRegistry :
        ExpressionBuilderFactory<IImportConvention>, IImportRegistry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportRegistry"/> class.
        /// </summary>
        public ImportRegistry()
        {
        }

        /// <summary>
        /// Gets the conventions registered in the registry.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IImportConvention"/> instances.</returns>
        public IEnumerable<IImportConvention> GetConventions()
        {
            return this.BuildConventions();
        }

        /// <summary>
        /// Creates a convention builder for <see cref="ImportConvention"/> conventions.
        /// </summary>
        /// <returns>A <see cref="ImportConventionBuilder{TPartConvention}"/> instance for the <see cref="ImportConvention"/> type.</returns>
        public ImportConventionBuilder<ImportConvention> Import()
        {
            return this.CreateExpressionBuilder<ImportConventionBuilder<ImportConvention>>();
        }

        /// <summary>
        /// Create a convention builder for the <typeparamref name="TConvention"/> convention type.
        /// </summary>
        /// <typeparam name="TConvention">The type of a class which implements the <see cref="IImportConvention"/> interface.</typeparam>
        /// <returns>A <see cref="ImportConventionBuilder{TPartConvention}"/> instance for the import convention type specified by the <typeparamref name="TConvention"/> type parameter.</returns>
        public ImportConventionBuilder<TConvention> ImportWithConvention<TConvention>() where TConvention : IImportConvention, new()
        {
            return this.CreateExpressionBuilder<ImportConventionBuilder<TConvention>>();
        }
    }
}