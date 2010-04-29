namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Defines the functionality of a <see cref="IImportConvention"/> expression builder.
    /// </summary>
    /// <typeparam name="TImportConvention">The type of the import convention that should be built by the expression builder.</typeparam>
    public interface IImportConventionBuilder<TImportConvention> where TImportConvention : IImportConvention, new()
    {
        /// <summary>
        /// Defines that the imports created by the convention allows default values if no matching exports could be found.
        /// </summary>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        IImportConventionBuilder<TImportConvention> AllowDefaultValue();

        /// <summary>
        /// Defines the contract name that will be added to the imports created by the convention.
        /// </summary>
        /// <param name="contractName">A <see cref="string"/> containing the name of the contract which should be used by the created imports.</param>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        IImportConventionBuilder<TImportConvention> ContractName(string contractName);

        /// <summary>
        /// Defines the contract name that will be added to the imports created by the convention, by deriving it from the provided type.
        /// </summary>
        /// <typeparam name="TContractType">A <see cref="Type"/> that should be used as the contract name of the created imports.</typeparam>
        /// <returns>Returns a reference to the same <see cref="IExportConventionBuilder{TExportConvention}"/> instance as the method was called on.</returns>
        IImportConventionBuilder<TImportConvention> ContractName<TContractType>();

        /// <summary>
        /// Defined the contract name that will be added to the imports created by the convention, by invoking the function.
        /// </summary>
        /// <param name="contractNameFunction">A function that accepts a <see cref="MemberInfo"/> instance for the member that is being imported and returns the contract name.</param>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        IImportConventionBuilder<TImportConvention> ContractName(Func<MemberInfo, string> contractNameFunction);

        /// <summary>
        /// Defines the contract type that will be added to the imports created by the convention.
        /// </summary>
        /// <typeparam name="TContractType">A <see cref="Type"/> that should be used as the contract type of the created imports.</typeparam>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        IImportConventionBuilder<TImportConvention> ContractType<TContractType>();

        /// <summary>
        /// Defined the contract type that will be added to the imports created by the convention, by invoking the function.
        /// </summary>
        /// <param name="contractTypeFunction">A function that accepts a <see cref="MemberInfo"/> instance for the member that is being imported and returns the contract type.</param>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        IImportConventionBuilder<TImportConvention> ContractType(Func<MemberInfo, Type> contractTypeFunction);

        /// <summary>
        /// Defines that all imports created using the convention should be unique instances.
        /// </summary>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        IImportConventionBuilder<TImportConvention> MakeNonShared();

        /// <summary>
        /// Defines that all imports created using the convention should use a shared instance.
        /// </summary>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        IImportConventionBuilder<TImportConvention> MakeShared();

        /// <summary>
        /// Defines the members which should be used as imports by retreiving them from a function.
        /// </summary>
        /// <param name="expression">A <see cref="Func{Type, MemberInfo}"/> function that returns a collection of <see cref="MemberInfo"/> instances based on the provided <see cref="Type"/>.</param>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        IImportConventionBuilder<TImportConvention> Members(Func<Type, MemberInfo[]> expression);

        /// <summary>
        /// Defines the members which sould be used as imports by extracting them from a lambda expression with no return value.
        /// </summary>
        /// <typeparam name="TPart">The <see cref="Type"/> that the members should be extracted from.</typeparam>
        /// <param name="expression">An expression that idenfies the members which should be used as imports.</param>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        IImportConventionBuilder<TImportConvention> Members<TPart>(Expression<Action<TPart>> expression);

        /// <summary>
        /// Defines the members which sould be used as imports by extracting them from a lambda expression with a return value.
        /// </summary>
        /// <typeparam name="TPart">The <see cref="Type"/> that the members should be extracted from.</typeparam>
        /// <param name="expression">An expression that idenfies the members which should be used as imports.</param>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        IImportConventionBuilder<TImportConvention> Members<TPart>(Expression<Func<TPart, object>> expression);

        /// <summary>
        /// Defines that the imports created by the convention allows recomposition.
        /// </summary>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        IImportConventionBuilder<TImportConvention> Recomposable();

        /// <summary>
        /// Defines metadata that imports created by the convention needs to have satisfied, by using the provided name and type.
        /// </summary>
        /// <typeparam name="TMetadataType">The <see cref="Type"/> of the required metadata.</typeparam>
        /// <param name="name">A <see cref="string"/> containing the name of the required metadata.</param>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        IImportConventionBuilder<TImportConvention> RequireMetadata<TMetadataType>(string name);

        /// <summary>
        /// Defines metadata that imports created by the convention needs to have satisfied, by extracting the property names and types from the provided type.
        /// </summary>
        /// <typeparam name="TMetadataView">The <see cref="Type"/> that the property names and values should be extracted from.</typeparam>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        IImportConventionBuilder<TImportConvention> RequireMetadata<TMetadataView>() where TMetadataView : class;
    }
}