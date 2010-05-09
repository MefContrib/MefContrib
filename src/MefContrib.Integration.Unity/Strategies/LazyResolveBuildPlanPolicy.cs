using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace MefContrib.Integration.Unity.Strategies
{
    /// <summary>
    /// Build plan which enables true support for <see cref="Lazy{T}"/>.
    /// </summary>
    public class LazyResolveBuildPlanPolicy : IBuildPlanPolicy
    {
        public void BuildUp(IBuilderContext context)
        {
            if (context.Existing == null)
            {
                var currentContainer = context.NewBuildUp<IUnityContainer>();
                var typeToBuild = GetTypeToBuild(context.BuildKey.Type);
                var nameToBuild = context.BuildKey.Name;

                context.Existing = IsResolvingIEnumerable(typeToBuild) ?
                    CreateResolveAllResolver(currentContainer, typeToBuild) :
                    CreateResolver(currentContainer, typeToBuild, nameToBuild);

                DynamicMethodConstructorStrategy.SetPerBuildSingleton(context);
            }
        }

        private static Type GetTypeToBuild(Type type)
        {
            return type.GetGenericArguments()[0];
        }

        private static bool IsResolvingIEnumerable(Type type)
        {
            return type.IsGenericType &&
                   type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        private static object CreateResolver(IUnityContainer currentContainer, Type typeToBuild,
            string nameToBuild)
        {
            Type lazyType = typeof(Lazy<>).MakeGenericType(typeToBuild);
            Type trampolineType = typeof(ResolveTrampoline<>).MakeGenericType(typeToBuild);
            Type delegateType = typeof(Func<>).MakeGenericType(typeToBuild);
            MethodInfo resolveMethod = trampolineType.GetMethod("Resolve");

            object trampoline = Activator.CreateInstance(trampolineType, currentContainer, nameToBuild);
            object trampolineDelegate = Delegate.CreateDelegate(delegateType, trampoline, resolveMethod);

            return Activator.CreateInstance(lazyType, trampolineDelegate);
        }

        private static object CreateResolveAllResolver(IUnityContainer currentContainer, Type enumerableType)
        {
            Type typeToBuild = GetTypeToBuild(enumerableType);
            Type lazyType = typeof(Lazy<>).MakeGenericType(enumerableType);
            Type trampolineType = typeof(ResolveAllTrampoline<>).MakeGenericType(typeToBuild);
            Type delegateType = typeof(Func<>).MakeGenericType(enumerableType);
            MethodInfo resolveAllMethod = trampolineType.GetMethod("ResolveAll");

            object trampoline = Activator.CreateInstance(trampolineType, currentContainer);
            object trampolineDelegate = Delegate.CreateDelegate(delegateType, trampoline, resolveAllMethod);

            return Activator.CreateInstance(lazyType, trampolineDelegate);
        }

        private class ResolveTrampoline<T>
        {
            private readonly IUnityContainer m_Container;
            private readonly string m_Name;

            public ResolveTrampoline(IUnityContainer container, string name)
            {
                m_Container = container;
                m_Name = name;
            }

            public T Resolve()
            {
                return m_Container.Resolve<T>(m_Name);
            }
        }

        private class ResolveAllTrampoline<T>
        {
            private readonly IUnityContainer m_Container;

            public ResolveAllTrampoline(IUnityContainer container)
            {
                m_Container = container;
            }

            public IEnumerable<T> ResolveAll()
            {
                return m_Container.ResolveAll<T>();
            }
        }
    }
}