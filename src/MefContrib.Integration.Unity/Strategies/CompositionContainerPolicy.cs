using System;
using System.ComponentModel.Composition.Hosting;

namespace MefContrib.Integration.Unity.Strategies
{
    /// <summary>
    /// Default implementation of the <see cref="ICompositionContainerPolicy"/> interface.
    /// </summary>
    public class CompositionContainerPolicy : ICompositionContainerPolicy
    {
        private readonly CompositionContainer m_Container;

        /// <summary>
        /// Creates a new instance of the <see cref="CompositionContainerPolicy"/> class.
        /// </summary>
        /// <param name="container">Instance of the <see cref="CompositionContainer"/>
        /// this policy holds.</param>
        public CompositionContainerPolicy(CompositionContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            m_Container = container;
        }

        /// <summary>
        /// Gets the <see cref="CompositionContainer"/> instance.
        /// </summary>
        public CompositionContainer Container
        {
            get { return m_Container; }
        }
    }
}