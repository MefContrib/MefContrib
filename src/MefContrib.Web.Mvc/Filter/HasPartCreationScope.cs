using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using MefContrib.Hosting.Filter;

namespace MefContrib.Web.Mvc.Filter
{
    /// <summary>
    /// Represent a filter which inspects PartCreationScope.
    /// </summary>
    public class HasPartCreationScope
        : IFilter
    {
        private readonly PartCreationScope partCreationScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="HasPartCreationScope"/> class.
        /// </summary>
        /// <param name="partCreationScope">The part creation scope.</param>
        public HasPartCreationScope(PartCreationScope partCreationScope)
        {
            this.partCreationScope = partCreationScope;
        }

        /// <summary>
        /// Decides whether given part satisfies a filter.
        /// </summary>
        /// <param name="part"><see cref="ComposablePartDefinition"/> being filtered.</param>
        /// <returns>True if a given <see cref="ComposablePartDefinition"/> satisfies the filter.
        /// False otherwise.</returns>
        public bool Filter(ComposablePartDefinition part)
        {
            // Fetch all metadata
            Dictionary<string, object> metadata = new Dictionary<string, object>();
            foreach (var md in part.Metadata)
            {
                metadata.Add(md.Key, md.Value);
            }
            var additionalMetadata = from ed in part.ExportDefinitions
                                     from md in ed.Metadata
                                     select md;
            foreach (var md in additionalMetadata)
            {
                if (!metadata.ContainsKey(md.Key))
                {
                    metadata.Add(md.Key, md.Value);
                }
            }

            // Fetch "Scope"
            var key = "Scope";
            if (metadata.ContainsKey(key))
            {
                PartCreationScope scope = PartCreationScope.Default;
                Enum.TryParse(metadata[key].ToString(), out scope);

                return scope == partCreationScope;
            }
            else if (partCreationScope == PartCreationScope.Default)
            {
                return true;
            }
            return false;
        }
    }
}
