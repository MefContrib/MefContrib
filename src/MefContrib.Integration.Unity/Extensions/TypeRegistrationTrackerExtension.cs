namespace MefContrib.Integration.Unity.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Unity extension that exposes events which can be used
    /// to track types registered within <see cref="IUnityContainer"/> container.
    /// </summary>
    public sealed class TypeRegistrationTrackerExtension : UnityContainerExtension
    {
        private readonly List<TypeRegistrationEntry> entries;

        /// <summary>
        /// Event raised whenever an instance is being registered.
        /// </summary>
        public event EventHandler<RegisterInstanceEventArgs> RegisteringInstance;

        /// <summary>
        /// Event raised whenever a type is being registered.
        /// </summary>
        public event EventHandler<RegisterEventArgs> Registering;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeRegistrationTrackerExtension"/> class.
        /// </summary>
        public TypeRegistrationTrackerExtension()
        {
            this.entries = new List<TypeRegistrationEntry>();
        }

        /// <summary>
        /// Initial the container with this extension's functionality.
        /// </summary>
        /// <remarks>
        /// When overridden in a derived class, this method will modify the given
        /// <see cref="ExtensionContext"/> by adding strategies, policies, etc.
        /// to install it's functions into the container.
        /// </remarks>
        protected override void Initialize()
        {
            Context.Registering += OnRegistering;
            Context.RegisteringInstance += OnRegisteringInstance;
        }

        /// <summary>
        /// Removes the extension's functions from the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is called when extensions are being removed from the container. It can be
        /// used to do things like disconnect event handlers or clean up member state. You do not
        /// need to remove strategies or policies here; the container will do that automatically.
        /// </para>
        /// <para>
        /// The default implementation of this method does nothing.
        /// </para>
        /// </remarks>
        public override void Remove()
        {
            Context.Registering -= OnRegistering;
            Context.RegisteringInstance -= OnRegisteringInstance;
        }

        private void OnRegisteringInstance(object sender, RegisterInstanceEventArgs e)
        {
            this.entries.Add(new TypeRegistrationEntry(e.RegisteredType, e.Name));

            if (RegisteringInstance != null)
                RegisteringInstance(sender, e);
        }

        private void OnRegistering(object sender, RegisterEventArgs e)
        {
            this.entries.Add(new TypeRegistrationEntry(e.TypeFrom ?? e.TypeTo, e.Name));

            if (Registering != null)
                Registering(sender, e);
        }

        /// <summary>
        /// Gets all types registered int the <see cref="IUnityContainer"/> since
        /// this extension was enabled.
        /// </summary>
        public ReadOnlyCollection<TypeRegistrationEntry> Entries
        {
            get { return this.entries.AsReadOnly(); }
        }

        /// <summary>
        /// Helper method that registers <see cref="TypeRegistrationTrackerExtension"/> extensions
        /// in the Unity container if not previously registered.
        /// </summary>
        /// <param name="container">Target container.</param>
        public static void RegisterIfMissing(IUnityContainer container)
        {
            var extension = container.Configure<TypeRegistrationTrackerExtension>();
            if (extension == null)
                container.AddNewExtension<TypeRegistrationTrackerExtension>();
        }
    }
}