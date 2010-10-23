namespace MefContrib.Hosting.Interception
{
    using System;
    using System.Collections.Generic;

    public class CompositeValueInterceptor : IExportedValueInterceptor
    {
        private readonly List<IExportedValueInterceptor> interceptors;

        public CompositeValueInterceptor(params IExportedValueInterceptor[] interceptors)
        {
            this.interceptors = new List<IExportedValueInterceptor>();

            if (interceptors != null)
            {
                this.interceptors.AddRange(interceptors);
            }
        }

        public object Intercept(object value)
        {
            foreach (var interceptor in interceptors)
            {
                value = interceptor.Intercept(value);
            }

            return value;
        }

        public IList<IExportedValueInterceptor> Interceptors
        {
            get { return interceptors; }
        }
    }
}