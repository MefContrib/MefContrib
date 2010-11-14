using MefContrib.Tests;

namespace MefContrib.Hosting.Conventions.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class PartConventionTests
    {
        [Test]
        public void Export_should_not_be_null_on_new_instance()
        {
            var instance =
                new PartConvention();

            instance.Exports.ShouldNotBeNull();
        }

        [Test]
        public void Import_should_not_be_null_on_new_instance()
        {
            var instance =
                new PartConvention();

            instance.Imports.ShouldNotBeNull();
        }

        [Test]
        public void Metadata_should_not_be_null_on_new_instance()
        {
            var instance =
                new PartConvention();

            instance.Metadata.ShouldNotBeNull();
        }
    }
}