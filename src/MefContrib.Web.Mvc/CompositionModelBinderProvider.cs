namespace MefContrib.Web.Mvc
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// CompositionModelBinderProvider
    /// </summary>
    public class CompositionModelBinderProvider 
        : IModelBinderProvider
    {
        /// <summary>
        /// The dependency builder.
        /// </summary>
        private readonly CompositionDependencyResolver resolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionModelBinderProvider"/> class.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        public CompositionModelBinderProvider(CompositionDependencyResolver resolver)
        {
            this.resolver = resolver;
        }

        /// <summary>
        /// Gets the binder.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <returns></returns>
        public IModelBinder GetBinder(Type modelType)
        {
            var modelBinder = resolver.Container.GetExports<IModelBinder, IModelBinderMetaData>()
                .FirstOrDefault(b => b.Metadata.ModelType.Contains(modelType));
            if (modelBinder != null)
            {
                return modelBinder.Value;
            }
            return null;
        }
    }
}
