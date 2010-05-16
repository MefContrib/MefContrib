using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace MefContrib.Hosting
{
    /// <summary>
    /// Represent a filter which inspects metadata.
    /// </summary>
    public class ContainsMetadata : IFilter
    {
        private readonly string m_Metadata;
        private readonly object m_Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainsMetadata"/> class.
        /// </summary>
        /// <param name="metadata">Metadata key.</param>
        /// <param name="value">Metadata value.</param>
        public ContainsMetadata(string metadata, object value)
        {
            if (metadata == null)
                throw new ArgumentNullException("metadata");

            m_Metadata = metadata;
            m_Value = value;
        }

        /// <summary>
        /// Decides whether given part satisfies a filter.
        /// </summary>
        /// <param name="part"><see cref="ComposablePartDefinition"/> being filtered.</param>
        /// <returns>True if a given <see cref="ComposablePartDefinition"/> satisfies the filter.
        /// False otherwise.</returns>
        public bool Filter(ComposablePartDefinition part)
        {
            return part.Metadata.ContainsKey(m_Metadata) &&
                   part.Metadata[m_Metadata].Equals(m_Value);
        }
    }

    /// <summary>
    /// Represents a filter which filters out composable parts based on a
    /// specified <see cref="CreationPolicy"/>.
    /// </summary>
    public class HasCreationPolicy : IFilter
    {
        private readonly CreationPolicy m_CreationPolicy;

        /// <summary>
        /// Initializes a new instance of the <see cref="HasCreationPolicy"/> class.
        /// </summary>
        /// <param name="creationPolicy">Creation policy.</param>
        public HasCreationPolicy(CreationPolicy creationPolicy)
        {
            m_CreationPolicy = creationPolicy;
        }

        /// <summary>
        /// Decides whether given part satisfies a filter.
        /// </summary>
        /// <param name="part"><see cref="ComposablePartDefinition"/> being filtered.</param>
        /// <returns>True if a given <see cref="ComposablePartDefinition"/> satisfies the filter.
        /// False otherwise.</returns>
        public bool Filter(ComposablePartDefinition part)
        {
            if (m_CreationPolicy == CreationPolicy.Any && !part.Metadata.ContainsKey(CompositionConstants.PartCreationPolicyMetadataName))
                return true;

            return part.Metadata.ContainsKey(CompositionConstants.PartCreationPolicyMetadataName) &&
                   ((CreationPolicy)part.Metadata[CompositionConstants.PartCreationPolicyMetadataName]) == m_CreationPolicy;
        }
    }
}