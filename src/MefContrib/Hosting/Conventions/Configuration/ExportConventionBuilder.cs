namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// A convention builder for <see cref="IExportConvention"/> instances.
    /// </summary>
    /// <typeparam name="TExportConvention">The type of the export convention that will be built by the expression builder.</typeparam>
    public class ExportConventionBuilder<TExportConvention>
        : ConventionBuilder<TExportConvention> where TExportConvention : IExportConvention, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportConventionBuilder{TExportConvention}"/> class.
        /// </summary>
        public ExportConventionBuilder()
        {
        }

        /// <summary>
        /// Defines metadata, using property name and values extracted from an anonymous type, that will be added to the exports created by the convention.
        /// </summary>
        /// <param name="anonymousType">The anonymous type that the names and values will be extracted from.</param>
        /// <returns>Returns a reference to the same <see cref="ExportConventionBuilder{TExportConvention}"/> instance as the method was called on.</returns>
        /// <exception cref="ArgumentNullException">The method was called with a null value.</exception>
        public ExportConventionBuilder<TExportConvention> AddMetadata(object anonymousType)
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
        /// Defines metadata, using a name and value pair, that will be added to the exports created by the convention.
        /// </summary>
        /// <param name="name">The name of the metadata.</param>
        /// <param name="value">The value of the metadata.</param>
        /// <returns>Returns a reference to the same <see cref="ExportConventionBuilder{TExportConvention}"/> instance as the method was called on.</returns>
        /// <exception cref="ArgumentNullException">The value of the <paramref name="name"/> or <paramref name="value"/> parameters were null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The lenght of the <paramref name="name"/> was zero.</exception>
        public ExportConventionBuilder<TExportConvention> AddMetadata(string name, object value)
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
        /// Defines metadata, using <see cref="KeyValuePair{TKey,TValue}"/> instances retrieved from a function, that will be added to the exports created by the convention.
        /// </summary>
        /// <param name="metadataFunction">The function that the metadata can be retrieved from.</param>
        /// <returns>Returns a reference to the same <see cref="ExportConventionBuilder{TExportConvention}"/> instance as the method was called on.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="metadataFunction"/>, or the value returned by it, was null.</exception>
        public ExportConventionBuilder<TExportConvention> AddMetadata(Func<KeyValuePair<string, object>[]> metadataFunction)
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
        /// Defines the contract name that will be added to the exports created by the convention.
        /// </summary>
        /// <param name="contractName">A <see cref="string"/> containing the name of the contract which should be used by the created exports.</param>
        /// <returns>Returns a reference to the same <see cref="ExportConventionBuilder{TExportConvention}"/> instance as the method was called on.</returns>
        public ExportConventionBuilder<TExportConvention> ContractName(string contractName)
        {
            if (contractName != null)
            {
                if (contractName.Length == 0)
                {
                    throw new ArgumentOutOfRangeException("contractName", "The contract name cannot be empty.");
                }
            }

            Func<MemberInfo, string> contractNameFunction = x => contractName;

            this.ProvideValueFor(x => x.ContractName, contractNameFunction);

            return this;
        }

        /// <summary>
        /// Defines the contract name that will be added to the exports created by the convention, by deriving it from the provided type.
        /// </summary>
        /// <typeparam name="TContractType">A <see cref="Type"/> that should be used as the contract name of the created exports.</typeparam>
        /// <returns>Returns a reference to the same <see cref="ExportConventionBuilder{TExportConvention}"/> instance as the method was called on.</returns>
        public ExportConventionBuilder<TExportConvention> ContractName<TContractType>()
        {
            Func<MemberInfo, string> contractNameFunction =
                x => AttributedModelServices.GetContractName(typeof (TContractType));

            this.ProvideValueFor(x => x.ContractName, contractNameFunction);

            return this;
        }

        /// <summary>
        /// Defined the contract name that will be added to the exports created by the convention, by invoking the function.
        /// </summary>
        /// <param name="contractNameFunction">A function that accepts a <see cref="MemberInfo"/> instance for the member that is being exported and returns the contract name.</param>
        /// <returns>Returns a reference to the same <see cref="ExportConventionBuilder{TExportConvention}"/> instance as the method was called on.</returns>
        public ExportConventionBuilder<TExportConvention> ContractName(Func<MemberInfo, string> contractNameFunction)
        {
            if (contractNameFunction == null)
            {
                throw new ArgumentNullException("contractNameFunction", "The contract name function cannot be null.");
            }

            this.ProvideValueFor(x => x.ContractName, contractNameFunction);

            return this;
        }

        /// <summary>
        /// Defines the contract type that will be added to the exports created by the convention.
        /// </summary>
        /// <typeparam name="TContractType">A <see cref="Type"/> that should be used as the contract type of the created exports.</typeparam>
        /// <returns>Returns a reference to the same <see cref="ExportConventionBuilder{TExportConvention}"/> instance as the method was called on.</returns>
        public ExportConventionBuilder<TExportConvention> ContractType<TContractType>()
        {
            Func<MemberInfo, Type> contractTypeFunction = x => typeof(TContractType);

            this.ProvideValueFor(x => x.ContractType, contractTypeFunction);

            return this;
        }

        /// <summary>
        /// Defined the contract type that will be added to the exports created by the convention, by invoking the function.
        /// </summary>
        /// <param name="contractTypeFunction">A function that accepts a <see cref="MemberInfo"/> instance for the member that is being exported and returns the contract type.</param>
        /// <returns>Returns a reference to the same <see cref="ExportConventionBuilder{TExportConvention}"/> instance as the method was called on.</returns>
        public ExportConventionBuilder<TExportConvention> ContractType(Func<MemberInfo, Type> contractTypeFunction)
        {
            if (contractTypeFunction == null)
            {
                throw new ArgumentNullException("contractTypeFunction", "The contract type function cannot be null.");
            }

            this.ProvideValueFor(x => x.ContractType, contractTypeFunction);

            return this;
        }

        /// <summary>
        /// Defines the members which should be used as exports by retreiving them from a function.
        /// </summary>
        /// <param name="expression">A <see cref="Func{Type, MemberInfo}"/> function that returns a collection of <see cref="MemberInfo"/> instances based on the provided <see cref="Type"/>.</param>
        /// <returns>Returns a reference to the same <see cref="ExportConventionBuilder{TExportConvention}"/> instance as the method was called on.</returns>
        /// <exception cref="ArgumentNullException">The value of the <paramref name="expression"/> parameter was null.</exception>
        public ExportConventionBuilder<TExportConvention> Members(Func<Type, MemberInfo[]> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression", "The expression cannot be null.");
            }

            this.ProvideValueFor(x => x.Members, expression);

            return this;
        }

        /// <summary>
        /// Defines the members which sould be used as exports by extracting them from a lambda expression with no return value.
        /// </summary>
        /// <typeparam name="TPart">The <see cref="Type"/> that the members should be extracted from.</typeparam>
        /// <param name="expression">An expression that idenfies the members which should be used as exports.</param>
        /// <returns>Returns a reference to the same <see cref="ExportConventionBuilder{TExportConvention}"/> instance as the method was called on.</returns>
        /// <exception cref="ArgumentNullException">The value of the <paramref name="expression"/> parameter was null.</exception>
        public ExportConventionBuilder<TExportConvention> Members<TPart>(Expression<Action<TPart>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression", "The expression cannot be null.");
            }

            Func<Type, MemberInfo[]> members = 
                x => new[] { expression.GetTargetMemberInfo() };

            this.ProvideValueFor(x => x.Members, members);

            return this;
        }

        /// <summary>
        /// Defines the members which sould be used as exports by extracting them from a lambda expression with a return value.
        /// </summary>
        /// <typeparam name="TPart">The <see cref="Type"/> that the members should be extracted from.</typeparam>
        /// <param name="expression">An expression that idenfies the members which should be used as exports.</param>
        /// <returns>Returns a reference to the same <see cref="ExportConventionBuilder{TExportConvention}"/> instance as the method was called on.</returns>
        /// <exception cref="ArgumentNullException">The value of the <paramref name="expression"/> parameter was null.</exception>
        public ExportConventionBuilder<TExportConvention> Members<TPart>(Expression<Func<TPart, object>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression", "The expression cannot be null.");
            }

            Func<Type, MemberInfo[]> members =
                x => new[] { expression.GetTargetMemberInfo() };

            this.ProvideValueFor(x => x.Members, members);

            return this;
        }
    }
}