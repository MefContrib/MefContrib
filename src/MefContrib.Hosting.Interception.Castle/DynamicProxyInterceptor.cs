using System;
using System.Linq;
using Castle.DynamicProxy;

namespace MefContrib.Hosting.Interception.Castle
{
    public class DynamicProxyInterceptor : IExportedValueInterceptor
    {
        private static readonly ProxyGenerator Generator = new ProxyGenerator();

        private readonly IInterceptor[] _interceptors;

        public DynamicProxyInterceptor(params IInterceptor[] interceptors)
        {
            _interceptors = interceptors;
        }

        public object Intercept(object value)
        {
            var interfaces = value.GetType().GetInterfaces();
            Type proxyInterface = interfaces.FirstOrDefault();
            Type[] additionalInterfaces = interfaces.Skip(1).ToArray();
            var proxy = Generator.CreateInterfaceProxyWithTargetInterface(proxyInterface, additionalInterfaces, value, _interceptors);            
            return proxy;
        }
    }
}
