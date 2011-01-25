namespace MefContrib.Web.Mvc
{
    using System;

    /// <summary>
    /// IModelBinderMetaData
    /// </summary>
    public interface IModelBinderMetaData
    {
        /// <summary>
        /// Gets the type of the model.
        /// </summary>
        /// <value>The type of the model.</value>
        Type[] ModelType { get; }
    }
}
