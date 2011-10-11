using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace MefContrib.Integration.Unity.Tests
{
    [TestFixture]
    public class ResolutionOrderTests
    {
        [Test]
        public void Unity_registered_components_take_precedence_over_MEF_registered_components_if_querying_for_a_single_component_registered_in_both_containers()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var typeCatalog = new TypeCatalog(typeof (Singleton));

            // Register catalog and types
            unityContainer.RegisterCatalog(typeCatalog);
            unityContainer.RegisterType<ISingleton, Singleton>(new ContainerControlledLifetimeManager());

            // Reset count
            Singleton.Count = 0;

            Assert.That(Singleton.Count, Is.EqualTo(0));
            var singleton = unityContainer.Resolve<ISingleton>();

            Assert.That(singleton, Is.Not.Null);
            Assert.That(Singleton.Count, Is.EqualTo(1));

            var mef = unityContainer.Resolve<CompositionContainer>();
            var mefSingleton = mef.GetExportedValue<ISingleton>();

            Assert.That(Singleton.Count, Is.EqualTo(1));
            Assert.That(singleton, Is.SameAs(mefSingleton));
        }

        [Test]
        public void When_querying_MEF_for_a_multiple_components_registered_in_both_containers_all_instances_are_returned()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var typeCatalog = new TypeCatalog(typeof(Singleton));

            // Register catalog and types
            unityContainer.RegisterCatalog(typeCatalog);
            unityContainer.RegisterType<ISingleton, Singleton>(new ContainerControlledLifetimeManager());

            // Reset count
            Singleton.Count = 0;

            Assert.That(Singleton.Count, Is.EqualTo(0));

            var mef = unityContainer.Resolve<CompositionContainer>();
            mef.GetExportedValues<ISingleton>();
            Assert.That(Singleton.Count, Is.EqualTo(2));
        }

        
    }

    [Export(typeof(ISingleton))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class Singleton : ISingleton
    {
        public static int Count;

        public Singleton()
        {
            Count++;
        }
    }

    public interface ISingleton { }
}