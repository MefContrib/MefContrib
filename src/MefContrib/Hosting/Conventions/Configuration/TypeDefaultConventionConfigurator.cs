namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Creates <see cref="ITypeDefaultConvention"/> instances.
    /// </summary>
    public class TypeDefaultConventionConfigurator : ITypeDefaultConventionConfigurator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeDefaultConventionConfigurator"/> class.
        /// </summary>
        public TypeDefaultConventionConfigurator()
        {
            this.ConventionBuilders = new List<TypeDefaultConventionBuilder>();
        }

        /// <summary>
        /// Gets or sets the convention builders.
        /// </summary>
        /// <value>An <see cref="IList{T}"/> instance, containing <see cref="TypeDefaultConventionBuilder"/> instances.</value>
        private IList<TypeDefaultConventionBuilder> ConventionBuilders { get; set; }

        /// <summary>
        /// Creates a <see cref="ITypeDefaultConventionBuilder"/> instance for the <see cref="Type"/> specified by
        /// the <typeparamref name="T"/> type parameter.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> that the <see cref="ITypeDefaultConventionBuilder"/> should be created for.</typeparam>
        /// <returns>An <see cref="ITypeDefaultConventionBuilder"/> instance.</returns>
        public ITypeDefaultConventionBuilder ForType<T>()
        {
            var builder =
                new TypeDefaultConventionBuilder(typeof(T));

            this.ConventionBuilders.Add(builder);

            return builder;
        }

        /// <summary>
        /// Gets the <see cref="ITypeDefaultConvention"/> that was defined by the instance.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ITypeDefaultConvention"/> instances.</returns>
        public IEnumerable<ITypeDefaultConvention> GetDefaultConventions()
        {
            return this.ConventionBuilders.Select(x => x.Build()).Cast<ITypeDefaultConvention>();
        }
    }
}