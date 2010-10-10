namespace MefContrib.Hosting.Filter
{
    using System;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Represents a catalog which wraps any <see cref="ComposablePartCatalog"/> and
    /// filters out <see cref="ComposablePartDefinition"/>s based on a given criteria.
    /// </summary>
    public class FilteringCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        private readonly INotifyComposablePartCatalogChanged innerNotifyChange;
        private readonly IQueryable<ComposablePartDefinition> partsQuery;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteringCatalog"/> class.
        /// </summary>
        /// <param name="inner">A <see cref="ComposablePartCatalog"/> whose parts
        /// are to be filtered based on a given criteria.</param>
        /// <param name="expression">An <see cref="Expression"/> which defines the filter query.</param>
        public FilteringCatalog(ComposablePartCatalog inner,
                               Expression<Func<ComposablePartDefinition, bool>> expression)
        {
            if (inner == null)
                throw new ArgumentNullException("inner");

            if (expression == null)
                throw new ArgumentNullException("expression");

            this.innerNotifyChange = inner as INotifyComposablePartCatalogChanged;
            this.partsQuery = inner.Parts.Where(expression);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteringCatalog"/> class.
        /// </summary>
        /// <param name="inner">A <see cref="ComposablePartCatalog"/> whose parts
        /// are to be filtered based on a given criteria.</param>
        /// <param name="filter">An instance of the <see cref="IFilter"/> interface
        /// to be used as a filter query.</param>
        public FilteringCatalog(ComposablePartCatalog inner, IFilter filter)
            : this(inner, p => filter.Filter(p))
        {
        }

        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                return this.partsQuery;
            }
        }

        #region INotifyComposablePartCatalogChanged Implementation

        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed
        {
            add
            {
                if (this.innerNotifyChange != null)
                    this.innerNotifyChange.Changed += value;
            }
            remove
            {
                if (this.innerNotifyChange != null)
                    this.innerNotifyChange.Changed -= value;
            }
        }

        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing
        {
            add
            {
                if (this.innerNotifyChange != null)
                    this.innerNotifyChange.Changing += value;
            }
            remove
            {
                if (this.innerNotifyChange != null)
                    this.innerNotifyChange.Changing -= value;
            }
        }

        #endregion
    }
}