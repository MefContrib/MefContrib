namespace MefContrib.Integration.Unity.Strategies
{
    using System.ComponentModel.Composition.Hosting;
    using Microsoft.Practices.ObjectBuilder2;

    /// <summary>
    /// Represents a builder policy which holds a <see cref="CompositionContainer"/> instance.
    /// </summary>
    public interface ICompositionContainerPolicy : IBuilderPolicy
    {
        /// <summary>
        /// Gets the <see cref="CompositionContainer"/> instance.
        /// </summary>
        CompositionContainer Container { get; }
    }
}