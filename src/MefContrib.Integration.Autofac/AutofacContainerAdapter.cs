#region Header

// -----------------------------------------------------------------------------
//  Copyright (c) Edenred (Incentives & Motivation) Ltd.  All rights reserved.
// -----------------------------------------------------------------------------

#endregion

namespace MefContrib.Integration.Autofac
{
    using System;
    using System.Linq;
    using Containers;
    using global::Autofac;
    using global::Autofac.Core;

    /// <summary>
    ///   Represents an adapter for the <see cref = "ILifetimeScope" /> container.
    /// </summary>
    public class AutofacContainerAdapter : ContainerAdapterBase
    {
        #region Fields

        private readonly ILifetimeScope _container;

        #endregion

        #region Constructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "AutofacContainerAdapter" /> class.
        /// </summary>
        /// <param name = "container"><see cref = "ILifetimeScope" /> instance.</param>
        public AutofacContainerAdapter(ILifetimeScope container)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            _container = container;
            OnRegisteringComponent(typeof (ILifetimeScope), null);
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Method called by <see cref = "ContainerExportProvider" /> in order
        ///   to initialize the adapter.
        /// </summary>
        public override void Initialize()
        {
            foreach (var registration in _container.ComponentRegistry.Registrations)
            {
                RegisterComponents(registration);
            }
            _container.ComponentRegistry.Registered += (s, e) => RegisterComponents(e.ComponentRegistration);
        }

        private void RegisterComponents(IComponentRegistration registration)
        {
            if (registration.Metadata.ContainsKey("Source") && registration.Metadata["Source"] as string == "MEF")
                return;
            var exportConfigurations =
                registration.Services.OfType<TypedService>()
                    .Select(svc => new {svc.ServiceType, ServiceName = (string) null})
                    .Union(registration.Services.OfType<KeyedService>()
                               .Select(svc => new {svc.ServiceType, ServiceName = svc.ServiceKey.ToString()}));
            foreach (var exportConfiguration in exportConfigurations)
            {
                OnRegisteringComponent(exportConfiguration.ServiceType, exportConfiguration.ServiceName);
            }
        }

        /// <summary>
        ///   Method called by <see cref = "ContainerExportProvider" /> to retrieve
        ///   an instance of a given type.
        /// </summary>
        /// <param name = "type">Type of the instance to retrieve.</param>
        /// <param name = "name">Optional name.</param>
        /// <returns>An instance of a given type.</returns>
        public override object Resolve(Type type, string name)
        {
            return name == null ? _container.Resolve(type) : _container.ResolveNamed(name, type);
        }

        #endregion
    }
}