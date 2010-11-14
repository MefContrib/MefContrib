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
        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PostBuildUp method is called when the chain has finished the PreBuildUp
        /// phase and executes in reverse order from the PreBuildUp calls.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
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