namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;

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