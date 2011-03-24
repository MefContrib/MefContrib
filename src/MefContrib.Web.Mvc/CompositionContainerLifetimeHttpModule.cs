using System;
using System.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

namespace MefContrib.Web.Mvc
{
    /// <summary>
    /// CompositionContainerLifetimeHttpModule is responsiblefor cleaning up per-request containers.
    /// </summary>
    public class CompositionContainerLifetimeHttpModule
        : IHttpModule
    {
        private static bool isRegistered;

        /// <summary>
        /// Registers this instance. Can only be called once per application.
        /// </summary>
        public static void Register()
        {
            // All Register calls are made on the same thread, so no lock needed here.
            if (isRegistered) return;

            isRegistered = true;
            DynamicModuleUtility.RegisterModule(typeof(CompositionContainerLifetimeHttpModule));
        }

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpApplication"/> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
        public void Init(HttpApplication context)
        {
            context.EndRequest += OnEndRequest;
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Called when [end request].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public static void OnEndRequest(object sender, EventArgs e)
        {
            if (CurrentRequestContext.Items.Contains(CompositionDependencyResolver.HttpContextKey))
            {
                var disposable = CurrentRequestContext.Items[CompositionDependencyResolver.HttpContextKey] as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}