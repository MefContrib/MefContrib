namespace MefContrib.Hosting.Conventions.Tests
{
    using MefContrib.Hosting.Conventions.Configuration;
    using MefContrib.Tests;

    using NUnit.Framework;

    [TestFixture]
    public class ExportRegistryTests
    {
        [Test]
        public void Export_should_return_instance_of_exportconventionbuilder_for_exportconvention_type()
        {
            var registry =
                new ExportRegistry();

            var result =
                registry.Export();

            result.ShouldBeOfType<ExportConventionBuilder<ExportConvention>>();
        }

        [Test]
        public void Export_of_tconvention_should_return_instance_of_exportconventionbuilder_for_tconvention_type()
        {
            var registry =
                new ExportRegistry();

            var result =
                registry.Export<ExportConvention>();

            result.ShouldBeOfType<ExportConventionBuilder<ExportConvention>>();
        }
    }
}