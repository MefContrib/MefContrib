using System.ComponentModel.Composition.Primitives;

namespace MefContrib.Hosting
{
    /// <summary>
    /// A base interface for a filter used with <see cref="FilteredCatalog"/> class.
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// Decides whether given part satisfies a filter.
        /// </summary>
        /// <param name="part"><see cref="ComposablePartDefinition"/> being filtered.</param>
        /// <returns>True if a given <see cref="ComposablePartDefinition"/> satisfies the filter.
        /// False otherwise.</returns>
        bool Filter(ComposablePartDefinition part);
    }
}