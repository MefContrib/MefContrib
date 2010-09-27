using MefContrib.Tests;

namespace MefContrib.Hosting.Conventions.Tests
{
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class TypeExtensionsTests
    {
        [Test]
        public void GetRequiredMetadata_should_return_metadata_items_for_public_non_readable_properties_on_interface()
        {
            var requiredMetadata =
                typeof(IFakeMetadataView).GetRequiredMetadata();

            requiredMetadata.Count().ShouldEqual(1);
        }
    }
}