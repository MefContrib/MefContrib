namespace MefContrib.Hosting.Interception
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an interceptor which aggregates other <see cref="IExportedValueInterceptor"/> instances.
    /// </summary>
    public class CompositeValueInterceptor : IExportedValueInterceptor
    {
        private readonly List<IExportedValueInterceptor> interceptors;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValueInterceptor"/> class.
        /// </summary>
        /// <param name="interceptors">An array of <see cref="IExportedValueInterceptor"/> to be aggregated.</param>
        public CompositeValueInterceptor(params IExportedValueInterceptor[] interceptors)
        {
            this.interceptors = new List<IExportedValueInterceptor>();

            if (interceptors != null)
            {
                this.interceptors.AddRange(interceptors);
            }
        }

        /// <summary>
        /// Intercepts an exported value.
        /// </summary>
        /// <param name="value">The value to be intercepted.</param>
        /// <returns>Intercepted value.</returns>
        public object Intercept(object value)
        {
            foreach (var interceptor in interceptors)
            {
                value = interceptor.Intercept(value);
            }

            return value;
        }

        /// <summary>
        /// Gets a collection of <see cref="IExportedValueInterceptor"/> instances.
        /// </summary>
        public IList<IExportedValueInterceptor> Interceptors
        {
            get { return interceptors; }
        }
    }
}