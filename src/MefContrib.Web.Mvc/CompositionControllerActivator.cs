namespace MefContrib.Web.Mvc
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// CompositionControllerActivator
    /// </summary>
    public class CompositionControllerActivator
        : IControllerActivator
    {
        /// <summary>
        /// The dependency builder.
        /// </summary>
        private readonly IDependencyBuilder builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionControllerActivator"/> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public CompositionControllerActivator(IDependencyBuilder builder)
        {
            this.builder = builder;
        }

        /// <summary>
        /// Creates a controller.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <param name="controllerType">The controller type.</param>
        /// <returns>The created controller.</returns>
        public IController Create(RequestContext requestContext, Type controllerType)
        {
            return this.builder.GetService(controllerType) as IController;
        } 
    }
}
