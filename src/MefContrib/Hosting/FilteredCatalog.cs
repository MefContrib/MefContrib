using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Linq.Expressions;

namespace MefContrib.Hosting
{
    /// <summary>
    /// Represents a catalog which wraps any <see cref="ComposablePartCatalog"/> and
    /// filters out <see cref="ComposablePartDefinition"/>s based on a given criteria.
    /// </summary>
    public class FilteredCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        private readonly INotifyComposablePartCatalogChanged m_InnerNotifyChange;
        private readonly IQueryable<ComposablePartDefinition> m_PartsQuery;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredCatalog"/> class.
        /// </summary>
        /// <param name="inner">A <see cref="ComposablePartCatalog"/> whose parts
        /// are to be filtered based on a given criteria.</param>
        /// <param name="expression">An <see cref="Expression"/> which defines the filter query.</param>
        public FilteredCatalog(ComposablePartCatalog inner,
                               Expression<Func<ComposablePartDefinition, bool>> expression)
        {
            if (inner == null)
                throw new ArgumentNullException("inner");

            if (expression == null)
                throw new ArgumentNullException("expression");

            m_InnerNotifyChange = inner as INotifyComposablePartCatalogChanged;
            m_PartsQuery = inner.Parts.Where(expression);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredCatalog"/> class.
        /// </summary>
        /// <param name="inner">A <see cref="ComposablePartCatalog"/> whose parts
        /// are to be filtered based on a given criteria.</param>
        /// <param name="filter">An instance of the <see cref="IFilter"/> interface
        /// to be used as a filter query.</param>
        public FilteredCatalog(ComposablePartCatalog inner, IFilter filter)
            : this(inner, p => filter.Filter(p))
        {
        }

        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                return m_PartsQuery;
            }
        }

        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed
        {
            add
            {
                if (m_InnerNotifyChange != null)
                    m_InnerNotifyChange.Changed += value;
            }
            remove
            {
                if (m_InnerNotifyChange != null)
                    m_InnerNotifyChange.Changed -= value;
            }
        }

        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing
        {
            add
            {
                if (m_InnerNotifyChange != null)
                    m_InnerNotifyChange.Changing += value;
            }
            remove
            {
                if (m_InnerNotifyChange != null)
                    m_InnerNotifyChange.Changing -= value;
            }
        }
    }
}