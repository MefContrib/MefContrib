using System;
using System.ComponentModel.Composition;
using Microsoft.Practices.ObjectBuilder2;

namespace MefContrib.Integration.Unity.Strategies
{
    /// <summary>
    /// Represents a strategy which injects MEF dependencies to
    /// the Unity created instance.
    /// </summary>
    public class ComposeStrategy : BuilderStrategy
    {
        public override void PostBuildUp(IBuilderContext context)
        {
            Type type = context.Existing.GetType();
            object[] attributes = type.GetCustomAttributes(typeof(PartNotComposableAttribute), false);

            if (attributes.Length == 0)
            {
                var container = context.Policies.Get<ICompositionContainerPolicy>(null).Container;
                container.ComposeParts(context.Existing);
            }
        }
    }
}