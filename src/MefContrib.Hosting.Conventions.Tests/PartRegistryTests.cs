namespace MefContrib.Hosting.Conventions.Tests
{
    using MefContrib.Hosting.Conventions.Configuration;
    using NUnit.Framework;

    [TestFixture]
    public class PartRegistryTests
    {
        [Test]
        public void ContractService_should_be_instance_of_defaultconventioncontractservice_on_new_instance()
        {
            var registry =
                new PartRegistry();

            registry.ContractService.ShouldBeOfType<DefaultConventionContractService>();
        }

        [Test]
        public void TypeLoader_should_be_null_on_new_instance()
        {
            var registry =
                new PartRegistry();

            registry.TypeLoader.ShouldBeNull();
        }

        [Test]
        public void Part_should_return_instance_of_partconventionbuilder_for_partconvention_type()
        {
            var registry =
                new PartRegistry();

            var result =
                registry.Part();

            result.ShouldBeOfType<PartConventionBuilder<PartConvention>>();
        }

        [Test]
        public void Part_of_tconvention_should_return_instance_of_partconventionbuilder_for_tconvention_type()
        {
            var registry =
                new PartRegistry();

            var result =
                registry.Part<PartConvention>();

            result.ShouldBeOfType<PartConventionBuilder<PartConvention>>();
        }
    }
}