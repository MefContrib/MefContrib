namespace MefContrib.Hosting.Conventions.Tests
{
    using MefContrib.Hosting.Conventions.Configuration;
    using MefContrib.Tests;

    using NUnit.Framework;

    [TestFixture]
    public class ImportRegistryTests
    {
        [Test]
        public void Import_should_return_instance_of_importconventionbuilder_for_importconvention_type()
        {
            var registry =
                new ImportRegistry();

            var result =
                registry.Import();

            result.ShouldBeOfType<ImportConventionBuilder<ImportConvention>>();
        }

        [Test]
        public void Import_of_tconvention_should_return_instance_of_importconventionbuilder_for_tconvention_type()
        {
            var registry =
                new ImportRegistry();

            var result =
                registry.ImportWithConvention<ImportConvention>();

            result.ShouldBeOfType<ImportConventionBuilder<ImportConvention>>();
        }
    }
}