namespace MefContrib.Hosting.Generics
{
    using System;

    public class MappingNotFoundException : Exception
    {
        public Type Type { get; private set; }

        public MappingNotFoundException(Type type, string message) : base(message)
        {
            this.Type = type;
        }
    }
}