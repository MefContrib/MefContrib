namespace MefContrib.Hosting.Interception.Configuration
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the functionality of the interception configuration.
    /// </summary>
    public interface IInterceptionConfiguration
    {
        /// <summary>
        /// Gets a collection of the catalog wide interceptors.
        /// </summary>
        /// <remarks>
        /// All parts inside <see cref="InterceptingCatalog"/> will be intercepted
        /// using this interceptors in order in which they were added.
        /// </remarks>
        IEnumerable<IExportedValueInterceptor> Interceptors { get; }

        /// <summary>
        /// Gets a collection of <see cref="IPartInterceptionCriteria"/> instances.
        /// </summary>
        IEnumerable<IPartInterceptionCriteria> InterceptionCriteria { get; }

        /// <summary>
        /// Gets a collection of <see cref="IExportHandler"/> instances.
        /// </summary>
        IEnumerable<IExportHandler> Handlers { get; }
    }
}