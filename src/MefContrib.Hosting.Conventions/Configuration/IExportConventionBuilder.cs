namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Defines the functionality of a <see cref="IExportConvention"/> expression builder.
    /// </summary>
    /// <typeparam name="TExportConvention">The type of the export convention that should be built by the expression builder.</typeparam>
    public interface IExportConventionBuilder<TExportConvention> where TExportConvention : IExportConvention, new()
    {
        /// <summary>
        /// Defines metadata, using a name and value pair, that will be added to the exports created by the convention.
        /// </summary>
        /// <param name="name">The name of the metadata.</param>
        /// <param name="value">The value of the metadata.</param>
        /// <returns>Returns a reference to the same <see cref="IExportConventionBuilder{TExportConvention}"/> instance as the method was called on.</returns>
        IExportConventionBuilder<TExportConvention> AddMetadata(string name, object value);

        /// <summary>
        /// Defines metadata, using property name and values extracted from an anonymous type, that will be added to the exports created by the convention.
        /// </summary>
        /// <param name="anonymousType">The anonymous type that the names and values will be extracted from.</param>
        /// <returns>Returns a reference to the same <see cref="IExportConventionBuilder{TExportConvention}"/> instance as the method was called on.</returns>
        IExportConventionBuilder<TExportConvention> AddMetadata(object anonymousType);

        /// <summary>
        /// Defines metadata, using <see cref="KeyValuePair{TKey,TValue}"/> instances retrieved from a function, that will be added to the exports created by the convention.
        /// </summary>
        /// <param name="metadataFunction">The function that the metadata can be retrieved from.</param>
        /// <returns>Returns a reference to the same <see cref="IExportConventionBuilder{TExportConvention}"/> instance as the method was called on.</returns>
        IExportConventionBuilder<TExportConvention> AddMetadata(Func<KeyValuePair<string, object>[]> metadataFunction);

        /// <summary>
        /// Defines the contract name that will be added to the exports created by the convention.
        /// </summary>
        /// <param name="contractName">A <see cref="string"/> containing the name of the contract which should be used by the created exports.</param>
        /// <returns>Returns a reference to the same <see cref="IExportConventionBuilder{TExportConvention}"/> instance as the method was called on.</returns>
        IExportConventionBuilder<TExportConvention> ContractName(string contractName);

        /// <summary>
        /// Defines the contract name that will be added to the exports created by the convention, by deriving it from the provided type.
        /// </summary>
        /// <typeparam name="TContractType">A <see cref="Type"/> that should be used as the contract name of the created exports.</typeparam>
        /// <returns>Returns a reference to the same <see cref="IExportConventionBuilder{TExportConvention}"/> instance as the method was called on.</returns>
        IExportConventionBuilder<TExportConvention> ContractName<TContractType>();

        /// <summary>
        /// Defined the contract name that will be added to the exports created by the convention, by invoking the function.
        /// </summary>
        /// <param name="contractNameFunction">A function that accepts a <see cref="MemberInfo"/> instance for the member that is being exported and returns the contract name.</param>
        /// <returns>Returns a reference to the same <see cref="IExportConventionBuilder{TExportConvention}"/> instance as the method was called on.</returns>
        IExportConventionBuilder<TExportConvention> ContractName(Func<MemberInfo, string> contractNameFunction);

        /// <summary>
        /// Defines the contract type that will be added to the exports created by the convention.
        /// </summary>
        /// <typeparam name="TContractType">A <see cref="Type"/> that should be used as the contract type of the created exports.</typeparam>
        /// <returns>Returns a reference to the same <see cref="IExportConventionBuilder{TExportConvention}"/> instance as the method was called on.</returns>
        IExportConventionBuilder<TExportConvention> ContractType<TContractType>();

        /// <summary>
        /// Defined the contract type that will be added to the exports created by the convention, by invoking the function.
        /// </summary>
        /// <param name="contractTypeFunction">A function that accepts a <see cref="MemberInfo"/> instance for the member that is being exported and returns the contract type.</param>
        /// <returns>Returns a reference to the same <see cref="IExportConventionBuilder{TExportConvention}"/> instance as the method was called on.</returns>
        IExportConventionBuilder<TExportConvention> ContractType(Func<MemberInfo, Type> contractTypeFunction);

        /// <summary>
        /// Defines the members which should be used as exports by retreiving them from a function.
        /// </summary>
        /// <param name="expression">A <see cref="Func{Type, MemberInfo}"/> function that returns a collection of <see cref="MemberInfo"/> instances based on the provided <see cref="Type"/>.</param>
        /// <returns>Returns a reference to the same <see cref="IExportConventionBuilder{TExportConvention}"/> instance as the method was called on.</returns>
        IExportConventionBuilder<TExportConvention> Members(Func<Type, MemberInfo[]> expression);

        /// <summary>
        /// Defines the members which sould be used as exports by extracting them from a lambda expression with no return value.
        /// </summary>
        /// <typeparam name="TPart">The <see cref="Type"/> that the members should be extracted from.</typeparam>
        /// <param name="expression">An expression that idenfies the members which should be used as exports.</param>
        /// <returns>Returns a reference to the same <see cref="IExportConventionBuilder{TExportConvention}"/> instance as the method was called on.</returns>
        IExportConventionBuilder<TExportConvention> Members<TPart>(Expression<Action<TPart>> expression);

        /// <summary>
        /// Defines the members which sould be used as exports by extracting them from a lambda expression with a return value.
        /// </summary>
        /// <typeparam name="TPart">The <see cref="Type"/> that the members should be extracted from.</typeparam>
        /// <param name="expression">An expression that idenfies the members which should be used as exports.</param>
        /// <returns>Returns a reference to the same <see cref="IExportConventionBuilder{TExportConvention}"/> instance as the method was called on.</returns>
        IExportConventionBuilder<TExportConvention> Members<TPart>(Expression<Func<TPart, object>> expression);
    }
}