namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using MefContrib.Hosting.Conventions.Configuration;
    using MefContrib.Hosting.Interception;

    public class ConventionPartHandler : IPartHandler
    {
        private readonly IEnumerable<IPartRegistry<IContractService>> registries;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionPartHandler"/> class, using the provided array part registries.
        /// </summary>
        /// <param name="registries">An array of <see cref="IPartRegistry{T}"/> instance.</param>
        public ConventionPartHandler(params IPartRegistry<IContractService>[] registries)
        {
            if (registries == null)
            {
                throw new ArgumentNullException("registries", "The registries parameter cannot be null.");
            }

            this.registries = registries;
        }

        /// <summary>
        /// Initializes this export handler.
        /// </summary>
        /// <param name="interceptedCatalog">The <see cref="ComposablePartCatalog"/> being intercepted.</param>
        public void Initialize(ComposablePartCatalog interceptedCatalog)
        {
        }

        public IEnumerable<ComposablePartDefinition> GetParts(IEnumerable<ComposablePartDefinition> parts)
        {
            return this.CreateParts();
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