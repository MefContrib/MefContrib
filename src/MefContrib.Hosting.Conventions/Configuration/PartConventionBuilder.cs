namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    /// <summary>
    /// A convention builder for <see cref="IPartConvention"/> instances.
    /// </summary>
    /// <typeparam name="TConvention">The type of the part convention that will be built by the expression builder.</typeparam>
    public class PartConventionBuilder<TConvention> :
        ConventionBuilder<TConvention> where TConvention : IPartConvention, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartConventionBuilder{TConvention}"/> class.
        /// </summary>
        public PartConventionBuilder()
        {
        }

        /// <summary>
        /// Defines metadata, using property name and values extracted from an anonymous type, that will be added to the parts created by the convention.
        /// </summary>
        /// <param name="anonymousType">The anonymous type that the names and values will be extracted from.</param>
        /// <returns>Returns a reference to the same <see cref="PartConventionBuilder{TConvention}"/> instance as the method was called on.</returns>
        /// <exception cref="ArgumentNullException">The method was called with a null value.</exception>
        public PartConventionBuilder<TConvention> AddMetadata(object anonymousType)
        {
            if (anonymousType == null)
            {
                throw new ArgumentNullException("anonymousType", "The anonymous type cannot be null.");
            }

            var metadataItems =
                from property in anonymousType.GetType().GetProperties()
                select new MetadataItem(property.Name, property.GetValue(anonymousType, null));

            this.ProvideValueFor(x => x.Metadata, metadataItems);

            return this;
        }

        /// <summary>
        /// Defines metadata, using a name and value pair, that will be added to the parts created by the convention.
        /// </summary>
        /// <param name="name">The name of the metadata.</param>
        /// <param name="value">The value of the metadata.</param>
        /// <returns>Returns a reference to the same <see cref="PartConventionBuilder{TConvention}"/> instance as the method was called on.</returns>
        /// <exception cref="ArgumentNullException">The value of the <paramref name="name"/> or <paramref name="value"/> parameters were null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The lenght of the <paramref name="name"/> was zero.</exception>
        public PartConventionBuilder<TConvention> AddMetadata(string name, object value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name", "The name cannot be null.");
            }

            if (name.Length == 0)
            {
                throw new ArgumentOutOfRangeException("name", "The name cannot be empty.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value", "The value cannot be null.");
            }

            this.ProvideValueFor(x => x.Metadata, new MetadataItem(name, value));

            return this;
        }

        /// <summary>
        /// Defines metadata, using <see cref="KeyValuePair{TKey,TValue}"/> instances retrieved from a function, that will be added to the parts created by the convention.
        /// </summary>
        /// <param name="metadataFunction">The function that the metadata can be retrieved from.</param>
        /// <returns>Returns a reference to the same <see cref="PartConventionBuilder{TConvention}"/> instance as the method was called on.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="metadataFunction"/>, or the value returned by it, was null.</exception>
        public PartConventionBuilder<TConvention> AddMetadata(Func<KeyValuePair<string, object>[]> metadataFunction)
        {
            if (metadataFunction == null)
            {
                throw new ArgumentNullException("metadataFunction", "The metadata function cannot be null.");
            }

            var metadata =
                metadataFunction.Invoke();

            if (metadata == null)
            {
                throw new InvalidOperationException("The metadata function cannot return null.");
            }

            var items =
                from meta in metadata
                select new MetadataItem(meta.Key, meta.Value);

            this.ProvideValueFor(x => x.Metadata, items);

            return this;
        }

        /// <summary>
        /// Defines the export conventions which should be assigned to the part convention.
        /// </summary>
        /// <param name="action">A closure for an <see cref="IExportRegistry"/> instance.</param>
        /// <returns>Returns a reference to the same <see cref="PartConventionBuilder{TConvention}"/> instance as the method was called on.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> parameter was null.</exception>
        public PartConventionBuilder<TConvention> Exports(Action<IExportRegistry> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action", "The action cannot be null.");
            }

            var closure =
                new ExportRegistry();

            action.Invoke(closure);

            this.ProvideValueFor(x => x.Exports, closure.GetConventions());

            return this;
        }

        /// <summary>
        /// Defines the condition that a type has to pass in order for the convention to be applied to it.
        /// </summary>
        /// <param name="condition">A function that evaluates if the convention can be applied to the specified type.</param>
        /// <returns>Returns a reference to the same <see cref="PartConventionBuilder{TConvention}"/> instance as the method was called on.</returns>
        /// <exception cref="ArgumentNullException">The the <paramref name="condition"/> parameter was null.</exception>
        public PartConventionBuilder<TConvention> ForTypesMatching(Predicate<Type> condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition", "The condition cannot be null.");
            }

            this.ProvideValueFor(x => x.Condition, condition);

            return this;
        }

        /// <summary>
        /// Defines the import conventions which should be assigned to the part convention.
        /// </summary>
        /// <param name="action">A closure for an <see cref="IImportRegistry"/> instance.</param>
        /// <returns>Returns a reference to the same <see cref="PartConventionBuilder{TConvention}"/> instance as the method was called on.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> parameter was null.</exception>
        public PartConventionBuilder<TConvention> Imports(Action<IImportRegistry> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action", "The action cannot be null.");
            }

            var closure =
                new ImportRegistry();

            action.Invoke(closure);

            this.ProvideValueFor(x => x.Imports, closure.GetConventions());

            return this;
        }

        /// <summary>
        /// Defines that all parts created using the convention should be unique instances.
        /// </summary>
        /// <returns>Returns a reference to the same <see cref="PartConventionBuilder{TConvention}"/> instance as the method was called on.</returns>
        public PartConventionBuilder<TConvention> MakeNonShared()
        {
            this.ProvideValueFor(x => x.CreationPolicy, CreationPolicy.NonShared);
            return this;
        }

        /// <summary>
        /// Defines that all parts created using the convention should use a shared instance.
        /// </summary>
        /// <returns>Returns a reference to the same <see cref="PartConventionBuilder{TConvention}"/> instance as the method was called on.</returns>
        public PartConventionBuilder<TConvention> MakeShared()
        {
            this.ProvideValueFor(x => x.CreationPolicy, CreationPolicy.Shared);
            return this;
        }
    }
}