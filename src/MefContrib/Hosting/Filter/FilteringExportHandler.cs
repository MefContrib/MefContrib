namespace MefContrib.Hosting.Filter
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using MefContrib.Hosting.Interception;

    public class FilteringExportHandler : IExportHandler
    {
        private readonly Func<ComposablePartDefinition, bool> filter;

        public FilteringExportHandler(Func<ComposablePartDefinition, bool> filter)
        {
            if (filter == null) throw new ArgumentNullException("filter");

            this.filter = filter;
        }

        public void Initialize(ComposablePartCatalog interceptedCatalog)
        {
        }

        public IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition, IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> exports)
        {
            return exports.Where(export => this.filter(export.Item1));
        }
    }
}