#region Header

// -----------------------------------------------------------------------------
//  Copyright (c) Edenred (Incentives & Motivation) Ltd.  All rights reserved.
// -----------------------------------------------------------------------------

#endregion

namespace MefContrib.Integration.Autofac
{
    using System.ComponentModel.Composition.Primitives;
    using global::Autofac;

    /// <summary>
    ///   Contains extensions for the <see cref = "ILifetimeScope" /> interface.
    /// </summary>
    public static class ContainerExtensions
    {
        #region Methods

        /// <summary>
        ///   Enables Managed Extensibility Framework two-way integration.
        /// </summary>
        /// <param name = "builder">The builder.</param>
        public static void EnableCompositionIntegration(this ContainerBuilder builder)
        {
            builder.RegisterType<CompositionIntegration>()
                .AsSelf()
                .As<IStartable>()
                //.SingleInstance()
                .OwnedByLifetimeScope()
                .InstancePerLifetimeScope()
                .OnActivated(e => e.Instance.Start());
        }

        /// <summary>
        ///   Enables Managed Extensibility Framework two-way integration.
        /// </summary>
        /// <param name = "container">Target container.</param>
        /// <returns>
        ///   <see cref = "CompositionIntegration" /> instance.
        /// </returns>
        public static CompositionIntegration EnableCompositionIntegration(this IComponentContext container)
        {
            lock (container)
            {
                if (!container.IsRegistered<CompositionIntegration>())
                {
                    var builder = new ContainerBuilder();
                    builder.EnableCompositionIntegration();
                    builder.Update(container.ComponentRegistry);
                }

                return container.Resolve<CompositionIntegration>();
            }
        }

        /// <summary>
        ///   Registers a MEF catalog within an Autofac container.
        /// </summary>
        /// <param name = "container">Autofac container instance.</param>
        /// <param name = "catalog">MEF catalog to be registered.</param>
        public static void RegisterCatalog(this IComponentContext container, ComposablePartCatalog catalog)
        {
            lock (container)
            {
                var compositionIntegration = EnableCompositionIntegration(container);
                compositionIntegration.Catalogs.Add(catalog);
            }
        }

        /// <summary>
        ///   Registers a MEF catalog within an Autofac container.
        /// </summary>
        /// <param name = "builder">The builder.</param>
        /// <param name = "catalog">MEF catalog to be registered.</param>
        public static void RegisterCatalog(this ContainerBuilder builder, ComposablePartCatalog catalog)
        {
            builder.RegisterInstance(catalog).OwnedByLifetimeScope();
        }

        #endregion
    }
}