using System.ComponentModel.Composition.Hosting;
using Microsoft.Practices.ObjectBuilder2;

namespace MefContrib.Integration.Unity.Strategies
{
    /// <summary>
    /// Represents a builder strategy which holds a <see cref="CompositionContainer"/> instance.
    /// </summary>
    public interface ICompositionContainerPolicy : IBuilderPolicy
    {
        /// <summary>
        /// Gets the <see cref="CompositionContainer"/> instance.
        /// </summary>
        CompositionContainer Container { get; }
    }
}