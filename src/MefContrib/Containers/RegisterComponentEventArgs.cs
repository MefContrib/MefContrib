namespace MefContrib.Containers
{
    using System;

    /// <summary>
    /// Represents arguments for <see cref="IContainerAdapter.RegisteringComponent"/> event.
    /// </summary>
    public class RegisterComponentEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterComponentEventArgs"/> class.
        /// </summary>
        /// <param name="type"><see cref="Type"/> being registered.</param>
        /// <param name="name">Optional name.</param>
        public RegisterComponentEventArgs(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> being registered.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the optional name used during component registration.
        /// </summary>
        public string Name { get; private set; }
    }
}