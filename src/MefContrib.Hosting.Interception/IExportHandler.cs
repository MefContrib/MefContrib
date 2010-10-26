namespace MefContrib.Hosting.Interception
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Primitives;

    /// <summary>
    /// Defines an export handler which can be used to filter parts or create exports on the fly.
    /// </summary>
    public interface IExportHandler
    {
        /// <summary>
        /// Initializes this export handler.
        /// </summary>
        /// <param name="interceptedCatalog">The <see cref="ComposablePartCatalog"/> being intercepted.</param>
        void Initialize(ComposablePartCatalog interceptedCatalog);

        /// <summary>
        /// Method which can filter exports for given <see cref="ImportDefinition"/> or produce new exports.
        /// </summary>
        /// <param name="definition"><see cref="ImportDefinition"/> instance.</param>
        /// <param name="exports">A collection of <see cref="ExportDefinition"/>
        /// instances along with their <see cref="ComposablePartDefinition"/> instances which match given <see cref="ImportDefinition"/>.</param>
        /// <returns>A collection of <see cref="ExportDefinition"/>
        /// instances along with their <see cref="ComposablePartDefinition"/> instances which match given <see cref="ImportDefinition"/>.</returns>
        IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition, IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> exports);
    }
}
