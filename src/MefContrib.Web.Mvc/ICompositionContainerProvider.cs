namespace MefContrib.Web.Mvc
{
    using System.ComponentModel.Composition.Hosting;

    /// <summary>
    /// ICompositionContainerProvider
    /// </summary>
    public interface ICompositionContainerProvider
    {
        /// <summary> 
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        CompositionContainer Container { get; }
    }
}