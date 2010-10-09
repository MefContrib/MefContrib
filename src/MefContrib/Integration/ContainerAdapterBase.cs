namespace MefContrib.Integration
{
    using System;

    /// <summary>
    /// Represents an abstract base class which all <see cref="IContainerAdapter"/>
    /// implementators can inherit from.
    /// </summary>
    public abstract class ContainerAdapterBase : IContainerAdapter
    {
        /// <summary>
        /// Event raised whenever a component gets registered in
        /// the underlying container.
        /// </summary>
        public event EventHandler<RegisterComponentEventArgs> RegisteringComponent;

        /// <summary>
        /// Method called by <see cref="ContainerExportProvider"/> to retrieve
        /// an instance of a given type.
        /// </summary>
        /// <param name="type">Type of the instance to retrieve.</param>
        /// <param name="name">Optional name.</param>
        /// <returns>An instance of a given type.</returns>
        public abstract object Resolve(Type type, string name);

        /// <summary>
        /// Method called by <see cref="ContainerExportProvider"/> in order
        /// to initialize the adapter.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Fires <see cref="RegisteringComponent"/> event.
        /// </summary>
        /// <param name="type">Type being registered.</param>
        /// <param name="name">Optional name.</param>
        protected void OnRegisteringComponent(Type type, string name)
        {
            if (RegisteringComponent != null)
                RegisteringComponent(this, new RegisterComponentEventArgs(type, name));
        }
    }
}