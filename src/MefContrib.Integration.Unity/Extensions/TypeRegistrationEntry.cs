using System;

namespace MefContrib.Integration.Unity.Extensions
{
    /// <summary>
    /// Represents an entry for <see cref="TypeRegistrationTrackerExtension.Entries"/> collection.
    /// </summary>
    public class TypeRegistrationEntry
    {
        /// <summary>
        /// Initializes a new instance of of the <see cref="TypeRegistrationEntry"/> class.
        /// </summary>
        /// <param name="type">Type this entry represents.</param>
        /// <param name="name">Optional name.</param>
        public TypeRegistrationEntry(Type type, string name)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            Type = type;
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of of the <see cref="TypeRegistrationEntry"/> class.
        /// </summary>
        /// <param name="type">Type this entry represents.</param>
        public TypeRegistrationEntry(Type type) : this(type, null)
        {
        }

        /// <summary>
        /// Gets a type registered in the container.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets an optional name.
        /// </summary>
        public string Name { get; private set; }
    }
}