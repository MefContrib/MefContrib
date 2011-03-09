namespace MefContrib.Hosting.Filter
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using MefContrib.Hosting.Interception;

    /// <summary>
    /// Defines a part handler which performs filtering based on a given criteria.
    /// </summary>
    public class FilteringPartHandler : IPartHandler
    {
        private readonly Func<ComposablePartDefinition, bool> filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteringPartHandler"/> class.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public FilteringPartHandler(Func<ComposablePartDefinition, bool> filter)
        {
            if (filter == null) throw new ArgumentNullException("filter");

            this.filter = filter;
        }

        /// <summary>
        /// Initializes the part handler.
        /// </summary>
        /// <param name="interceptedCatalog">The <see cref="ComposablePartCatalog"/> being intercepted.</param>
        public void Initialize(ComposablePartCatalog interceptedCatalog)
        {
        }

        /// <summary>
        /// Method which can filter or produce parts.
        /// </summary>
        /// <param name="parts">An <see cref="IEnumerable{T}"/> of <see cref="ComposablePartDefinition"/> instances.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ComposablePartDefinition"/> instances.</returns>
        /// <remarks>This method is called at most once when the <see cref="InterceptingCatalog.Parts"/>
        /// property is being calculated.</remarks>
        public IEnumerable<ComposablePartDefinition> GetParts(IEnumerable<ComposablePartDefinition> parts)
        {
            return parts.Where(part => this.filter(part));
        }

        /// <summary>
        /// Occurs when <see cref="IPartHandler"/> is changed.
        /// </summary>
        public event EventHandler<PartHandlerChangedEventArgs> Changed;
    }
}