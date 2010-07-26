namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// A convention registry for types implementing the <see cref="IPartConvention"/> interface.
    /// </summary>
    public class PartRegistry :
        ExpressionBuilderFactory<IPartConvention>, IPartRegistry<DefaultConventionContractService>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartRegistry"/> class.
        /// </summary>
        public PartRegistry()
        {
            this.ContractService = new DefaultConventionContractService();
        }

        /// <summary>
        /// Gets or sets the contract service used by the registry.
        /// </summary>
        /// <value>An <see cref="DefaultConventionContractService"/> instance.</value>
        public DefaultConventionContractService ContractService { get; private set; }

        /// <summary>
        /// Gets or sets the type scanner used to create parts out of the conventions in the registry.
        /// </summary>
        /// <value>An <see cref="ITypeScanner"/> instance.</value>
        public ITypeScanner TypeScanner { get; set; }

        public void Scan(Action<ITypeScannerConfigurator> closure)
        {
            if (closure == null)
            {
                throw new ArgumentNullException("closure", "The closure cannot be null.");
            }
        }

        /// <summary>
        /// Gets the conventions registered in the registry.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IPartConvention"/> instances.</returns>
        public IEnumerable<IPartConvention> GetConventions()
        {
            return this.BuildConventions();
        }

        /// <summary>
        /// Creates a convention builder for <see cref="PartConvention"/> conventions.
        /// </summary>
        /// <returns>A <see cref="PartConventionBuilder{TPartConvention}"/> instance for the <see cref="PartConvention"/> type.</returns>
        public PartConventionBuilder<PartConvention> Part()
        {
            return this.CreateExpressionBuilder<PartConventionBuilder<PartConvention>>();
        }

        /// <summary>
        /// Create a convention builder for the <typeparamref name="TConvention"/> convention type.
        /// </summary>
        /// <typeparam name="TConvention">The type of a class which implements the <see cref="IPartConvention"/> interface.</typeparam>
        /// <returns>A <see cref="PartConventionBuilder{TPartConvention}"/> instance for the part convention type specified by the <typeparamref name="TConvention"/> type parameter.</returns>
        public PartConventionBuilder<TConvention> Part<TConvention>() where TConvention : IPartConvention, new()
        {
            return this.CreateExpressionBuilder<PartConventionBuilder<TConvention>>();
        }
    }

    public interface ITypeScannerConfigurator
    {
        ITypeScannerConfigurator Assembly(Assembly assembly);

        ITypeScannerConfigurator Assembly(string path);

        ITypeScannerConfigurator Assembly(Func<Assembly, bool> condition);

        ITypeScannerConfigurator Directory(string path);

        ITypeScannerConfigurator Types(IEnumerable<Type> types);

        ITypeScannerConfigurator Types(Func<IEnumerable<Type>> values);
    }

    public class TypeScannerConfigurator : ITypeScannerConfigurator
    {
        public TypeScannerConfigurator()
        {
            this.Scanner = new AggregatedTypeScanner();
        }

        private AggregatedTypeScanner Scanner { get; set; }

        public ITypeScannerConfigurator Assembly(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly", "The assembly cannot be null.");
            }

            this.Scanner.Add(
                new AssemblyTypeScanner(assembly));

            return this;
        }

        public ITypeScannerConfigurator Assembly(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path", "The path cannot be null.");
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

        public AggregatedTypeScanner GetTypeScanner()
        {
            return this.Scanner;
        }
    }
}