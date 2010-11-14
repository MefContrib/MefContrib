namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A type scanner that contains a collection of <see cref="ITypeScanner"/> instances that are
    /// queries for types when the <see cref="GetTypes"/> method is invoked.
    /// </summary>
    public class AggregatedTypeScanner : ITypeScanner
    {
        private readonly object instanceLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatedTypeScanner"/> class.
        /// </summary>
        /// <param name="scanners">An array of <see cref="ITypeScanner"/> instances, that should be queried by the type scanner.</param>
        public AggregatedTypeScanner(params ITypeScanner[] scanners)
            : this(scanners.ToList())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatedTypeScanner"/> class.
        /// </summary>
        /// <param name="scanners">An <see cref="IEnumerable{T}"/> of <see cref="ITypeScanner"/> instances, that should be queried by the type scanner.</param>
        public AggregatedTypeScanner(IEnumerable<ITypeScanner> scanners)
        {
            if (scanners == null)
            {
                throw new ArgumentNullException("scanners", "The list of scanners cannot be null.");
            }

            this.Scanners = scanners.ToList();
        }

        /// <summary>
        /// Gets or sets an <see cref="IEnumerable{T}"/> of <see cref="ITypeScanner"/> instances that are queried by the scanner.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> of <see cref="ITypeScanner"/> instances that are queried by the scanner.</value>
        private IList<ITypeScanner> Scanners { get; set; }

        /// <summary>
        /// Adds an <see cref="ITypeScanner"/> instance to the aggregate scanner.
        /// </summary>
        /// <param name="scanner">The <see cref="ITypeScanner"/> instance to add to the aggregate scanner.</param>
        /// <exception cref="ArgumentNullException">The value of the <paramref name="scanner"/> parameter cannot be <see langword="null"/>.</exception>
        public void Add(ITypeScanner scanner)
        {
            if (scanner == null)
            {
                throw new ArgumentNullException("scanner", "The value of the scanner parameter cannot be null.");
            }

            lock(this.instanceLock)
            {
                this.Scanners.Add(scanner);
            }
        }

        /// <summary>
        /// Retreives a collection of <see cref="Type"/> instances that matched the provided <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A <see cref="Predicate{T}"/> used to match the types to return.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing the matched types.</returns>
        public IEnumerable<Type> GetTypes(Predicate<Type> predicate)
        {
            lock(this.instanceLock)
            {
                var results = new List<Type>();

                foreach (var types in this.Scanners
                    .Select(scanner => scanner.GetTypes(predicate))
                    .Where(types => types != null))
                {
                    results.AddRange(types);
                }

                return results.Distinct();
            }
        }
    }
}