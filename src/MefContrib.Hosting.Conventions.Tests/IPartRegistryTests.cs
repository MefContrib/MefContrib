namespace MefContrib.Hosting.Conventions.Tests
{
    using MefContrib.Hosting.Conventions.Configuration;
    using NUnit.Framework;

    [TestFixture]
    public class IPartRegistryTests
    {
        [Test]
        public void IPartRegistry_should_implement_ihideobjectmemebers_interface()
        {
            typeof(IPartRegistry<IContractService>).GetInterfaces().ShouldContainType<IHideObjectMembers>();
        }
    }
}