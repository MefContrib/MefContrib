namespace MefContrib.Web.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using System.Web.Mvc;
    using MefContrib.Hosting.Filter;
    using MefContrib.Web.Mvc.Filter;

    /// <summary>
    /// CompositionDependencyResolver
    /// </summary>
    public class CompositionDependencyResolver
        : IDependencyResolver, IDependencyBuilder, IServiceProvider
    {
        /// <summary>
        /// HttpContext key for the container.
        /// </summary>
        public const string HttpContextKey = "__CompositionDependencyResolver_Container";

        private ComposablePartCatalog completeCatalog;
        private ComposablePartCatalog globalCatalog;
        private CompositionContainer globalContainer;
        private ComposablePartCatalog filteredCatalog;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionDependencyResolver"/> class.
        /// </summary>
        /// <param name="catalog">The catalog.</param>
        public CompositionDependencyResolver(ComposablePartCatalog catalog)
        {
            // Keep the original catalog
            this.completeCatalog = catalog;

            // Filter the global part catalog to a set of parts that define PartCreationScope.Global.
            this.globalCatalog = new FilteringCatalog(
                this.completeCatalog, new HasPartCreationScope(PartCreationScope.Global));
            this.globalContainer = new CompositionContainer(this.globalCatalog, true, null);

            // Filter the per-request part catalog to a set of parts that define PartCreationScope.PerRequest.
            this.filteredCatalog = new FilteringCatalog(
                this.completeCatalog, new HasPartCreationScope(PartCreationScope.PerRequest));
        }

        /// <summary> 
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        public CompositionContainer Container
        {
            get
            {
                if (!CurrentRequestContext.Items.Contains(HttpContextKey))
                {
                    CurrentRequestContext.Items.Add(HttpContextKey,
                        new CompositionContainer(this.filteredCatalog, true, this.globalContainer));
                }

                return (CompositionContainer)CurrentRequestContext.Items[HttpContextKey];
            }
        }

        /// <summary>
        /// Resolves singly registered services that support arbitrary object creation.
        /// </summary>
        /// <param name="serviceType">The type of the requested service or object.</param>
        /// <returns>The requested service or object.</returns>
        public object GetService(Type serviceType)
        {
            var exports = this.Container.GetExports(serviceType, null, null);
            if (exports.Any())
            {
                return exports.First().Value;
            }
            return null;
        }

        /// <summary>
        /// Resolves multiply registered services.
        /// </summary>
        /// <param name="serviceType">The type of the requested services.</param>
        /// <returns>The requested services.</returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            var exports = this.Container.GetExports(serviceType, null, null);
            if (exports.Any())
            {
                return exports.Select(e => e.Value).AsEnumerable();
            }
            return new List<object>();
        }

        /// <summary>
        /// Builds the specified service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public T Build<T>(T service)
        {
            this.Container.SatisfyImportsOnce(service);
            return service;
        }
    }
}
