namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// 
    /// </summary>
    public interface ITypeScannerConfigurator
    {
        /// <summary>
        /// Adds the <see cref="Assembly(System.Reflection.Assembly)"/> specified by the <paramref name="assembly"/> parameter.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly(System.Reflection.Assembly)"/> that should be added.</param>
        /// <returns>A reference to the current <see cref="ITypeScannerConfigurator"/> instance.</returns>
        ITypeScannerConfigurator Assembly(Assembly assembly);

        /// <summary>
        /// Adds the <see cref="Assembly(System.Reflection.Assembly)"/> specified by the <paramref name="path"/> parameter.
        /// </summary>
        /// <param name="path">A <see cref="string"/> containing the path to the <see cref="Assembly(System.Reflection.Assembly)"/> that should be added.</param>
        /// <returns>A reference to the current <see cref="ITypeScannerConfigurator"/> instance.</returns>
        ITypeScannerConfigurator Assembly(string path);

        /// <summary>
        /// Adds the <see cref="Assembly(System.Reflection.Assembly)"/> in the current <see cref="AppDomain"/> that matches the
        /// condition specified by the <paramref name="condition"/> parameter.
        /// </summary>
        /// <param name="condition">A condition that an <see cref="Assembly(System.Reflection.Assembly)"/> has to meet in order to be added.</param>
        /// <returns>A reference to the current <see cref="ITypeScannerConfigurator"/> instance.</returns>
        ITypeScannerConfigurator Assembly(Func<Assembly, bool> condition);

        /// <summary>
        /// Adds the <see cref="Assembly(System.Reflection.Assembly)"/> instances that can be found in the directory that is
        /// specified by the <paramref name="path"/> parameter.
        /// </summary>
        /// <param name="path">A <see cref="string"/> containing the path of the directory that should be inspected for <see cref="Assembly(System.Reflection.Assembly)"/> instances to add.</param>
        /// <returns>A reference to the current <see cref="ITypeScannerConfigurator"/> instance.</returns>
        ITypeScannerConfigurator Directory(string path);

        /// <summary>
        /// Adds the types specified by the <paramref name="types"/> parameter.
        /// </summary>
        /// <param name="types">An <see cref="IEnumerable{T}"/> of <see cref="Type"/> instances that should be added.</param>
        /// <returns>A reference to the current <see cref="ITypeScannerConfigurator"/> instance.</returns>
        ITypeScannerConfigurator Types(IEnumerable<Type> types);

        /// <summary>
        /// Adds the types that are returned by the function specified by the <paramref name="values"/> function.
        /// </summary>
        /// <param name="values">A function that returns an <see cref="IEnumerable{T}"/> of <see cref="Type"/> instances that should be added.</param>
        /// <returns>A reference to the current <see cref="ITypeScannerConfigurator"/> instance.</returns>
        ITypeScannerConfigurator Types(Func<IEnumerable<Type>> values);
    }

    public class TypeScannerConfigurator : ITypeScannerConfigurator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeScannerConfigurator"/> class.
        /// </summary>
        public TypeScannerConfigurator()
        {
            this.Scanner = new AggregatedTypeScanner();
        }

        private AggregatedTypeScanner Scanner { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scanner"></param>
        /// <returns></returns>
        public ITypeScannerConfigurator Add(ITypeScanner scanner)
        {
            if (scanner == null)
            {
                throw new ArgumentNullException("scanner", "The scanner argument cannot be null.");
            }

            this.Scanner.Add(scanner);

            return this;
        }

        /// <summary>
        /// Adds the <see cref="Assembly(System.Reflection.Assembly)"/> specified by the <paramref name="assembly"/> parameter.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly(System.Reflection.Assembly)"/> that should be added.</param>
        /// <returns>A reference to the current <see cref="ITypeScannerConfigurator"/> instance.</returns>
        public ITypeScannerConfigurator Assembly(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly", "The assembly argument cannot be null.");
            }

            this.Scanner.Add(
                new AssemblyTypeScanner(assembly));

            return this;
        }

        /// <summary>
        /// Adds the <see cref="Assembly(System.Reflection.Assembly)"/> specified by the <paramref name="path"/> parameter.
        /// </summary>
        /// <param name="path">A <see cref="string"/> containing the path to the <see cref="Assembly(System.Reflection.Assembly)"/> that should be added.</param>
        /// <returns>A reference to the current <see cref="ITypeScannerConfigurator"/> instance.</returns>
        public ITypeScannerConfigurator Assembly(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path", "The path argument cannot be null.");
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The file specified by the path argument could not be found");
            }

            var loadedAssembly =
                System.Reflection.Assembly.LoadFrom(path);

            this.Scanner.Add(
                new AssemblyTypeScanner(loadedAssembly));

            return this;
        }

        /// <summary>
        /// Adds the <see cref="Assembly(System.Reflection.Assembly)"/> in the current <see cref="AppDomain"/> that matches the
        /// condition specified by the <paramref name="condition"/> parameter.
        /// </summary>
        /// <param name="condition">A condition that an <see cref="Assembly(System.Reflection.Assembly)"/> has to meet in order to be added.</param>
        /// <returns>A reference to the current <see cref="ITypeScannerConfigurator"/> instance.</returns>
        public ITypeScannerConfigurator Assembly(Func<Assembly, bool> condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition", "The condition cannot be null.");
            }

            var matchingAssemblies =
                AppDomain.CurrentDomain.GetAssemblies().Where(condition);

            foreach (var matchingAssembly in matchingAssemblies)
            {
                this.Scanner.Add(new AssemblyTypeScanner(matchingAssembly));
            }

            return this;
        }

        /// <summary>
        /// Adds the <see cref="Assembly(System.Reflection.Assembly)"/> instances that can be found in the directory that is
        /// specified by the <paramref name="path"/> parameter.
        /// </summary>
        /// <param name="path">A <see cref="string"/> containing the path of the directory that should be inspected for <see cref="Assembly(System.Reflection.Assembly)"/> instances to add.</param>
        /// <returns>A reference to the current <see cref="ITypeScannerConfigurator"/> instance.</returns>
        public ITypeScannerConfigurator Directory(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path", "The path cannot be null.");
            }

            if (path.Length == 0)
            {
                throw new ArgumentOutOfRangeException("path", "The path argument cannot be empty.");
            }

            if (!System.IO.Directory.Exists(path))
            {
                throw new DirectoryNotFoundException("The directory specified by the path argument could not be found.");
            }

            var assemblies =
                System.IO.Directory.GetFiles(path, "*.dll");

            foreach (var assembly in assemblies)
            {
                this.Scanner.Add(
                    new AssemblyTypeScanner(System.Reflection.Assembly.LoadFrom(assembly)));
            }

            return this;
        }

        /// <summary>
        /// Adds the types specified by the <paramref name="types"/> parameter.
        /// </summary>
        /// <param name="types">An <see cref="IEnumerable{T}"/> of <see cref="Type"/> instances that should be added.</param>
        /// <returns>A reference to the current <see cref="ITypeScannerConfigurator"/> instance.</returns>
        public ITypeScannerConfigurator Types(IEnumerable<Type> types)
        {
            if (types == null)
            {
                throw new ArgumentNullException("types", "The types argument cannot be null.");
            }

            this.Scanner.Add(
                new TypeScanner(types));

            return this;
        }

        /// <summary>
        /// Adds the types that are returned by the function specified by the <paramref name="values"/> function.
        /// </summary>
        /// <param name="values">A function that returns an <see cref="IEnumerable{T}"/> of <see cref="Type"/> instances that should be added.</param>
        /// <returns>A reference to the current <see cref="ITypeScannerConfigurator"/> instance.</returns>
        public ITypeScannerConfigurator Types(Func<IEnumerable<Type>> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values", "The values argument cannot be null.");
            }

            this.Scanner.Add(
                new TypeScanner(values));

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public AggregatedTypeScanner GetTypeScanner()
        {
            return this.Scanner;
        }
    }
}