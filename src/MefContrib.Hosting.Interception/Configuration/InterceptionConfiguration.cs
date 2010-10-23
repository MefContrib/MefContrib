namespace MefContrib.Hosting.Interception.Configuration
{
    using System;
    using System.Collections.Generic;

    public class InterceptionConfiguration : IInterceptionConfiguration
    {
        private readonly List<IPartInterceptor> partInterceptors;
        private readonly List<IExportHandler> handlers;

        public InterceptionConfiguration()
        {
            this.partInterceptors = new List<IPartInterceptor>();
            this.handlers = new List<IExportHandler>();
        }

        public IExportedValueInterceptor Interceptor { get; set; }

        public IEnumerable<IPartInterceptor> PartInterceptors
        {
            get { return this.partInterceptors; }
        }

        public IEnumerable<IExportHandler> Handlers
        {
            get { return this.handlers; }
        }

        public InterceptionConfiguration AddInterceptor(IExportedValueInterceptor interceptor)
        {
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
                    this.Interceptor = new CompositeValueInterceptor(this.Interceptor);
                }
            }

            return this;
        }

        public InterceptionConfiguration AddHandler(IExportHandler handler)
        {
            this.handlers.Add(handler);
            return this;
        }

        public InterceptionConfiguration AddPartInterceptor(IPartInterceptor partInterceptor)
        {
            this.partInterceptors.Add(partInterceptor);
            return this;
        }
    }
}