namespace MefContrib.Hosting.Interception
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Primitives;

    public class PartHandlerChangedEventArgs : EventArgs
    {
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

        public IEnumerable<ComposablePartDefinition> AddedDefinitions { get; private set; }

        public IEnumerable<ComposablePartDefinition> RemovedDefinitions { get; private set; }
    }
}