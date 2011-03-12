namespace MefContrib.Hosting.Interception
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;

    /// <summary>
    /// Defines arguments for <see cref="IPartHandler.Changed"/> event.
    /// </summary>
    public class PartHandlerChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartHandlerChangedEventArgs"/> class.
        /// </summary>
        /// <param name="addedDefinitions">A collection of added <see cref="ComposablePartDefinition"/> instances.
        /// If no definitions were added, pass <see cref="Enumerable.Empty{ComposablePartDefinition}"/>.</param>
        /// <param name="removedDefinitions">A collection of removed <see cref="ComposablePartDefinition"/> instances.
        /// If no definitions were removed, pass <see cref="Enumerable.Empty{ComposablePartDefinition}"/>.</param>
        public PartHandlerChangedEventArgs(IEnumerable<ComposablePartDefinition> addedDefinitions, IEnumerable<ComposablePartDefinition> removedDefinitions)
        {
            if (addedDefinitions == null)
            {
                throw new ArgumentNullException("addedDefinitions");
            }

            if (removedDefinitions == null)
            {
                throw new ArgumentNullException("removedDefinitions");
            }

            AddedDefinitions = addedDefinitions;
            RemovedDefinitions = removedDefinitions;
        }

        /// <summary>
        /// Gets a collection of added definitions.
        /// </summary>
        public IEnumerable<ComposablePartDefinition> AddedDefinitions { get; private set; }

        /// <summary>
        /// Gets a collection of removed definitions.
        /// </summary>
        public IEnumerable<ComposablePartDefinition> RemovedDefinitions { get; private set; }
    }
}