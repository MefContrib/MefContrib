namespace MefContrib.Hosting.Interception.Handlers
{
    using System;
    using System.ComponentModel.Composition;

    [InheritedExport]
    public abstract class GenericContractTypeMapping
    {
        private readonly Type genericContractTypeDefinitionDefinition;
        private readonly Type genericImplementationType;

        protected GenericContractTypeMapping(Type genericContractTypeDefinition, Type genericImplementationTypeDefinition)
        {
            if (!genericImplementationTypeDefinition.IsGenericTypeDefinition)
                throw new ArgumentException("Implementation Type must be a generic definition.", "genericImplementationTypeDefinition");

            if (!genericContractTypeDefinition.IsGenericTypeDefinition)
                throw new ArgumentException("Contract Type must be a generic definition.", "genericImplementationTypeDefinition");

            this.genericContractTypeDefinitionDefinition = genericContractTypeDefinition;
            this.genericImplementationType = genericImplementationTypeDefinition;
        }

        public Type GenericContractTypeDefinition
        {
            get { return this.genericContractTypeDefinitionDefinition; }
        }
        
        public Type GenericImplementationTypeDefinition
        {
            get { return this.genericImplementationType; }
        }
    }
}
