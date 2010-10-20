namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.IO;
    using System.Reflection;

    public class AssemblyWrapper : IDisposable
    {
        public AssemblyWrapper(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly", "The assembly cannot be null");
            }

            this.Assembly = assembly;
        }

        public Assembly Assembly { get; private set; }

        public void Dispose()
        {
            try
            {
                File.Delete(this.Assembly.FullName);
            }
            catch
            {
            }
        }

        public static implicit operator Assembly(AssemblyWrapper wrapper)
        {
            return wrapper.Assembly;
        }

        public static implicit operator string(AssemblyWrapper wrapper)
        {
            return wrapper.Assembly.Location;
        }
    }
}