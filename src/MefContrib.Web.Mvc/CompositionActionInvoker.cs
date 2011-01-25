namespace MefContrib.Web.Mvc
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.Mvc.Async;

    /// <summary>
    /// CompositionActionInvoker
    /// </summary>
    public class CompositionActionInvoker
        : AsyncControllerActionInvoker 
    {
        /// <summary>
        /// The dependency builder.
        /// </summary>
        private readonly IDependencyBuilder builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionActionInvoker"/> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public CompositionActionInvoker(IDependencyBuilder builder)
        {
            this.builder = builder;
        }

        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param><param name="actionDescriptor">The action descriptor.</param>
        /// <returns>
        /// The filter information object.
        /// </returns>
        protected override FilterInfo GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var filters = base.GetFilters(controllerContext, actionDescriptor);
            BuildFilters(filters.ActionFilters);
            BuildFilters(filters.AuthorizationFilters);
            BuildFilters(filters.ExceptionFilters);
            BuildFilters(filters.ResultFilters);
            return filters;
        }

        /// <summary>
        /// Gets the parameter value.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param><param name="parameterDescriptor">The parameter descriptor.</param>
        /// <returns>
        /// The parameter value.
        /// </returns>
        protected override object GetParameterValue(ControllerContext controllerContext, ParameterDescriptor parameterDescriptor)
        {
            return this.builder.GetService(parameterDescriptor.ParameterType)
                ?? base.GetParameterValue(controllerContext, parameterDescriptor);
        }

        /// <summary>
        /// Builds the filters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filters">The filters.</param>
        private void BuildFilters<T>(ICollection<T> filters)
        {
            foreach (var filter in filters)
            {
                this.builder.Build(filter);
            }
        }
    }
}
