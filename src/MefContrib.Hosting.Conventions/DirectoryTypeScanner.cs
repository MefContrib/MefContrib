namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Scans an directory for types that matches a provided predicate
    /// </summary>
    public class DirectoryTypeScanner : ITypeScanner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryTypeScanner"/> class.
        /// </summary>
        /// <param name="path">A <see cref="string"/> containing the path from where the assemblies should be loaded.</param>
        /// <exception cref="ArgumentNullException">The value provided to the <paramref name="path"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The value provided to the <paramref name="path"/> was empty.</exception>
        /// <exception cref="DirectoryNotFoundException">The directory specified by the <paramref name="path"/> paramter could not be found.</exception>
        public DirectoryTypeScanner(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path", "The value for the path cannot be null.");
            }

            if (path.Length == 0)
            {
                throw new ArgumentOutOfRangeException("path", "The lenght of the path cannot be empty.");
            }

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException(string.Format("Could not locate directory {0}", path));
            }

            this.Path = path;
        }

        /// <summary>
        /// Gets or sets an <see cref="IEnumerable{T}"/> of <see cref="AssemblyTypeScanner"/> instances.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> of <see cref="AssemblyTypeScanner"/> instances.</value>
        private IEnumerable<AssemblyTypeScanner> AssemblyScanners { get; set; }

        /// <summary>
        /// Gets the path where types should be loaded from.
        /// </summary>
        /// <value>A <see cref="string"/> containing the path of the folder where types are loaded from.</value>
        public string Path { get; private set; }

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> of <see cref="AssemblyScanners"/> for the assemblies that
        /// is located in the folder specified by the <see cref="Path"/> property.
        /// </summary>
        private IEnumerable<AssemblyTypeScanner> GetAssemblyScanners()
        {
            return from file in Directory.GetFiles(this.Path, "*.dll")
                select new AssemblyTypeScanner(Assembly.LoadFrom(file));
        }

        /// <summary>
        /// Retreives a collection of <see cref="Type"/> instances that matched the provided <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A <see cref="Predicate{T}"/> used to match the types to return.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing the matched types.</returns>
        public IEnumerable<Type> GetTypes(Predicate<Type> predicate)
        {
            if (this.AssemblyScanners == null)
            {
                this.AssemblyScanners = this.GetAssemblyScanners();
            }

            var types =
                this.AssemblyScanners.SelectMany(x => x.GetTypes(predicate));

            return types;
        }
    }
}