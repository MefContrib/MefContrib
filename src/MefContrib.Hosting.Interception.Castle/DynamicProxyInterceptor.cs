namespace MefContrib.Hosting.Interception.Castle
{
    using System;
    using System.Linq;
    using global::Castle.DynamicProxy;

    /// <summary>
    /// Defines an interceptor which creates proxies using the Castle.DynamicProxy library.
    /// </summary>
    public class DynamicProxyInterceptor : IExportedValueInterceptor
    {
        private static readonly ProxyGenerator Generator = new ProxyGenerator();

        private readonly IInterceptor[] interceptors;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicProxyInterceptor"/> class.
        /// </summary>
        /// <param name="interceptors">An array of <see cref="IInterceptor"/> instances.</param>
        public DynamicProxyInterceptor(params IInterceptor[] interceptors)
        {
            this.interceptors = interceptors;
        }

        /// <summary>
        /// Intercepts an exported value.
        /// </summary>
        /// <param name="value">The value to be intercepted.</param>
        /// <returns>Intercepted value.</returns>
        public object Intercept(object value)
        {
            var interfaces = value.GetType().GetInterfaces();
            Type proxyInterface = interfaces.FirstOrDefault();
            Type[] additionalInterfaces = interfaces.Skip(1).ToArray();
            
            return Generator.CreateInterfaceProxyWithTargetInterface(
                proxyInterface,
                additionalInterfaces,
                value,
                this.interceptors);
        }
    }
}
