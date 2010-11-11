namespace MefContrib.Hosting.Interception.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public abstract class GenericContractRegistry : IGenericContractRegistry
    {
        private readonly List<GenericContractTypeMapping> mappings;

        private int isInitialized;

        protected GenericContractRegistry()
        {
            this.mappings = new List<GenericContractTypeMapping>();
        }

        protected void Register(Type genericContractTypeDefinition, Type genericImplementationTypeDefinition)
        {
            if (genericContractTypeDefinition == null)
                throw new ArgumentNullException("genericContractTypeDefinition");

            if (genericImplementationTypeDefinition == null)
                throw new ArgumentNullException("genericImplementationTypeDefinition");

            this.mappings.Add(new GenericContractTypeMapping(
                genericContractTypeDefinition, genericImplementationTypeDefinition));
        }

        protected abstract void Initialize();

        public IEnumerable<GenericContractTypeMapping> GetMappings()
        {
            if (Interlocked.CompareExchange(ref this.isInitialized, 1, 0) == 0)
            {
                Initialize();
            }

            return this.mappings.AsReadOnly();
        }
    }
}