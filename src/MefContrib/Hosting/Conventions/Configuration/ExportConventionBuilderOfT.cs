namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// A convention builder for <see cref="IExportConvention"/> instances.
    /// </summary>
    /// <typeparam name="TExportConvention">The type of the export convention that will be built by the expression builder.</typeparam>
    /// <typeparam name="TPart">The type of the part this export is applied to.</typeparam>
    public class ExportConventionBuilder<TPart, TExportConvention>
        : ExportConventionBuilder<TExportConvention> where TExportConvention : IExportConvention, new()
    {
        public ExportConventionBuilder<TPart, TExportConvention> Member(Expression<Func<TPart, object>> expression)
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