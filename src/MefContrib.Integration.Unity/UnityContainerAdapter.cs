namespace MefContrib.Integration.Unity
{
    using System;
    using MefContrib.Integration.Exporters;
    using MefContrib.Integration.Unity.Extensions;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Represents an adapter for the <see cref="IUnityContainer"/> container.
    /// </summary>
    public class UnityContainerAdapter : ContainerAdapterBase
    {
        private readonly IUnityContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityContainerAdapter"/> class.
        /// </summary>
        /// <param name="container"><see cref="IUnityContainer"/> instance.</param>
        public UnityContainerAdapter(IUnityContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            this.container = container;
        }

        /// <summary>
        /// Method called by <see cref="ContainerExportProvider"/> to retrieve
        /// an instance of a given type.
        /// </summary>
        /// <param name="type">Type of the instance to retrieve.</param>
        /// <param name="name">Optional name.</param>
        /// <returns>An instance of a given type.</returns>
        public override object Resolve(Type type, string name)
        {
            return this.container.Resolve(type, name);
        }

        /// <summary>
        /// Method called by <see cref="ContainerExportProvider"/> in order
        /// to initialize the adapter.
        /// </summary>
        public override void Initialize()
        {
            TypeRegistrationTrackerExtension.RegisterIfMissing(container);

            var tracker = this.container.Configure<TypeRegistrationTrackerExtension>();

            foreach (var entry in tracker.Entries)
            {
                OnRegisteringComponent(entry.Type, entry.Name);
            }

            tracker.Registering += (s, e) =>
                OnRegisteringComponent(e.TypeFrom ?? e.TypeTo, e.Name);

            tracker.RegisteringInstance += (s, e) =>
                OnRegisteringComponent(e.RegisteredType, e.Name);
        }
    }
}