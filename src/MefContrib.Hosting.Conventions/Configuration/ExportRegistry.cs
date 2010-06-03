namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A convention registry for types implementing the <see cref="IPartConvention"/> interface.
    /// </summary>
    public class ExportRegistry :
        ExpressionBuilderFactory<IExportConvention>, IExportRegistry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportRegistry"/> class.
        /// </summary>
        public ExportRegistry()
        {
        }

        /// <summary>
        /// Gets the conventions registered in the registry.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IExportConvention"/> instances.</returns>
        public IEnumerable<IExportConvention> GetConventions()
        {
            return this.BuildConventions();
        }

        /// <summary>
        /// Creates a convention builder for <see cref="ExportConvention"/> conventions.
        /// </summary>
        /// <returns>A <see cref="ExportConventionBuilder{TPartConvention}"/> instance for the <see cref="ExportConvention"/> type.</returns>
        public ExportConventionBuilder<ExportConvention> Export()
        {
            return this.CreateExpressionBuilder<ExportConventionBuilder<ExportConvention>>();
        }

        /// <summary>
        /// Create a convention builder for the <typeparamref name="TConvention"/> convention type.
        /// </summary>
        /// <typeparam name="TConvention">The type of a class which implements the <see cref="IExportConvention"/> interface.</typeparam>
        /// <returns>A <see cref="ExportConventionBuilder{TPartConvention}"/> instance for the export convention type specified by the <typeparamref name="TConvention"/> type parameter.</returns>
        public ExportConventionBuilder<TConvention> Export<TConvention>() where TConvention : IExportConvention, new()
        {
            return this.CreateExpressionBuilder<ExportConventionBuilder<TConvention>>();
        }
    }
}
