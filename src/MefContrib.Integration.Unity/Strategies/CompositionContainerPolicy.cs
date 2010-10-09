namespace MefContrib.Integration.Unity.Strategies
{
    using System;
    using System.ComponentModel.Composition.Hosting;

    /// <summary>
    /// Default implementation of the <see cref="ICompositionContainerPolicy"/> interface.
    /// </summary>
    public class CompositionContainerPolicy : ICompositionContainerPolicy
    {
        private readonly CompositionContainer container;

        /// <summary>
        /// Creates a new instance of the <see cref="CompositionContainerPolicy"/> class.
        /// </summary>
        /// <param name="container">Instance of the <see cref="CompositionContainer"/>
        /// this policy holds.</param>
        public CompositionContainerPolicy(CompositionContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            this.container = container;
        }

        /// <summary>
        /// Gets the <see cref="CompositionContainer"/> instance.
        /// </summary>
        public CompositionContainer Container
        {
            get { return this.container; }
        }
    }
}