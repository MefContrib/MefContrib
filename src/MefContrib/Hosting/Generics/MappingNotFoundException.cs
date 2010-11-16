namespace MefContrib.Hosting.Generics
{
    using System;

    /// <summary>
    /// Exception which identifies situation where open-generics mapping is not
    /// found for a given type.
    /// </summary>
    public class MappingNotFoundException : Exception
    {
        /// <summary>
        /// Gets the type for which the mapping was not found.
        /// </summary>
        public Type Type { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingNotFoundException"/> class.
        /// </summary>
        /// <param name="type">The type for which the mapping was not found.</param>
        /// <param name="message">The message.</param>
        public MappingNotFoundException(Type type, string message) : base(message)
        {
            this.Type = type;
        }
    }
}