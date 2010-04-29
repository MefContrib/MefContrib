namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// A convention builder for <see cref="IImportConvention"/> instances.
    /// </summary>
    /// <typeparam name="TImportConvention">The type of the import convention that will be built by the expression builder.</typeparam>
    public class ImportConventionBuilder<TImportConvention> : 
        ExpressionBuilder<TImportConvention>, IImportConventionBuilder<TImportConvention>, IConventionBuilder<TImportConvention> where TImportConvention : IImportConvention, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportConventionBuilder{TImportConvention}"/> class.
        /// </summary>
        public ImportConventionBuilder()
        {
        }

        /// <summary>
        /// Defines that the imports created by the convention allows default values if no matching exports could be found.
        /// </summary>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        public IImportConventionBuilder<TImportConvention> AllowDefaultValue()
        {
            this.ProvideValueFor(x => x.AllowDefaultValue, true);
            return this;
        }

        /// <summary>
        /// Defines the contract name that will be added to the imports created by the convention.
        /// </summary>
        /// <param name="contractName">A <see cref="string"/> containing the name of the contract which should be used by the created imports.</param>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        public IImportConventionBuilder<TImportConvention> ContractName(string contractName)
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
        /// Defines the contract name that will be added to the imports created by the convention, by deriving it from the provided type.
        /// </summary>
        /// <typeparam name="TContractType">A <see cref="Type"/> that should be used as the contract name of the created imports.</typeparam>
        /// <returns>Returns a reference to the same <see cref="IExportConventionBuilder{TExportConvention}"/> instance as the method was called on.</returns>
        public IImportConventionBuilder<TImportConvention> ContractName<TContractType>()
        {
            Func<MemberInfo, string> contractNameFunction =
                x => typeof(TContractType).FullName;

            this.ProvideValueFor(x => x.ContractName, contractNameFunction);

            return this;
        }

        /// <summary>
        /// Defined the contract name that will be added to the imports created by the convention, by invoking the function.
        /// </summary>
        /// <param name="contractNameFunction">A function that accepts a <see cref="MemberInfo"/> instance for the member that is being imported and returns the contract name.</param>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        public IImportConventionBuilder<TImportConvention> ContractName(Func<MemberInfo, string> contractNameFunction)
        {
            if (contractNameFunction == null)
            {
                throw new ArgumentNullException("contractNameFunction", "The contract name function cannot be null.");
            }

            this.ProvideValueFor(x => x.ContractName, contractNameFunction);

            return this;
        }

        /// <summary>
        /// Defines the contract type that will be added to the imports created by the convention.
        /// </summary>
        /// <typeparam name="TContractType">A <see cref="Type"/> that should be used as the contract type of the created imports.</typeparam>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        public IImportConventionBuilder<TImportConvention> ContractType<TContractType>()
        {
            Func<MemberInfo, Type> contractTypeFunction = x => typeof(TContractType);

            this.ProvideValueFor(x => x.ContractType, contractTypeFunction);

            return this;
        }

        /// <summary>
        /// Defined the contract type that will be added to the imports created by the convention, by invoking the function.
        /// </summary>
        /// <param name="contractTypeFunction">A function that accepts a <see cref="MemberInfo"/> instance for the member that is being imported and returns the contract type.</param>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        public IImportConventionBuilder<TImportConvention> ContractType(Func<MemberInfo, Type> contractTypeFunction)
        {
            if (contractTypeFunction == null)
            {
                throw new ArgumentNullException("contractTypeFunction", "The contract type function cannot be null.");
            }

            this.ProvideValueFor(x => x.ContractType, contractTypeFunction);

            return this;
        }

        /// <summary>
        /// Gets the convention instance built by the convention builder.
        /// </summary>
        /// <returns>
        /// An instance of the convention type that the convention builder can build.
        /// </returns>
        public TImportConvention GetBuiltInstance()
        {
            return (TImportConvention)this.Build();
        }

        /// <summary>
        /// Defines that all imports created using the convention should be unique instances.
        /// </summary>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        public IImportConventionBuilder<TImportConvention> MakeNonShared()
        {
            this.ProvideValueFor(x => x.CreationPolicy, CreationPolicy.NonShared);
            return this;
        }

        /// <summary>
        /// Defines that all imports created using the convention should use a shared instance.
        /// </summary>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        public IImportConventionBuilder<TImportConvention> MakeShared()
        {
            this.ProvideValueFor(x => x.CreationPolicy, CreationPolicy.Shared);
            return this;
        }

        /// <summary>
        /// Defines the members which should be used as imports by retreiving them from a function.
        /// </summary>
        /// <param name="expression">A <see cref="Func{Type, MemberInfo}"/> function that returns a collection of <see cref="MemberInfo"/> instances based on the provided <see cref="Type"/>.</param>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        /// <exception cref="ArgumentNullException">The value of the <paramref name="expression"/> parameter was null.</exception>
        public IImportConventionBuilder<TImportConvention> Members(Func<Type, MemberInfo[]> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression", "The expression cannot be null.");
            }

            this.ProvideValueFor(x => x.Members, expression);

            return this;
        }

        /// <summary>
        /// Defines the members which sould be used as imports by extracting them from a lambda expression with no return value.
        /// </summary>
        /// <typeparam name="TPart">The <see cref="Type"/> that the members should be extracted from.</typeparam>
        /// <param name="expression">An expression that idenfies the members which should be used as imports.</param>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        /// <exception cref="ArgumentNullException">The value of the <paramref name="expression"/> parameter was null.</exception>
        public IImportConventionBuilder<TImportConvention> Members<TPart>(Expression<Action<TPart>> expression)
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
        /// Defines the members which sould be used as imports by extracting them from a lambda expression with a return value.
        /// </summary>
        /// <typeparam name="TPart">The <see cref="Type"/> that the members should be extracted from.</typeparam>
        /// <param name="expression">An expression that idenfies the members which should be used as imports.</param>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        /// <exception cref="ArgumentNullException">The value of the <paramref name="expression"/> parameter was null.</exception>
        public IImportConventionBuilder<TImportConvention> Members<TPart>(Expression<Func<TPart, object>> expression)
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
        /// Defines that the imports created by the convention allows recomposition.
        /// </summary>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        public IImportConventionBuilder<TImportConvention> Recomposable()
        {
            this.ProvideValueFor(x => x.Recomposable, true);
            return this;
        }

        /// <summary>
        /// Defines metadata that imports created by the convention needs to have satisfied, by using the provided name and type.
        /// </summary>
        /// <typeparam name="TMetadataType">The <see cref="Type"/> of the required metadata.</typeparam>
        /// <param name="name">A <see cref="string"/> containing the name of the required metadata.</param>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        /// <exception cref="ArgumentNullException">The value of the <paramref name="name"/> parameter was null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The lenght of the <paramref name="name"/> parameter was zero.</exception>
        public IImportConventionBuilder<TImportConvention> RequireMetadata<TMetadataType>(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name", "The name cannot be null.");
            }

            if (name.Length == 0)
            {
                throw new ArgumentOutOfRangeException("name", "The name cannot be empty.");
            }

            var requiredMetadata =
                new RequiredMetadataItem(name, typeof(TMetadataType));

            this.ProvideValueFor(x => x.RequiredMetadata, requiredMetadata);

            return this;
        }

        /// <summary>
        /// Defines metadata that imports created by the convention needs to have satisfied, by extracting the property names and types from the provided type.
        /// </summary>
        /// <typeparam name="TMetadataView">The <see cref="Type"/> that the property names and values should be extracted from.</typeparam>
        /// <returns>Returns a reference to the same <see cref="IImportConventionBuilder{TImportConvention}"/> instance as the method was called on.</returns>
        public IImportConventionBuilder<TImportConvention> RequireMetadata<TMetadataView>() where TMetadataView : class
        {
            var requiredMetadata =
                typeof(TMetadataView).GetRequiredMetadata();

            this.ProvideValueFor(x => x.RequiredMetadata, requiredMetadata);

            return this;
        }
    }
}