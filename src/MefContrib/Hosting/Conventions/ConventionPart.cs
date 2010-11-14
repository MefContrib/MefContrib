namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.ComponentModel.Composition;

    /// <summary>
    /// A wrapper class that enables a type to be composed using conventions.
    /// </summary>
    /// <typeparam name="TCompose">The <see cref="Type"/> that should be composed.</typeparam>
    /// <remarks>This class is required to bridge the gap between the convention based composition and the attributed based composition.</remarks>
    public class ConventionPart<TCompose> : IPartImportsSatisfiedNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionPart{T}"/> class.
        /// </summary>
        public ConventionPart()
        {
        }

        /// <summary>
        /// Gets or sets the types that was composed.
        /// </summary>
        /// <value>An array of composed instances of the type specified by <typeparam name="TCompose"/>.</value>
        [ImportMany]
        public TCompose[] Imports { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has been satisfied.
        /// </summary>
        /// <value><see langword="true"/> if this instance has been satisfied; otherwise, <see langword="false"/>.</value>
        public bool HasBeenSatisfied { get; private set; }

        /// <summary>
        /// Called when <see cref="Imports"/> has been satisfied.
        /// </summary>
        public void OnImportsSatisfied()
        {
            this.HasBeenSatisfied = true;
        }
    }
}