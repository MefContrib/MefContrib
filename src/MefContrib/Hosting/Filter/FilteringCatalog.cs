namespace MefContrib.Hosting.Filter
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using MefContrib.Hosting.Interception;
    using MefContrib.Hosting.Interception.Configuration;

    /// <summary>
    /// Represents a catalog which wraps any <see cref="ComposablePartCatalog"/> and
    /// filters out <see cref="ComposablePartDefinition"/>s based on a given criteria.
    /// </summary>
    public class FilteringCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        private readonly InterceptingCatalog interceptingCatalog;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteringCatalog"/> class.
        /// </summary>
        /// <param name="inner">A <see cref="ComposablePartCatalog"/> whose parts
        /// are to be filtered based on a given criteria.</param>
        /// <param name="filter">A filter query.</param>
        public FilteringCatalog(ComposablePartCatalog inner, Func<ComposablePartDefinition, bool> filter)
        {
            if (inner == null)
                throw new ArgumentNullException("inner");

            if (filter == null)
                throw new ArgumentNullException("filter");

            var cfg = new InterceptionConfiguration()
                .AddHandler(new FilteringPartHandler(filter));
            this.interceptingCatalog = new InterceptingCatalog(inner, cfg);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteringCatalog"/> class.
        /// </summary>
        /// <param name="inner">A <see cref="ComposablePartCatalog"/> whose parts
        /// are to be filtered based on a given criteria.</param>
        /// <param name="filter">An instance of the <see cref="IFilter"/> interface
        /// to be used as a filter query.</param>
        public FilteringCatalog(ComposablePartCatalog inner, IFilter filter)
            : this(inner, filter.Filter)
        {
        }

        /// <summary>
        /// Gets the part definitions of the catalog.
        /// </summary>
        /// <value>A <see cref="IQueryable{T}"/> of <see cref="ComposablePartDefinition"/> objects of the <see cref="FilteringCatalog"/>.</value>
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get { return this.interceptingCatalog.Parts; }
        }

        /// <summary>
        /// Method which can filter exports for given <see cref="ImportDefinition"/> or produce new exports.
        /// </summary>
        /// <param name="definition"><see cref="ImportDefinition"/> instance.</param>
        /// <returns>
        /// A collection of <see cref="ExportDefinition"/>
        /// instances along with their <see cref="ComposablePartDefinition"/> instances which match given <see cref="ImportDefinition"/>.
        /// </returns>
        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            return this.interceptingCatalog.GetExports(definition);
        }
        
        #region INotifyComposablePartCatalogChanged Implementation

        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed
        {
            add { this.interceptingCatalog.Changed += value; }
            remove { this.interceptingCatalog.Changed -= value; }
        }

        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing
        {
            add { this.interceptingCatalog.Changing += value; }
            remove { this.interceptingCatalog.Changing -= value; }
        }

        #endregion
    }
}