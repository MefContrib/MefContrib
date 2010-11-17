namespace MefContrib.Hosting.Interception.Configuration
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the interception configuration.
    /// </summary>
    public class InterceptionConfiguration : IInterceptionConfiguration
    {
        private readonly List<IExportedValueInterceptor> interceptors;
        private readonly List<IPartInterceptionCriteria> interceptionCriteria;
        private readonly List<IExportHandler> exportHandlers;
        private readonly List<IPartHandler> partHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptingCatalog"/> class.
        /// </summary>
        public InterceptionConfiguration()
        {
            this.interceptors = new List<IExportedValueInterceptor>();
            this.interceptionCriteria = new List<IPartInterceptionCriteria>();
            this.exportHandlers = new List<IExportHandler>();
            this.partHandlers = new List<IPartHandler>();
        }
        
        /// <summary>
        /// Gets a collection of the catalog wide interceptors.
        /// </summary>
        /// <remarks>
        /// All parts inside <see cref="InterceptingCatalog"/> will be intercepted
        /// using this interceptors in order in which they were added.
        /// </remarks>
        public IEnumerable<IExportedValueInterceptor> Interceptors
        {
            get { return this.interceptors; }
        }

        /// <summary>
        /// Gets a collection of <see cref="IPartInterceptionCriteria"/> instances.
        /// </summary>
        public IEnumerable<IPartInterceptionCriteria> InterceptionCriteria
        {
            get { return this.interceptionCriteria; }
        }

        /// <summary>
        /// Gets a collection of <see cref="IExportHandler"/> instances.
        /// </summary>
        public IEnumerable<IExportHandler> ExportHandlers
        {
            get { return this.exportHandlers; }
        }

        /// <summary>
        /// Gets a collection of <see cref="IPartHandler"/> instances.
        /// </summary>
        public IEnumerable<IPartHandler> PartHandlers
        {
            get { return this.partHandlers; }
        }

        /// <summary>
        /// Adds a catalog wide interceptor. If adding more than one catalog wide interceptor,
        /// it is wrapped in <see cref="CompositeValueInterceptor"/> instance.
        /// </summary>
        /// <param name="interceptor">Interceptor to be added.</param>
        /// <returns><see cref="InterceptionConfiguration"/> instance to enable fluent configuration.</returns>
        public InterceptionConfiguration AddInterceptor(IExportedValueInterceptor interceptor)
        {
            if (interceptor == null) throw new ArgumentNullException("interceptor");

            this.interceptors.Add(interceptor);
            return this;
        }

        /// <summary>
        /// Adds an <see cref="IExportHandler"/> instance.
        /// </summary>
        /// <param name="handler">Export handler to be added.</param>
        /// <returns><see cref="InterceptionConfiguration"/> instance to enable fluent configuration.</returns>
        public InterceptionConfiguration AddHandler(IExportHandler handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");

            this.exportHandlers.Add(handler);
            return this;
        }

        /// <summary>
        /// Adds an <see cref="IPartHandler"/> instance.
        /// </summary>
        /// <param name="handler">Part handler to be added.</param>
        /// <returns><see cref="InterceptionConfiguration"/> instance to enable fluent configuration.</returns>
        public InterceptionConfiguration AddHandler(IPartHandler handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");

            this.partHandlers.Add(handler);
            return this;
        }

        /// <summary>
        /// Adds <see cref="IPartInterceptionCriteria"/> instance.
        /// </summary>
        /// <param name="partInterceptionCriteria">Criteria to be added.</param>
        /// <returns><see cref="InterceptionConfiguration"/> instance to enable fluent configuration.</returns>
        public InterceptionConfiguration AddInterceptionCriteria(IPartInterceptionCriteria partInterceptionCriteria)
        {
            if (partInterceptionCriteria == null) throw new ArgumentNullException("partInterceptionCriteria");

            this.interceptionCriteria.Add(partInterceptionCriteria);
            return this;
        }
    }
}