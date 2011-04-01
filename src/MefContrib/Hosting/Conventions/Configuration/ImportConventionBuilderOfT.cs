namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// A convention builder for <see cref="IImportConvention"/> instances.
    /// </summary>
    /// <typeparam name="TImportConvention">The type of the import convention that will be built by the expression builder.</typeparam>
    /// <typeparam name="TPart">The type of the part used to get imports for.</typeparam>
    public class ImportConventionBuilder<TPart, TImportConvention> :
        ImportConventionBuilder<TImportConvention> where TImportConvention : IImportConvention, new()
    {
        public ImportConventionBuilder<TPart, TImportConvention> Member(Expression<Func<TPart, object>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            this.Members(m => new[] { expression.GetTargetMemberInfo() });

            return this;
        }
    }
}