namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Scans an <see cref="Assembly"/> for types that matches a provided predicate.
    /// </summary>
    public class AssemblyTypeScanner : ITypeScanner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyTypeScanner"/> class.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> instance that the scanner should use when looking for types that matches a predicate.</param>
        public AssemblyTypeScanner(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly", "The value of the assembly parameter cannot be null.");
            }

            this.Assembly = assembly;
        }

        /// <summary>
        /// Gets the <see cref="Assembly"/> instance that the <see cref="AssemblyTypeScanner"/> uses.
        /// </summary>
        /// <value>The <see cref="Assembly"/> instance that the <see cref="AssemblyTypeScanner"/> uses.</value>
        public Assembly Assembly { get; private set; }

        /// <summary>
        /// Retreives a collection of <see cref="Type"/> instances that matched the provided <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A <see cref="Predicate{T}"/> used to match the types to return.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing the matched types.</returns>
        public IEnumerable<Type> GetTypes(Predicate<Type> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            return this.Assembly.GetTypes().Where(x =>
                predicate(x) &&
                x.IsPublic &&
                !x.IsAbstract &&
                x.IsClass);
        }
    }
}