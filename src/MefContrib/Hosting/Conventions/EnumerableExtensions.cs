namespace MefContrib.Hosting.Conventions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Contains extension methods for the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Converts an <see cref="IEnumerable{T}"/> of <see cref="MetadataItem"/> instances into a <see cref="Dictionary{TKey,TValue}"/>
        /// of string and object.
        /// </summary>
        /// <param name="metadata">The <see cref="IEnumerable{T}"/> of <see cref="MetadataItem"/></param> instances to convert.
        /// <returns>A <see cref="Dictionary{TKey,TValue}"/> of string and object, containing the converted metadata.</returns>
        public static Dictionary<string, object> ToMetadataDictionary(this IEnumerable<MetadataItem> metadata)
        {
            if (metadata == null)
                throw new ArgumentNullException("metadata");

            var metadataDictionary =
                new Dictionary<string, object>();

            foreach (var metadataItem in metadata)
            {
                metadataDictionary.Add(metadataItem.Name, metadataItem.Value);
            }

            return metadataDictionary;
        }
    }
}