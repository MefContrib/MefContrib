namespace MefContrib.Integration.Unity
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using MefContrib.Containers;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Contains extensions for the <see cref="IUnityContainer"/> interface.
    /// </summary>
    public static class UnityContainerExtensions
    {
        /// <summary>
        /// Registers a MEF catalog within Unity container.
        /// </summary>
        /// <param name="unityContainer">Unity container instance.</param>
        /// <param name="catalog">MEF catalog to be registered.</param>
        public static void RegisterCatalog(this IUnityContainer unityContainer, ComposablePartCatalog catalog)
        {
            lock (unityContainer)
            {
                var compositionIntegration = EnableCompositionIntegration(unityContainer);
                compositionIntegration.Catalogs.Add(catalog);
            }
        }

        /// <summary>
        /// Enables Managed Extensibility Framework two-way integration.
        /// </summary>
        /// <param name="unityContainer">Target container.</param>
        /// <returns>
        /// <see cref="CompositionIntegration"/> instance.
        /// </returns>
        public static CompositionIntegration EnableCompositionIntegration(
            this IUnityContainer unityContainer)
        {
            lock (unityContainer)
            {
                var extension = unityContainer.Configure<CompositionIntegration>();
                if (extension == null)
                {
                    var adapter = new UnityContainerAdapter(unityContainer);
                    var containerExportProvider = new ContainerExportProvider(adapter);
                    var parentExtension = (CompositionIntegration) null;
                    
                    if (unityContainer.Parent != null)
                    {
                        parentExtension = unityContainer.Parent.Configure<CompositionIntegration>();
                    }

                    if (parentExtension != null)
                    {
                        // Get the parent ContainerExportProvider
                        var parentContainerExportProvider = (ContainerExportProvider)parentExtension.Providers.Where(
                            ep => typeof(ContainerExportProvider).IsAssignableFrom(ep.GetType())).First();

                        // Collect all the exports provided by the parent container and add
                        // them to the child export provider
                        foreach (var definition in parentContainerExportProvider.FactoryExportProvider.ReadOnlyDefinitions)
                        {
                            containerExportProvider.FactoryExportProvider.Register(
                                definition.ContractType,
                                definition.RegistrationName);
                        }

                        // Grab all the parent export providers except the container ones
                        var parentExporters = new List<ExportProvider>(
                            parentExtension.Providers.Where(
                                ep => !typeof (ContainerExportProvider).IsAssignableFrom(ep.GetType())))
                                                  { containerExportProvider };

                        var catalog = new AggregateCatalog(parentExtension.Catalogs);

                        extension = new CompositionIntegration(true, parentExporters.ToArray());
                        extension.Catalogs.Add(catalog);
                    }
                    else
                    {
                        extension = new CompositionIntegration(true, containerExportProvider);
                    }

                    unityContainer.AddExtension(extension);
                }

                return extension;
            }
        }

        /// <summary>
        /// Creates child container.
        /// </summary>
        /// <param name="unityContainer">Target container.</param>
        /// <param name="enableComposition">True if the child container should
        /// support MEF integration. False otherwise.</param>
        /// <returns><see cref="IUnityContainer"/> child container.</returns>
        public static IUnityContainer CreateChildContainer(this IUnityContainer unityContainer, bool enableComposition)
        {
            var childContainer = unityContainer.CreateChildContainer();
            if (enableComposition)
            {
                childContainer.EnableCompositionIntegration();
            }

            return childContainer;
        }
    }
}