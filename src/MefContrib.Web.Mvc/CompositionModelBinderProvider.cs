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
        private readonly ICompositionContainerProvider compositionContainerProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionModelBinderProvider"/> class.
        /// </summary>
        /// <param name="compositionContainerProvider">The compositionContainerProvider.</param>
        public CompositionModelBinderProvider(ICompositionContainerProvider compositionContainerProvider)
        {
            this.compositionContainerProvider = compositionContainerProvider;
        }

        /// <summary>
        /// Gets the binder.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <returns></returns>
        public IModelBinder GetBinder(Type modelType)
        {
            var modelBinder = compositionContainerProvider.Container.GetExports<IModelBinder, IModelBinderMetaData>()
                .FirstOrDefault(b => b.Metadata.ModelType.Contains(modelType));
            if (modelBinder != null)
            {
                return modelBinder.Value;
            }
            return null;
        }
    }
}
