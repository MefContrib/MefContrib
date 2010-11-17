namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using MefContrib.Hosting.Conventions.Configuration;
    using MefContrib.Hosting.Interception;
    using MefContrib.Hosting.Interception.Configuration;

    /// <summary>
    /// Defines the class for composable part catalogs, which produce and return <see cref="ComposablePartDefinition"/> objects based on conventions.
    /// </summary>
    public class ConventionCatalog : ComposablePartCatalog
    {
        private readonly InterceptingCatalog interceptingCatalog;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionCatalog"/> class.
        /// </summary>
        /// <param name="locator">The locator.</param>
        public ConventionCatalog(IPartRegistryLocator locator)
            : this (locator.GetRegistries().ToArray())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionCatalog"/> class, using the provided array part registries.
        /// </summary>
        /// <param name="registries">An array of <see cref="IPartRegistry{T}"/> instance.</param>
        public ConventionCatalog(params IPartRegistry<IContractService>[] registries)
        {
            if (registries == null)
            {
                throw new ArgumentNullException("registries", "The registries parameter cannot be null.");
            }

            var cfg = new InterceptionConfiguration()
                .AddHandler(new ConventionPartHandler(registries));
            this.interceptingCatalog = new InterceptingCatalog(new EmptyCatalog(), cfg);
        }

        /// <summary>
        /// Gets the part definitions of the catalog.
        /// </summary>
        /// <value>A <see cref="IQueryable{T}"/> of <see cref="ComposablePartDefinition"/> objects of the <see cref="ConventionCatalog"/>.</value>
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get { return this.interceptingCatalog.Parts; }
        }

        /// <summary>
        /// Method which can filter exports for given <see cref="ImportDefinition"/> or produce new exports.
        /// </summary>
        /// <param name="definition"><see cref="ImportDefinition"/> instance.</param>
        /// <returns>
        /// A collection of <see cref="ExportDefinition"/>
        /// instances along with their <see cref="ComposablePartDefinition"/> instances which match given <see cref="ImportDefinition"/>.
        /// </returns>
        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            return (from part in this.Parts
                    from export in part.ExportDefinitions
                    where definition.IsConstraintSatisfiedBy(export)
                    select new Tuple<ComposablePartDefinition, ExportDefinition>(part, export)).ToList();
        }
    }
}