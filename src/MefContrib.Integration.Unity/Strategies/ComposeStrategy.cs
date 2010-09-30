namespace MefContrib.Integration.Unity.Strategies
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.Practices.ObjectBuilder2;

    /// <summary>
    /// Represents a strategy which injects MEF dependencies to
    /// the Unity created instance.
    /// </summary>
    public class ComposeStrategy : BuilderStrategy
    {
        public override void PostBuildUp(IBuilderContext context)
        {
            var type = context.Existing.GetType();
            var attributes = type.GetCustomAttributes(typeof(PartNotComposableAttribute), false);

            if (attributes.Length == 0)
            {
                var container = context.Policies.Get<ICompositionContainerPolicy>(null).Container;
                container.SatisfyImportsOnce(context.Existing);
            }
        }
    }
}