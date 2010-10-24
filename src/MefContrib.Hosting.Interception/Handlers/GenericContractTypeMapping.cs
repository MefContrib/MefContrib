namespace MefContrib.Hosting.Interception.Handlers
{
    using System;
    using System.ComponentModel.Composition;

    [InheritedExport]
    public abstract class GenericContractTypeMapping
    {
        private readonly Type _genericContractTypeDefinitionDefinition;
        private readonly Type _genericImplementationType;

        protected GenericContractTypeMapping(Type genericContractTypeDefinition, Type genericImplementationTypeDefinition)
        {
            if (!genericImplementationTypeDefinition.IsGenericTypeDefinition)
                throw new ArgumentException("Implementation Type must be a generic definition", "genericImplementationTypeDefinition");

            if (!genericContractTypeDefinition.IsGenericTypeDefinition)
                throw new ArgumentException("Contract Type must be a generic definition", "genericImplementationTypeDefinition");

            _genericContractTypeDefinitionDefinition = genericContractTypeDefinition;
            _genericImplementationType = genericImplementationTypeDefinition;
        }

        public Type GenericContractTypeDefinition { get { return _genericContractTypeDefinitionDefinition; } }
        
        public Type GenericImplementationTypeDefinition { get { return _genericImplementationType; } }
    }
}
