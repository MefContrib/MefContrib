namespace MefContrib.Web.Mvc
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    /// <summary>
    /// CompositionFilterAttributeFilterProvider
    /// </summary>
    public class CompositionFilterAttributeFilterProvider
        : FilterAttributeFilterProvider
    {
        /// <summary>
        /// The dependency builder.
        /// </summary>
        private readonly IDependencyBuilder builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionFilterAttributeFilterProvider"/> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public CompositionFilterAttributeFilterProvider(IDependencyBuilder builder)
        {
            this.builder = builder;
        }

        /// <summary>
        /// Gets the controller attributes.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <returns>The filters defined by attributes</returns>
        protected override IEnumerable<FilterAttribute> GetControllerAttributes(
            ControllerContext controllerContext,
            ActionDescriptor actionDescriptor)
        {
            var attributes = base.GetControllerAttributes(controllerContext, actionDescriptor);
            foreach (var attribute in attributes)
            {
                this.builder.Build(attribute);
            }

            return attributes;
        }

        /// <summary>
        /// Gets the action attributes.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <returns>The filters defined by attributes.</returns>
        protected override IEnumerable<FilterAttribute> GetActionAttributes(
            ControllerContext controllerContext,
            ActionDescriptor actionDescriptor)
        {
            var attributes = base.GetActionAttributes(controllerContext, actionDescriptor);
            foreach (var attribute in attributes)
            {
                this.builder.Build(attribute);
            }

            return attributes;
        }
    }
}
