using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using MefContrib.Interception;

namespace Composition.Interception.DynamicProxy
{
    public class DynamicProxyInterceptor : IExportedValueInterceptor
    {
        private readonly IInterceptor[] _interceptors;
        private static readonly ProxyGenerator _generator = new ProxyGenerator();

        public DynamicProxyInterceptor(params IInterceptor[] interceptors)
        {
            _interceptors = interceptors;
        }

        public object Intercept(object value)
        {
            ProxyGenerationOptions options = new ProxyGenerationOptions();

            var interfaces = value.GetType().GetInterfaces();
            Type proxyInterface = interfaces.FirstOrDefault();
            Type[] additionalInterfaces = interfaces.Skip(1).ToArray();
            var proxy = _generator.CreateInterfaceProxyWithTargetInterface(proxyInterface, additionalInterfaces, value, _interceptors);            
            return proxy;
        }
    }
}
