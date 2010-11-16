namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using MefContrib.Hosting.Conventions.Configuration;
    using MefContrib.Hosting.Interception;

    public class ConventionsExportHandler : IExportHandler
    {
        private readonly IEnumerable<IPartRegistry<IContractService>> registries;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionCatalog"/> class, using the provided array part registries.
        /// </summary>
        /// <param name="registries">An array of <see cref="IPartRegistry{T}"/> instance.</param>
        public ConventionsExportHandler(params IPartRegistry<IContractService>[] registries)
        {
            if (registries == null)
            {
                throw new ArgumentNullException("registries", "The registries parameter cannot be null.");
            }

            this.registries = registries;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionCatalog"/> class.
        /// </summary>
        /// <param name="locator">The locator.</param>
        public ConventionsExportHandler(IPartRegistryLocator locator)
        {
            if (locator == null)
            {
                throw new ArgumentNullException("locator", "The locator parameter cannot be null.");
            }

            this.registries = locator.GetRegistries();
        }

        /// <summary>
        /// Method which can filter exports for given <see cref="ImportDefinition"/> or produce new exports.
        /// </summary>
        /// <param name="definition"><see cref="ImportDefinition"/> instance.</param>
        /// <param name="exports">A collection of <see cref="ExportDefinition"/>
        /// instances along with their <see cref="ComposablePartDefinition"/> instances which match given <see cref="ImportDefinition"/>.</param>
        /// <returns>
        /// A collection of <see cref="ExportDefinition"/>
        /// instances along with their <see cref="ComposablePartDefinition"/> instances which match given <see cref="ImportDefinition"/>.
        /// </returns>
        public IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition, IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> exports)
        {
            var values = new List<Tuple<ComposablePartDefinition, ExportDefinition>>();

            foreach (var part in this.CreateParts())
            {
                foreach (var export in part.ExportDefinitions)
                {
                    if (definition.IsConstraintSatisfiedBy(export))
                    {
                        values.Add(new Tuple<ComposablePartDefinition, ExportDefinition>(part, export));
                    }
                }
            }

            return values;
        }

        /// <summary>
        /// Initializes this export handler.
        /// </summary>
        /// <param name="interceptedCatalog">The <see cref="ComposablePartCatalog"/> being intercepted.</param>
        public void Initialize(ComposablePartCatalog interceptedCatalog)
        {
        }

        /// <summary>
        /// Creates <see cref="ComposablePartDefinition"/> instances from the <see cref="IPartConvention"/> and types.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/>, containing <see cref="ComposablePartDefinition"/> instances.</returns>
        private IEnumerable<ComposablePartDefinition> CreateParts()
        {
            var definitionsFromRegistries =
                this.registries.SelectMany(x =>
                    (new ConventionPartCreator(x)).CreateParts());

            return definitionsFromRegistries;
        }
    }
}