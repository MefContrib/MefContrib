namespace MefContrib.Web.Mvc
{
    using System;
    using System.ComponentModel.Composition;
    using System.Web.Mvc;

    /// <summary>
    /// ModelBinderExportAttribute
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ModelBinderExportAttribute
        : ExportAttribute, IModelBinderMetaData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelBinderExportAttribute"/> class.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        public ModelBinderExportAttribute(Type[] modelType)
            : base(typeof(IModelBinder))
        {
            ModelType = modelType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelBinderExportAttribute"/> class.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        public ModelBinderExportAttribute(Type modelType)
            : base(typeof(IModelBinder))
        {
            ModelType = new Type[] { modelType };
        }

        /// <summary>
        /// Gets or sets the type of the model.
        /// </summary>
        /// <value>The type of the model.</value>
        public Type[] ModelType { get; set; }
    }
}
