namespace MefContrib.Hosting.Interception.LinFu
{
    using System.Linq;
    using global::LinFu.DynamicProxy;

    /// <summary>
    /// Defines an interceptor which creates proxies using the Castle.DynamicProxy library.
    /// </summary>
    public class DynamicProxyInterceptor : IExportedValueInterceptor
    {
        private readonly IInterceptor _interceptor;
        private static readonly ProxyFactory Generator = new ProxyFactory();

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicProxyInterceptor"/> class.
        /// </summary>
        /// <param name="interceptor">An <see cref="IInterceptor"/> instance.</param>
        public DynamicProxyInterceptor(IInterceptor interceptor)
        {
            _interceptor = interceptor;
        }

        /// <summary>
        /// Intercepts an exported value.
        /// </summary>
        /// <param name="value">The value to be intercepted.</param>
        /// <returns>Intercepted value.</returns>
        public object Intercept(object value)
        {
            var interfaces = value.GetType().GetInterfaces();
            var proxyInterface = interfaces.FirstOrDefault();
            var additionalInterfaces = interfaces.Skip(1).ToArray();
            
            return Generator.CreateProxy(proxyInterface, _interceptor, additionalInterfaces);
        }
    }
}
