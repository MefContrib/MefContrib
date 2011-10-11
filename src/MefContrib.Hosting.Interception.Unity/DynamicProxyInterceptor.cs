namespace MefContrib.Hosting.Interception.Unity
{
    using System.Linq;
    using Microsoft.Practices.Unity.InterceptionExtension;

    /// <summary>
    /// Defines an interceptor which creates proxies using the Unity.Interception library.
    /// </summary>
    public class DynamicProxyInterceptor : IExportedValueInterceptor
    {
        private readonly IInstanceInterceptor interceptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicProxyInterceptor"/> class.
        /// </summary>
        /// <param name="interceptor">An <see cref="IInterceptor"/> instance.</param>
        public DynamicProxyInterceptor(IInstanceInterceptor interceptor)
        {
            this.interceptor = interceptor;
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

            CurrentInterceptionRequest request = new CurrentInterceptionRequest(interceptor, proxyInterface, value.GetType());
            InjectionPolicy[] policies = new InjectionPolicy[] { new AttributeDrivenPolicy() };
            PolicyInjectionBehavior behaviour = new PolicyInjectionBehavior(request, policies, null);

            var proxy = Microsoft.Practices.Unity.InterceptionExtension.Intercept.ThroughProxyWithAdditionalInterfaces(
                    proxyInterface,
                    value,
                    interceptor,
                    new[] { behaviour },
                    additionalInterfaces);

            return proxy;
        }
    }
}