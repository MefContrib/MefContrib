namespace MefContrib.Web.Mvc
{
    using System;
    using System.ComponentModel.Composition;

    /// <summary>
    /// PartCreationScope
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PartCreationScopeAttribute
        : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartCreationScopeAttribute"/> class.
        /// </summary>
        /// <param name="scope">The part's scope.</param>
        public PartCreationScopeAttribute(PartCreationScope scope)
        {
            Scope = scope;
        }

        /// <summary>
        /// Gets the part's scope.
        /// </summary>
        /// <value>
        /// The part's scope.
        /// </value>
        public PartCreationScope Scope { get; set; }
    }
}
