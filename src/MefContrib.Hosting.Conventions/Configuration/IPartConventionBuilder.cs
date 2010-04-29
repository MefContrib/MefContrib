namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the functionality of a <see cref="IPartConvention"/> expression builder.
    /// </summary>
    /// <typeparam name="TPartConvention">The type of the part convention that should be built by the expression builder.</typeparam>
    public interface IPartConventionBuilder<TPartConvention> where TPartConvention : IPartConvention, new()
    {
        /// <summary>
        /// Defines metadata, using a name and value pair, that will be added to the parts created by the convention.
        /// </summary>
        /// <param name="name">The name of the metadata.</param>
        /// <param name="value">The value of the metadata.</param>
        /// <returns>Returns a reference to the same <see cref="IPartConventionBuilder{TPartConvention}"/> instance as the method was called on.</returns>
        IPartConventionBuilder<TPartConvention> AddMetadata(string name, object value);

        /// <summary>
        /// Defines metadata, using property name and values extracted from an anonymous type, that will be added to the parts created by the convention.
        /// </summary>
        /// <param name="anonymousType">The anonymous type that the names and values will be extracted from.</param>
        /// <returns>Returns a reference to the same <see cref="IPartConventionBuilder{TPartConvention}"/> instance as the method was called on.</returns>
        IPartConventionBuilder<TPartConvention> AddMetadata(object anonymousType);

        /// <summary>
        /// Defines metadata, using <see cref="KeyValuePair{TKey,TValue}"/> instances retrieved from a function, that will be added to the parts created by the convention.
        /// </summary>
        /// <param name="metadataFunction">The function that the metadata can be retrieved from.</param>
        /// <returns>Returns a reference to the same <see cref="IPartConventionBuilder{TPartConvention}"/> instance as the method was called on.</returns>
        IPartConventionBuilder<TPartConvention> AddMetadata(Func<KeyValuePair<string, object>[]> metadataFunction);

        /// <summary>
        /// Defines the export conventions which should be assigned to the part convention.
        /// </summary>
        /// <param name="action">A closure for an <see cref="IExportRegistry"/> instance.</param>
        /// <returns>Returns a reference to the same <see cref="IPartConventionBuilder{TPartConvention}"/> instance as the method was called on.</returns>
        IPartConventionBuilder<TPartConvention> Exports(Action<IExportRegistry> action);

        /// <summary>
        /// Defines the condition that a type has to pass in order for the convention to be applied to it.
        /// </summary>
        /// <param name="condition">A function that evaluates if the convention can be applied to the specified type.</param>
        /// <returns>Returns a reference to the same <see cref="IPartConventionBuilder{TPartConvention}"/> instance as the method was called on.</returns>
        IPartConventionBuilder<TPartConvention> ForTypesMatching(Predicate<Type> condition);

        /// <summary>
        /// Defines the import conventions which should be assigned to the part convention.
        /// </summary>
        /// <param name="action">A closure for an <see cref="IImportRegistry"/> instance.</param>
        /// <returns>Returns a reference to the same <see cref="IPartConventionBuilder{TPartConvention}"/> instance as the method was called on.</returns>
        IPartConventionBuilder<TPartConvention> Imports(Action<IImportRegistry> action);

        /// <summary>
        /// Defines that all parts created using the convention should be unique instances.
        /// </summary>
        /// <returns>Returns a reference to the same <see cref="IPartConventionBuilder{TPartConvention}"/> instance as the method was called on.</returns>
        IPartConventionBuilder<TPartConvention> MakeNonShared();

        /// <summary>
        /// Defines that all parts created using the convention should use a shared instance.
        /// </summary>
        /// <returns>Returns a reference to the same <see cref="IPartConventionBuilder{TPartConvention}"/> instance as the method was called on.</returns>
        IPartConventionBuilder<TPartConvention> MakeShared();
    }
}