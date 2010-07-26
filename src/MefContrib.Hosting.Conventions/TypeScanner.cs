namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Stores the types which will be available to the <see cref="ConventionCatalog"/> when creating parts from conventions.
    /// </summary>
    public class TypeScanner : ITypeScanner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeScanner"/> class.
        /// </summary>
        public TypeScanner()
            : this(Enumerable.Empty<Type>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeScanner"/> class.
        /// </summary>
        /// <param name="values">The types.</param>
        public TypeScanner(IEnumerable<Type> values)
            : this(() => values)
        {
        }

        public TypeScanner(Func<IEnumerable<Type>> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values", "The types argument cannot be null.");
            }

            var types =
                values.Invoke().ToList();

            this.Types = types;
        }

        /// <summary>
        /// Gets or sets the types that have been found.
        /// </summary>
        /// <value>A <see cref="List{T}"/> instance, containing the loaded types.</value>
        public List<Type> Types { get; private set; }

        /// <summary>
        /// Adds the types, returned by the function, to the type scanner.
        /// </summary>
        /// <param name="typeValueGetter">The type value getter function.</param>
        public void AddTypes(Func<IEnumerable<Type>> typeValueGetter)
        {
            if (typeValueGetter == null)
            {
                throw new ArgumentNullException("typeValueGetter", "The type value getter cannot be null.");
            }

            var typesToAdd =
                typeValueGetter.Invoke();

            if (typesToAdd == null)
            {
                throw new ArgumentNullException("typeValueGetter", "The value getter cannot return null.");
            }

            this.Types.AddRange(typesToAdd);
        }

        /// <summary>
        /// Retreives a collection of <see cref="Type"/> instances that matched the provided <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A <see cref="Predicate{T}"/> used to match the types to return.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing the matched types.</returns>
        public IEnumerable<Type> GetTypes(Predicate<Type> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate", "The predicate cannot be null.");
            }

            return this.Types.Where(t => predicate(t)).ToList();
        }
    }
}