namespace MefContrib.Hosting.Interception
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Primitives;

    /// <summary>
    /// Defines a composable part definition handler which can be used to filter
    /// parts or create them on the fly.
    /// </summary>
    public interface IPartHandler
    {
        /// <summary>
        /// Initializes the part handler.
        /// </summary>
        /// <param name="interceptedCatalog">The <see cref="ComposablePartCatalog"/> being intercepted.</param>
        void Initialize(ComposablePartCatalog interceptedCatalog);

        /// <summary>
        /// Method which can filter or produce <see cref="ComposablePartDefinition"/> instances.
        /// </summary>
        /// <param name="parts">An <see cref="IEnumerable{T}"/> of <see cref="ComposablePartDefinition"/> instances.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ComposablePartDefinition"/> instances.</returns>
        /// <remarks>This method is called at most once when the <see cref="InterceptingCatalog.Parts"/>
        /// property is being calculated.</remarks>
        IEnumerable<ComposablePartDefinition> GetParts(IEnumerable<ComposablePartDefinition> parts);

        /// <summary>
        /// Occurs when <see cref="IPartHandler"/> is changed.
        /// </summary>
        /// <remarks>This event is used by the <see cref="InterceptingCatalog"/>.
        /// When the event is raised, the catalog is recomposed.</remarks>
        event EventHandler<PartHandlerChangedEventArgs> Changed;
    }
}