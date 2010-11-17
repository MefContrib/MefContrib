namespace MefContrib.Hosting
{
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;

    /// <summary>
    /// Defines a <see cref="ComposablePartCatalog"/> implementation that always returns an empty collection of parts.
    /// </summary>
    public class EmptyCatalog : ComposablePartCatalog
    {
        /// <summary>
        /// Gets an empty <see cref="IQueryable{T}"/> collection.
        /// </summary>
        /// <returns>An empty <see cref="IQueryable{T}"/> instance.</returns>
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get { return Enumerable.Empty<ComposablePartDefinition>().AsQueryable(); }
        }
    }
}