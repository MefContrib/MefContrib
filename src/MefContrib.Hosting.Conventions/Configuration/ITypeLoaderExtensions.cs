namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;
    
    public static class ITypeLoaderExtensions
    {
        public static void Configure(this ITypeLoader typeLoader, Action<ITypeLoaderConfigurer> closure)
        {
            throw new NotImplementedException("The configure extension method has not been implemented yet.");
        }
    }

    public interface ITypeLoaderConfigurer
    {
        void AddExecutingAssembly();
        void AddAssembly(string fileName);
    }
}