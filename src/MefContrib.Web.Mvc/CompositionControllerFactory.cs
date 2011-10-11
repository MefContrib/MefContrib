namespace MefContrib.Web.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Routing;

    public class CompositionControllerFactory
        : DefaultControllerFactory
    {
        private readonly ICompositionContainerProvider compositionContainerProvider;

        public CompositionControllerFactory()
        {
        }

        public CompositionControllerFactory(IControllerActivator activator)
            : this(activator, DependencyResolver.Current as ICompositionContainerProvider)
        {
        }

        public CompositionControllerFactory(IControllerActivator activator,
                                            ICompositionContainerProvider compositionContainerProvider)
            : base(activator)
        {
            this.compositionContainerProvider = compositionContainerProvider;
        }

        protected override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            // First, try resolving through ASP.NET MVC
            var controllerType = base.GetControllerType(requestContext, controllerName);

            // If that does not work out, check our exports
            if (controllerType == null && this.compositionContainerProvider != null &&
                this.compositionContainerProvider.Container != null)
            {
                var controllerTypes =
                    this.compositionContainerProvider.Container.GetExports<IController>()
                        .Where(e => e.Value.GetType().Name.ToLowerInvariant() == controllerName.ToLowerInvariant() + "controller")
                        .Select(e => e.Value.GetType()).ToList();

                switch (controllerTypes.Count)
                {
                    case 0:
                        controllerType = null;
                        break;
                    case 1:
                        controllerType = controllerTypes.First();
                        break;
                    case 2:
                        throw CreateAmbiguousControllerException(requestContext.RouteData.Route, controllerName, controllerTypes);
                }
            }

            // Finally, make sure we return something
            return controllerType;
        }

        /// <summary>
        /// Creates the ambiguous controller exception.
        /// </summary>
        /// <param name="route">The route.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="matchingTypes">The matching types.</param>
        /// <returns></returns>
        internal static InvalidOperationException CreateAmbiguousControllerException(RouteBase route, string controllerName, ICollection<Type> matchingTypes)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Type current in matchingTypes)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(current.FullName);
            }
            Route route2 = route as Route;
            string message;
            if (route2 != null)
            {
                message = string.Format(CultureInfo.CurrentUICulture,
                                        "The request for '{0}' has found the following matching controllers:{2}\r\n\r\nMultiple types were found that match the controller named '{0}'. This can happen if the route that services this request does not specify namespaces to search for a controller that matches the request. If this is the case, register this route by calling an overload of the 'MapRoute' method that takes a 'namespaces' parameter.",
                                        new object[]
                                        {
                                            controllerName,
                                            route2.Url,
                                            stringBuilder
                                        });
            }
            else
            {
                message = string.Format(CultureInfo.CurrentUICulture,
                                        "The request for '{0}' has found the following matching controllers:{2}\r\n\r\nMultiple types were found that match the controller named '{0}'.",
                                        new object[]
                                        {
                                            controllerName,
                                            stringBuilder
                                        });
            }
            return new InvalidOperationException(message);
        }
    }
}