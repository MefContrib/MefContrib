namespace MefContrib.Web.Mvc
{
    using System.Web.Mvc;

    /// <summary>
    /// IDependencyBuilder
    /// </summary>
    public interface IDependencyBuilder
        : IDependencyResolver
    {
        /// <summary>
        /// Builds the specified service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        T Build<T>(T service);
    }
}
