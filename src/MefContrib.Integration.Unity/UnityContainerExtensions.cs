using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using MefContrib.Integration.Unity.Exporters;
using MefContrib.Integration.Unity.Extensions;
using Microsoft.Practices.Unity;

namespace MefContrib.Integration.Unity
{
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
                    var unityExportProvider = new UnityExportProvider(unityContainer);
                    var parentExtension = (CompositionIntegration) null;
                    
                    if (unityContainer.Parent != null)
                    {
                        parentExtension = unityContainer.Parent.Configure<CompositionIntegration>();
                    }

                    if (parentExtension != null)
                    {
                        // Get the parent UnityExportProvider
                        var parentUnityExportProvider = (UnityExportProvider)parentExtension.Providers.Where(
                            ep => typeof(UnityExportProvider).IsAssignableFrom(ep.GetType())).First();

                        // Collect all the exports provided by the parent container and add
                        // them to the child export provider
                        foreach (var definition in parentUnityExportProvider.ReadOnlyDefinitions)
                        {
                            unityExportProvider.AddExportDefinition(
                                definition.ServiceType,
                                definition.RegistrationName);
                        }

                        // Grab all the parent export providers except the unity ones
                        var parentExporters = new List<ExportProvider>(
                            parentExtension.Providers.Where(
                                ep => !typeof (UnityExportProvider).IsAssignableFrom(ep.GetType())))
                                                  { unityExportProvider };

                        var catalog = new AggregateCatalog(parentExtension.Catalogs);

                        extension = new CompositionIntegration(true, parentExporters.ToArray());
                        extension.Catalogs.Add(catalog);
                    }
                    else
                    {
                        extension = new CompositionIntegration(true, unityExportProvider);
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

        /// <summary>
        /// Returns whether a specified type has a type mapping registered in the container.
        /// </summary>
        /// <param name="container">The <see cref="IUnityContainer"/> to check for the type mapping.</param>
        /// <param name="type">The type to check if there is a type mapping for.</param>
        /// <returns><see langword="true"/> if there is a type mapping registered for <paramref name="type"/>.</returns>
        /// <remarks>In order to use this extension method, you first need to add the
        /// <see cref="IUnityContainer"/> extension to the <see cref="TypeRegistrationTrackerExtension"/>.
        /// </remarks>        
        public static bool IsTypeRegistered(this IUnityContainer container, Type type)
        {
            return TypeRegistrationTrackerExtension.IsTypeRegistered(container, type);
        }
    }
}