namespace MefContrib.Web.Mvc
{
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// CompositionBootstrapper
    /// </summary>
    public static class CompositionBootstrapper
    {
        /// <summary>
        /// Bootstraps System.ComponentModel.Composition based application components.
        /// </summary>
        public static void Bootstrap()
        {
            Bootstrap(new MvcApplicationCatalog());
        }

        /// <summary>
        /// Bootstraps System.ComponentModel.Composition based application components.
        /// </summary>
        /// <param name="catalog">The catalog.</param>
        public static void Bootstrap(ComposablePartCatalog catalog)
        {
            var dependencyResolver = new CompositionDependencyResolver(catalog);

            // Set dependency resolver
            DependencyResolver.SetResolver(dependencyResolver);

            // Set filter provider
            FilterProviders.Providers.Remove(FilterProviders.Providers.Single(f => f is FilterAttributeFilterProvider));
            FilterProviders.Providers.Add(new CompositionFilterAttributeFilterProvider(dependencyResolver));

            // Set model validation provider
            ModelValidatorProviders.Providers.Remove(ModelValidatorProviders.Providers.OfType<DataAnnotationsModelValidatorProvider>().Single());
            ModelValidatorProviders.Providers.Add(
                new CompositionDataAnnotationsModelValidatorProvider(dependencyResolver));

            // Model binders
            ModelBinderProviders.BinderProviders.Add(
                new CompositionModelBinderProvider(dependencyResolver));

            // Controller factory
            ControllerBuilder.Current.SetControllerFactory(
                new DefaultControllerFactory(
                    new CompositionControllerActivator(dependencyResolver)));
        }
    }
}
