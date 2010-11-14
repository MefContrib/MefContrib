namespace MefContrib.Integration.Unity.Strategies
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Practices.ObjectBuilder2;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Build plan which enables true support for <see cref="Lazy{T}"/>.
    /// </summary>
    public class LazyResolveBuildPlanPolicy : IBuildPlanPolicy
    {
        /// <summary>
        /// Creates an instance of this build plan's type, or fills
        /// in the existing type if passed in.
        /// </summary>
        /// <param name="context">Context used to build up the object.</param>
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
            private readonly IUnityContainer container;
            private readonly string name;

            public ResolveTrampoline(IUnityContainer container, string name)
            {
                this.container = container;
                this.name = name;
            }

            public T Resolve()
            {
                return this.container.Resolve<T>(name);
            }
        }

        private class ResolveAllTrampoline<T>
        {
            private readonly IUnityContainer container;

            public ResolveAllTrampoline(IUnityContainer container)
            {
                this.container = container;
            }

            public IEnumerable<T> ResolveAll()
            {
                return this.container.ResolveAll<T>();
            }
        }
    }
}