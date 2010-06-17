namespace MefContrib.Hosting.Conventions.Tests
{
    using MefContrib.Hosting.Conventions.Configuration;
    using NUnit.Framework;

    [TestFixture]
    public class IConventionRegistryTests
    {
        [Test]
        public void IConventionRegistry_should_implement_ihideobjectmemebers_interface()
        {
            typeof(IConventionRegistry<>).GetInterfaces().ShouldContainType<IHideObjectMembers>();
        }
    }
}