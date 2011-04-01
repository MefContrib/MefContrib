using System;
using System.Linq.Expressions;

namespace MefContrib.Hosting.Conventions.Configuration
{
    /// <summary>
    /// A convention builder for <see cref="IPartConvention"/> instances.
    /// </summary>
    /// <typeparam name="TPart">The type of the part this convention is applied to.</typeparam>
    /// <typeparam name="TConvention">The type of the part convention that will be built by the expression builder.</typeparam>
    public class PartConventionBuilder<TPart, TConvention> :
        PartConventionBuilder<TConvention> where TConvention : IPartConvention, new()
    {
        /// <summary>
        /// Defines the export conventions which should be assigned to the part convention.
        /// </summary>
        /// <param name="action">A closure for an <see cref="IExportRegistry"/> instance.</param>
        /// <returns>Returns a reference to the same <see cref="PartConventionBuilder{TConvention}"/> instance as the method was called on.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> parameter was null.</exception>
        public new PartConventionBuilder<TPart, TConvention> Exports(Action<IExportRegistry> action)
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
        /// Defines the import conventions which should be assigned to the part convention.
        /// </summary>
        /// <param name="action">A closure for an <see cref="IImportRegistry"/> instance.</param>
        /// <returns>Returns a reference to the same <see cref="PartConventionBuilder{TConvention}"/> instance as the method was called on.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> parameter was null.</exception>
        public new PartConventionBuilder<TPart, TConvention> Imports(Action<IImportRegistry> action)
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

        public PartConventionBuilder<TPart, TConvention> Export()
        {
            this.Exports(x => x.Export().Members(m => new[] { m }));

            return this;
        }

        public PartConventionBuilder<TPart, TConvention> ExportMember(Expression<Func<TPart, object>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            this.Exports(i => i.Export().Members(m => new[] { expression.GetTargetMemberInfo() }));

            return this;
        }

        public PartConventionBuilder<TPart, TConvention> ImportMember(Expression<Func<TPart, object>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            this.Imports(i => i.Import().Members(m => new[] { expression.GetTargetMemberInfo() }));

            return this;
        }
    }
}