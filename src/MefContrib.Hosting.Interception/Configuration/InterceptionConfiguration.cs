namespace MefContrib.Hosting.Interception.Configuration
{
    using System;
    using System.Collections.Generic;

    public class InterceptionConfiguration : IInterceptionConfiguration
    {
        private readonly List<IPartInterceptionCriteria> interceptionCriteria;
        private readonly List<IExportHandler> handlers;

        public InterceptionConfiguration()
        {
            this.interceptionCriteria = new List<IPartInterceptionCriteria>();
            this.handlers = new List<IExportHandler>();
        }

        public IExportedValueInterceptor Interceptor { get; set; }

        public IEnumerable<IPartInterceptionCriteria> InterceptionCriteria
        {
            get { return this.interceptionCriteria; }
        }

        public IEnumerable<IExportHandler> Handlers
        {
            get { return this.handlers; }
        }

        public InterceptionConfiguration AddInterceptor(IExportedValueInterceptor interceptor)
        {
            if (interceptor == null) throw new ArgumentNullException("interceptor");

            if (this.Interceptor == null)
            {
                this.Interceptor = interceptor;
            }
            else
            {
                var compositeValueInterceptor = this.Interceptor as CompositeValueInterceptor;
                if (compositeValueInterceptor != null)
                {
                    compositeValueInterceptor.Interceptors.Add(interceptor);
                }
                else
                {
                    this.Interceptor = new CompositeValueInterceptor(this.Interceptor, interceptor);
                }
            }

            return this;
        }

        public InterceptionConfiguration AddHandler(IExportHandler handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");

            this.handlers.Add(handler);
            return this;
        }

        public InterceptionConfiguration AddInterceptionCriteria(IPartInterceptionCriteria partInterceptionCriteria)
        {
            if (partInterceptionCriteria == null) throw new ArgumentNullException("partInterceptionCriteria");

            this.interceptionCriteria.Add(partInterceptionCriteria);
            return this;
        }
    }
}