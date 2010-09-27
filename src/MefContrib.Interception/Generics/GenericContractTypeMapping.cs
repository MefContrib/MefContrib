using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace MefContrib.Interception.Generics
{
    [InheritedExport]
    public abstract class GenericContractTypeMapping
    {
        private Type _genericContractTypeDefinitionDefinition = null;
        private Type _genericImplementationType = null;

        public GenericContractTypeMapping(Type genericContractTypeDefinition, Type genericImplementationTypeDefinition)
        {
            if (!genericImplementationTypeDefinition.IsGenericTypeDefinition)
                throw new ArgumentException("Implementation Type must be a generic definition", "genericImplementationType");

            if (!genericContractTypeDefinition.IsGenericTypeDefinition)
                throw new ArgumentException("Contract Type must be a generic definition", "genericContractType");

            _genericContractTypeDefinitionDefinition = genericContractTypeDefinition;
            _genericImplementationType = genericImplementationTypeDefinition;
        }

        public Type GenericContractTypeDefinition { get { return _genericContractTypeDefinitionDefinition; } }
        public Type GenericImplementationTypeDefinition { get { return _genericImplementationType; } }
    }
}
