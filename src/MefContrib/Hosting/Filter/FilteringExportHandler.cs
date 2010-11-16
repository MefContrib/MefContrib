namespace MefContrib.Hosting.Filter
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using MefContrib.Hosting.Interception;

    /// <summary>
    /// Defines an export handler which performs filtering based on given criteria.
    /// </summary>
    public class FilteringExportHandler : IExportHandler
    {
        private readonly Func<ComposablePartDefinition, bool> filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteringExportHandler"/> class.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public FilteringExportHandler(Func<ComposablePartDefinition, bool> filter)
        {
            if (filter == null) throw new ArgumentNullException("filter");

            this.filter = filter;
        }

        /// <summary>
        /// Initializes this export handler.
        /// </summary>
        /// <param name="interceptedCatalog">The <see cref="ComposablePartCatalog"/> being intercepted.</param>
        public void Initialize(ComposablePartCatalog interceptedCatalog)
        {
        }

        /// <summary>
        /// Method which can filter exports for given <see cref="ImportDefinition"/> or produce new exports.
        /// </summary>
        /// <param name="definition"><see cref="ImportDefinition"/> instance.</param>
        /// <param name="exports">A collection of <see cref="ExportDefinition"/>
        /// instances along with their <see cref="ComposablePartDefinition"/> instances which match given <see cref="ImportDefinition"/>.</param>
        /// <returns>A collection of <see cref="ExportDefinition"/>
        /// instances along with their <see cref="ComposablePartDefinition"/> instances which match given <see cref="ImportDefinition"/>.</returns>
        public IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition, IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> exports)
        {
            return exports.Where(export => this.filter(export.Item1));
        }
    }
}