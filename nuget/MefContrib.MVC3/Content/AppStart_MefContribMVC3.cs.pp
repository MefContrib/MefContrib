[assembly: WebActivator.PreApplicationStartMethod(typeof($rootnamespace$.AppStart_MefContribMVC3), "Start")]

namespace $rootnamespace$
{
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using System.Web.Mvc;

    public static class AppStart_MefContribMVC3
    {
        public static void Start()
        {
            // Create MEF catalog based on the contents of ~/bin.
            // Note that any class in the referenced assemblies implementing in "IController"
            // is automatically exported to MEF. There is no need for explicit [Export] attributes
            // on ASP.NET MVC controllers. When implementing multiple constructors ensure that
            // there is one constructor marked with the [ImportingConstructor] attribute.
            var catalog = new MvcApplicationCatalog();

            // Tell MVC3 to use MEF as its dependency resolver.
            var dependencyResolver = new CompositionDependencyResolver(catalog);
            DependencyResolver.SetResolver(dependencyResolver);

            // Tell MVC3 to resolve dependencies in controllers
            ControllerBuilder.Current.SetControllerFactory(
                new DefaultControllerFactory(
                    new CompositionControllerActivator(dependencyResolver)));

            // Tell MVC3 to resolve dependencies in filters
            FilterProviders.Providers.Remove(FilterProviders.Providers.Single(f => f is FilterAttributeFilterProvider));
            FilterProviders.Providers.Add(new CompositionFilterAttributeFilterProvider(dependencyResolver));

            // Tell MVC3 to resolve dependencies in model validators
            ModelValidatorProviders.Providers.Remove(ModelValidatorProviders.Providers.OfType<DataAnnotationsModelValidatorProvider>().Single());
            ModelValidatorProviders.Providers.Add(
                new CompositionDataAnnotationsModelValidatorProvider(dependencyResolver));

            // Tell MVC3 to resolve model binders through MEF. Note that a model binder should be decorated
            // with [ModelBinderExport].
            ModelBinderProviders.BinderProviders.Add(
                new CompositionModelBinderProvider(dependencyResolver));
        }
    }
}