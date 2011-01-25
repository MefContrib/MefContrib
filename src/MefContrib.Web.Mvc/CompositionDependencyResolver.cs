namespace MefContrib.Web.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// CompositionDependencyResolver
    /// </summary>
    public class CompositionDependencyResolver
        : IDependencyResolver, IDependencyBuilder
    {
        /// <summary>
        /// HttpContext key for the container.
        /// </summary>
        const string HttpContextKey = "__CompositionDependencyResolver_Container";

        private ComposablePartCatalog catalog;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionDependencyResolver"/> class.
        /// </summary>
        /// <param name="catalog">The catalog.</param>
        public CompositionDependencyResolver(ComposablePartCatalog catalog)
        {
            this.catalog = catalog;
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        internal CompositionContainer Container
        {
            get
            {
                if (!HttpContext.Current.Items.Contains(HttpContextKey))
                {
                    HttpContext.Current.Items.Add(HttpContextKey,
                        new CompositionContainer(catalog));
                }

                return (CompositionContainer)HttpContext.Current.Items[HttpContextKey];
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
                return exports.AsEnumerable();
            }
            return new List<object>();
        }

        public T Build<T>(T service)
        {
            this.Container.SatisfyImportsOnce(service);
            return service;
        }
    }
}
