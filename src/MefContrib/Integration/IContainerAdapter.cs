namespace MefContrib.Integration
{
    using System;
    using MefContrib.Integration.Exporters;

    /// <summary>
    /// Represents an abstraction over a dependency injection container.
    /// </summary>
    public interface IContainerAdapter
    {
        /// <summary>
        /// Event raised whenever a component gets registered in
        /// the underlying container.
        /// </summary>
        event EventHandler<RegisterComponentEventArgs> RegisteringComponent;
        
        /// <summary>
        /// Method called by <see cref="ContainerExportProvider"/> to retrieve
        /// an instance of a given type.
        /// </summary>
        /// <param name="type">Type of the instance to retrieve.</param>
        /// <param name="name">Optional name.</param>
        /// <returns>An instance of a given type.</returns>
        object Resolve(Type type, string name);

        /// <summary>
        /// Method called by <see cref="ContainerExportProvider"/> in order
        /// to initialize the adapter.
        /// </summary>
        void Initialize();
    }
}