namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using MefContrib.Hosting.Conventions.Configuration;

    /// <summary>
    /// Defines the class for composable part catalogs, which produce and return <see cref="ComposablePartDefinition"/> objects based on conventions.
    /// </summary>
    public class ConventionCatalog : ComposablePartCatalog
    {
        public ConventionCatalog()
        {
            var locator =
                new AppDomainPartRegistryLocator(AppDomain.CurrentDomain);

            this.Registries = locator.Locate();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionCatalog"/> class, using the provided array part registries.
        /// </summary>
        /// <param name="registries">An array of <see cref="IPartRegistry{T}"/> instance.</param>
        public ConventionCatalog(params IPartRegistry<IContractService>[] registries) 
            : this(registries.ToList())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionCatalog"/> class, using the providedenumerable of part registries.
        /// </summary>
        /// <param name="registries">An <see cref="IEnumerable{T}"/> instance, containing <see cref="IPartRegistry"/> instances.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="registries"/> parameter was <see langword="null" />.</exception>
        public ConventionCatalog(IEnumerable<IPartRegistry<IContractService>> registries)
        {
            if (registries == null)
            {
                throw new ArgumentNullException("registries", "The registries parameter cannot be null.");
            }

            this.Registries = registries;
        }

        public IEnumerable<IPartRegistry<IContractService>> Registries { get; private set; }

        /// <summary>
        /// Gets the part definitions of the catalog.
        /// </summary>
        /// <value>A <see cref="IQueryable{T}"/> of <see cref="ComposablePartDefinition"/> objects of the <see cref="ConventionCatalog"/>.</value>
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get { return this.CreateParts().AsQueryable(); }
        }
       
        /// <summary>
        /// Creates <see cref="ComposablePartDefinition"/> instances from the <see cref="IPartConvention"/> and types.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/>, containing <see cref="ComposablePartDefinition"/> instances.</returns>
        private IEnumerable<ComposablePartDefinition> CreateParts()
        {
            var definitionsFromRegistries = 
                this.Registries.SelectMany(x =>
                    (new ConventionPartCreator(x)).CreateParts());

            return definitionsFromRegistries;
        }
    }
}