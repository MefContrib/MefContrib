namespace MefContrib.Hosting.Generics
{
    using System;
    
    /// <summary>
    /// Defines open generics type mapping.
    /// </summary>
    public sealed class GenericContractTypeMapping
    {
        private readonly Type genericContractTypeDefinitionDefinition;
        private readonly Type genericImplementationType;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericContractTypeMapping"/> class.
        /// </summary>
        /// <param name="genericContractTypeDefinition">Open generics contract type.</param>
        /// <param name="genericImplementationTypeDefinition">Open generics implementation type.</param>
        public GenericContractTypeMapping(
            Type genericContractTypeDefinition,
            Type genericImplementationTypeDefinition)
        {
            if (genericContractTypeDefinition == null)
                throw new ArgumentNullException("genericContractTypeDefinition");

            if (genericImplementationTypeDefinition == null)
                throw new ArgumentNullException("genericImplementationTypeDefinition");

            if (!genericContractTypeDefinition.IsGenericTypeDefinition)
                throw new ArgumentException("Contract Type must be a generic definition.", "genericImplementationTypeDefinition");

            if (!genericImplementationTypeDefinition.IsGenericTypeDefinition)
                throw new ArgumentException("Implementation Type must be a generic definition.", "genericImplementationTypeDefinition");
            
            this.genericContractTypeDefinitionDefinition = genericContractTypeDefinition;
            this.genericImplementationType = genericImplementationTypeDefinition;
        }

        /// <summary>
        /// Gets the open generics contract type.
        /// </summary>
        public Type GenericContractTypeDefinition
        {
            get { return this.genericContractTypeDefinitionDefinition; }
        }
        
        /// <summary>
        /// Gets the open generics implementation type.
        /// </summary>
        public Type GenericImplementationTypeDefinition
        {
            get { return this.genericImplementationType; }
        }
    }
}
