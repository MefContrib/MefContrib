using System;
using System.Linq;
using Castle.DynamicProxy;

namespace MefContrib.Hosting.Interception.Castle
{
    public class DynamicProxyInterceptor : IExportedValueInterceptor
    {
        private static readonly ProxyGenerator Generator = new ProxyGenerator();

        private readonly IInterceptor[] interceptors;

        public DynamicProxyInterceptor(params IInterceptor[] interceptors)
        {
            this.interceptors = interceptors;
        }

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
