namespace MefContrib.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;

    /// <summary>
    /// Represents a factory export provider.
    /// </summary>
    /// <remarks>
    /// This class can be used to build custom <see cref="ExportProvider"/> which
    /// provides exports from various data sources.
    /// </remarks>
    public class FactoryExportProvider : ExportProvider
    {
        private readonly Func<Type, string, object> factoryMethod;
        private readonly Dictionary<ContractBasedExportDefinition, Func<object>> definitions;

        /// <summary>
        /// Initializes a new instance of <see cref="FactoryExportProvider"/> class.
        /// </summary>
        public FactoryExportProvider()
        {
            this.definitions = new Dictionary<ContractBasedExportDefinition, Func<object>>();
            this.factoryMethod = (t, s) => { throw new InvalidOperationException("Global factory method has not been supplied."); };
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FactoryExportProvider"/> class.
        /// </summary>
        /// <param name="type"><see cref="Type"/> to be registered.</param>
        /// <param name="factory">Factory method.</param>
        public FactoryExportProvider(Type type, Func<object> factory)
            : this()
        {
            if (type == null) throw new ArgumentNullException("type");
            if (factory == null) throw new ArgumentNullException("factory");

            Register(type, factory);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FactoryExportProvider"/> class.
        /// </summary>
        /// <param name="type"><see cref="Type"/> to be registered.</param>
        /// <param name="registrationName">Registration name.</param>
        /// <param name="factory">Factory method.</param>
        public FactoryExportProvider(Type type, string registrationName, Func<object> factory)
            : this()
        {
            if (type == null) throw new ArgumentNullException("type");
            if (registrationName == null) throw new ArgumentNullException("registrationName");
            if (factory == null) throw new ArgumentNullException("factory");

            Register(type, registrationName, factory);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FactoryExportProvider"/> class.
        /// </summary>
        /// <param name="factoryMethod">Method that is called when an instance of specific type
        /// is requested, optionally with given registration name.</param>
        public FactoryExportProvider(Func<Type, string, object> factoryMethod)
        {
            if (factoryMethod == null)
                throw new ArgumentNullException("factoryMethod");

            this.definitions = new Dictionary<ContractBasedExportDefinition, Func<object>>();
            this.factoryMethod = factoryMethod;
        }

        protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
        {
            if (definition.Cardinality == ImportCardinality.ExactlyOne || definition.Cardinality == ImportCardinality.ZeroOrOne)
            {
                var constraint = definition.Constraint.Compile();
                return from exportDefinition in this.definitions
                       where constraint(exportDefinition.Key)
                       select new Export(exportDefinition.Key, exportDefinition.Value);
            }

            if (definition.ContractName != null)
            {
                return from exportDefinition in this.definitions
                       where AttributedModelServices.GetContractName(
                           exportDefinition.Key.ContractType) == definition.ContractName
                       select new Export(exportDefinition.Key, exportDefinition.Value);
            }

            return Enumerable.Empty<Export>();
        }

        /// <summary>
        /// Registers a new type.
        /// </summary>
        /// <param name="type">Type that is being exported.</param>
        /// <param name="factory">Optional factory method. If not supplied, the general
        /// factory method will be used.</param>
        /// <returns><see cref="FactoryExportProvider"/> instance for fluent registration.</returns>
        public FactoryExportProvider Register(Type type, Func<object> factory = null)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return Register(type, null, factory);
        }

        /// <summary>
        /// Registers a new type.
        /// </summary>
        /// <typeparam name="T">Type that is being exported.</typeparam>
        /// <param name="factory">Optional factory method. If not supplied, the general
        /// factory method will be used.</param>
        /// <returns><see cref="FactoryExportProvider"/> instance for fluent registration.</returns>
        public FactoryExportProvider Register<T>(Func<T> factory = null)
        {
            var factoryWrapper = (factory != null) ? () => factory() : (Func<object>)null;
            return Register(typeof(T), null, factoryWrapper);
        }

        /// <summary>
        /// Registers a new type as a singleton.
        /// </summary>
        /// <param name="type">Type that is being exported.</param>
        /// <param name="factory">Factory method.</param>
        /// <returns><see cref="FactoryExportProvider"/> instance for fluent registration.</returns>
        public FactoryExportProvider RegisterInstance(Type type, Func<object> factory)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (factory == null) throw new ArgumentNullException("factory");

            var lazy = new Lazy<object>(factory);
            return Register(type, null, () => lazy.Value);
        }

        /// <summary>
        /// Registers a new type as a singleton.
        /// </summary>
        /// <typeparam name="T">Type that is being exported.</typeparam>
        /// <param name="factory">Optional factory method. If not supplied, the general
        /// factory method will be used.</param>
        /// <returns><see cref="FactoryExportProvider"/> instance for fluent registration.</returns>
        public FactoryExportProvider RegisterInstance<T>(Func<T> factory)
        {
            if (factory == null) throw new ArgumentNullException("factory");

            var lazy = new Lazy<object>(() => factory());
            return Register(typeof(T), null, () => lazy.Value);
        }

        /// <summary>
        /// Registers a new type.
        /// </summary>
        /// <param name="type">Type that is being registered.</param>
        /// <param name="registrationName">Registration name under which <paramref name="type"/>
        /// is being registered.</param>
        /// <param name="factory">Optional factory method. If not supplied, the general
        /// factory method will be used.</param>
        /// <returns><see cref="FactoryExportProvider"/> instance for fluent registration.</returns>
        public FactoryExportProvider RegisterInstance(Type type, string registrationName, Func<object> factory)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (factory == null) throw new ArgumentNullException("factory");

            var lazy = new Lazy<object>(factory);
            return Register(type, registrationName, () => lazy.Value);
        }

        /// <summary>
        /// Registers a new type as a singleton.
        /// </summary>
        /// <typeparam name="T">Type that is being exported.</typeparam>
        /// <param name="registrationName">Registration name under which <typeparamref name="T"/>
        /// is being registered.</param>
        /// <param name="factory">Optional factory method. If not supplied, the general
        /// factory method will be used.</param>
        /// <returns><see cref="FactoryExportProvider"/> instance for fluent registration.</returns>
        public FactoryExportProvider RegisterInstance<T>(string registrationName, Func<T> factory)
        {
            if (factory == null) throw new ArgumentNullException("factory");

            var lazy = new Lazy<object>(() => factory());
            return Register(typeof(T), registrationName, () => lazy.Value);
        }

        /// <summary>
        /// Registers a new type.
        /// </summary>
        /// <typeparam name="T">Type that is being exported.</typeparam>
        /// <param name="registrationName">Registration name under which <typeparamref name="T"/>
        /// is being registered.</param>
        /// <param name="factory">Optional factory method. If not supplied, the general
        /// factory method will be used.</param>
        /// <returns><see cref="FactoryExportProvider"/> instance for fluent registration.</returns>
        public FactoryExportProvider Register<T>(string registrationName, Func<T> factory = null)
        {
            var factoryWrapper = (factory != null) ? () => factory() : (Func<object>) null;
            return Register(typeof(T), registrationName, factoryWrapper);
        }

        /// <summary>
        /// Registers a new type.
        /// </summary>
        /// <param name="type">Type that is being registered.</param>
        /// <param name="registrationName">Registration name under which <paramref name="type"/>
        /// is being registered.</param>
        /// <param name="factory">Optional factory method. If not supplied, the general
        /// factory method will be used.</param>
        /// <returns><see cref="FactoryExportProvider"/> instance for fluent registration.</returns>
        public FactoryExportProvider Register(Type type, string registrationName, Func<object> factory = null)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (factory == null)
            {
                factory = () => this.factoryMethod(type, registrationName);
            }

            var exportDefinitions = ReadOnlyDefinitions.Where(t => t.ContractType == type &&
                                                                   t.RegistrationName == registrationName);

            // We cannot add an export definition with the same type and registration name
            // since we will introduce cardinality problems
            if (exportDefinitions.Count() == 0)
            {
                this.definitions.Add(new ContractBasedExportDefinition(type, registrationName), factory);
            }
            
            return this;
        }

        /// <summary>
        /// Gets a read only list of definitions known to the export provider.
        /// </summary>
        public IEnumerable<ContractBasedExportDefinition> ReadOnlyDefinitions
        {
            get { return new ReadOnlyCollection<ContractBasedExportDefinition>(this.definitions.Keys.ToList()); }
        }
    }
}