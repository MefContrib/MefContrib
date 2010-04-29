using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MefContrib.Integration.Unity.Properties;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace MefContrib.Integration.Unity.Extensions
{
    /// <summary>
    /// Unity extension that exposes events which can be used
    /// to track types registered within <see cref="IUnityContainer"/> container.
    /// </summary>
    public sealed class TypeRegistrationTrackerExtension : UnityContainerExtension
    {
        private readonly List<TypeRegistrationEntry> m_Entries;

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
            m_Entries = new List<TypeRegistrationEntry>();
        }

        protected override void Initialize()
        {
            Context.Registering += OnRegistering;
            Context.RegisteringInstance += OnRegisteringInstance;
        }

        public override void Remove()
        {
            Context.Registering -= OnRegistering;
            Context.RegisteringInstance -= OnRegisteringInstance;
        }

        private void OnRegisteringInstance(object sender, RegisterInstanceEventArgs e)
        {
            m_Entries.Add(new TypeRegistrationEntry(e.RegisteredType, e.Name));

            if (RegisteringInstance != null)
                RegisteringInstance(sender, e);
        }

        private void OnRegistering(object sender, RegisterEventArgs e)
        {
            m_Entries.Add(new TypeRegistrationEntry(e.TypeFrom ?? e.TypeTo, e.Name));

            if (Registering != null)
                Registering(sender, e);
        }

        /// <summary>
        /// Gets all types registered int the <see cref="IUnityContainer"/> since
        /// this extension was enabled.
        /// </summary>
        public ReadOnlyCollection<TypeRegistrationEntry> Entries
        {
            get { return m_Entries.AsReadOnly(); }
        }

        /// <summary>
        /// Evaluates if a specified type was registered in the container.
        /// </summary>
        /// <param name="container">The container to check if the type was registered in.</param>
        /// <param name="type">The type to check if it was registered.</param>
        /// <returns><see langword="true" /> if the <paramref name="type"/> was registered with the container.</returns>
        /// <remarks>
        /// In order to use this extension, you must first call <see cref="IUnityContainer.AddNewExtension{TExtension}"/> 
        /// and specify <see cref="UnityContainerExtension"/> as the extension type.
        /// </remarks>
        public static bool IsTypeRegistered(IUnityContainer container, Type type)
        {
            var extension = container.Configure<TypeRegistrationTrackerExtension>();
            if (extension == null)
            {
                // Extension was not added to the container.
                throw new InvalidOperationException(string.Format(Resources.ExtensionMissing,
                    typeof(TypeRegistrationTrackerExtension).Name));
            }

            var policy = extension.Context.Policies.Get<IBuildKeyMappingPolicy>(new NamedTypeBuildKey(type));
            return policy != null;
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