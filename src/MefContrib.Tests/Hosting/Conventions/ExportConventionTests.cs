using MefContrib.Tests;

namespace MefContrib.Hosting.Conventions.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public class ExportConventionTests
    {
        [Test]
        public void Metadata_should_not_null_on_new_instance()
        {
            var export =
                new ExportConvention();

            export.Metadata.ShouldNotBeNull();
        }

        [Test]
        public void Equals_should_return_false_if_called_with_null()
        {
            var export =
                new ExportConvention();

            var results =
                export.Equals((ExportConvention)null);

            results.ShouldBeFalse();
        }

        [Test]
        public void Equals_should_return_false_when_contract_is_not_same_on_the_instance_that_is_being_compared_againts()
        {
            var export =
                new ExportConvention { ContractName = x => "Foo" };

            var results =
                export.Equals(new ExportConvention { ContractName = x => "Bar" });

            results.ShouldBeFalse();
        }

        [Test]
        public void Equals_should_return_false_when_required_metadata_is_not_same_on_the_instance_that_is_being_compared_against()
        {
            var instance =
                new ExportConvention
                {
                    Metadata = new List<MetadataItem>
                    {
                        new MetadataItem("Name", "Foo")
                    }
                };

            var results = 
                instance.Equals(new ExportConvention
                {
                    Metadata = new List<MetadataItem>
                    {
                        new MetadataItem("Name", "Bar")
                    }
                });

            results.ShouldBeFalse();
        }

        [Test]
        public void Equals_should_return_false_when_type_identity_is_not_same_on_the_instance_that_is_being_compared_against()
        {
            var instance =
                new ExportConvention { ContractType = x => typeof(string) };

            var results =
                instance.Equals(new ExportConvention { ContractType = x => typeof(int) });

            results.ShouldBeFalse();
        }

        [Test]
        public void Equals_should_return_true_if_same_as_instance_being_compared_against()
        {
            var instance =
                new ExportConvention();

            var results =
                instance.Equals(new ExportConvention());

            results.ShouldBeTrue();
        }
    }
}