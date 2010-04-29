namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the functionality of a class that stores types retreived from functions.
    /// </summary>
    public interface ITypeLoader
    {
        /// <summary>
        /// Adds the types, returned by the function, to the type loader.
        /// </summary>
        /// <param name="typeValueGetter">The type value getter function.</param>
        void AddTypes(Func<IEnumerable<Type>> typeValueGetter);

        /// <summary>
        /// Retreives a collection of <see cref="Type"/> instances that matched the provided <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A <see cref="Predicate{T}"/> used to match the types to return.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing the matched types.</returns>
        IEnumerable<Type> GetTypes(Predicate<Type> predicate);
    }
}