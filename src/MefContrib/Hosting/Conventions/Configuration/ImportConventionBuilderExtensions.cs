namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;

    /// <summary>
    /// Defines a set of useful <see cref="ImportConventionBuilder{T}"/> extensions.
    /// </summary>
    public static class ImportConventionBuilderExtensions
    {
        public static ImportConventionBuilder<ImportConvention> ImportProperty(
            this ImportConventionBuilder<ImportConvention> builder, string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            builder.Members(m => new[] { m.GetProperty(propertyName) });

            return builder;
        }
    }
}