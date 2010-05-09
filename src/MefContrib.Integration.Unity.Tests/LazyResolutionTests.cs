using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace MefContrib.Integration.Unity.Tests
{
    [TestFixture]
    public class LazyResolutionTests
    {
        #region Fake components

        public interface IMixedComponent { }

        public class MixedComponent1 : IMixedComponent
        {
            public static int InstanceCount;

            public MixedComponent1()
            {
                InstanceCount++;
            }
        }

        public class MixedComponent2 : IMixedComponent
        {
            public static int InstanceCount;

            public MixedComponent2()
            {
                InstanceCount++;
            }
        }

        public class MixedComponent3 : IMixedComponent { }

        [Export(typeof(IMixedComponent))]
        public class MixedComponent4 : IMixedComponent { }

        [Export(typeof(IMixedComponent))]
        public class MixedComponent5 : IMixedComponent
        {
            public static int InstanceCount;

            public MixedComponent5()
            {
                InstanceCount++;
            }
        }

        #endregion

        [Test]
        public void UnityCanResolveLazyTypeRegisteredInMefTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddExtension(new CompositionIntegration(false));
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            var lazyMefComponent = unityContainer.Resolve<Lazy<IMefComponent>>();
            Assert.That(lazyMefComponent, Is.Not.Null);
            Assert.That(lazyMefComponent.Value, Is.Not.Null);
            Assert.That(lazyMefComponent.Value.GetType(), Is.EqualTo(typeof(MefComponent1)));
        }

        [Test]
        public void UnityCanResolveLazyTypeRegisteredInUnityTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddExtension(new CompositionIntegration(false));
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            UnityComponent1.InstanceCount = 0;
            unityContainer.RegisterType<IUnityComponent, UnityComponent1>();

            var lazyUnityComponent = unityContainer.Resolve<Lazy<IUnityComponent>>();
            Assert.That(lazyUnityComponent, Is.Not.Null);
            Assert.That(UnityComponent1.InstanceCount, Is.EqualTo(0));

            Assert.That(lazyUnityComponent.Value, Is.Not.Null);
            Assert.That(lazyUnityComponent.Value.GetType(), Is.EqualTo(typeof(UnityComponent1)));
            Assert.That(UnityComponent1.InstanceCount, Is.EqualTo(1));
        }

        [Test]
        public void UnityCanResolveLazyEnumerableOfTypesRegisteredInUnityTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddExtension(new CompositionIntegration(false));
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            UnityComponent1.InstanceCount = 0;
            
            unityContainer.RegisterType<IUnityComponent, UnityComponent1>("component1");
            unityContainer.RegisterType<IUnityComponent, UnityComponent2>("component2");

            var collectionOfLazyUnityComponents = unityContainer.Resolve<Lazy<IEnumerable<IUnityComponent>>>();
            Assert.That(collectionOfLazyUnityComponents, Is.Not.Null);

            Assert.That(UnityComponent1.InstanceCount, Is.EqualTo(0));
            var list = new List<IUnityComponent>(collectionOfLazyUnityComponents.Value);
            Assert.That(UnityComponent1.InstanceCount, Is.EqualTo(1));
            Assert.That(list.Count, Is.EqualTo(2));
        }

        [Test]
        public void UnityCanResolveEnumerableOfLazyTypesRegisteredInUnityAndMefTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            MixedComponent1.InstanceCount = 0;
            MixedComponent2.InstanceCount = 0;
            MixedComponent5.InstanceCount = 0;

            // Add composition support for unity
            unityContainer.AddExtension(new CompositionIntegration(true));
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            unityContainer.RegisterType<IMixedComponent, MixedComponent1>("component1");
            unityContainer.RegisterType<IMixedComponent, MixedComponent2>("component2");
            unityContainer.RegisterType<IMixedComponent, MixedComponent3>();

            var collectionOfLazyUnityComponents = unityContainer.Resolve<IEnumerable<Lazy<IMixedComponent>>>();
            Assert.That(collectionOfLazyUnityComponents, Is.Not.Null);

            Assert.That(MixedComponent1.InstanceCount, Is.EqualTo(0));
            Assert.That(MixedComponent2.InstanceCount, Is.EqualTo(0));
            Assert.That(MixedComponent5.InstanceCount, Is.EqualTo(0));

            var list = new List<Lazy<IMixedComponent>>(collectionOfLazyUnityComponents);

            Assert.That(MixedComponent1.InstanceCount, Is.EqualTo(0));
            Assert.That(MixedComponent2.InstanceCount, Is.EqualTo(0));
            Assert.That(MixedComponent5.InstanceCount, Is.EqualTo(0));

            Assert.That(list[0].Value, Is.Not.Null);
            Assert.That(list[1].Value, Is.Not.Null);
            Assert.That(list[2].Value, Is.Not.Null);
            Assert.That(list[3].Value, Is.Not.Null);
            Assert.That(list[4].Value, Is.Not.Null);

            Assert.That(MixedComponent1.InstanceCount, Is.EqualTo(1));
            Assert.That(MixedComponent2.InstanceCount, Is.EqualTo(1));
            Assert.That(MixedComponent5.InstanceCount, Is.EqualTo(1));

            Assert.That(list.Count, Is.EqualTo(5));
        }

        [Test]
        public void UnityCanResolveEnumerableOfTypesRegisteredInUnityAndMefTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            MixedComponent1.InstanceCount = 0;
            MixedComponent2.InstanceCount = 0;
            MixedComponent5.InstanceCount = 0;

            // Add composition support for unity
            unityContainer.AddExtension(new CompositionIntegration(true));
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            unityContainer.RegisterType<IMixedComponent, MixedComponent1>("component1");
            unityContainer.RegisterType<IMixedComponent, MixedComponent2>("component2");
            unityContainer.RegisterType<IMixedComponent, MixedComponent3>();

            var collectionOfLazyUnityComponents = unityContainer.Resolve<IEnumerable<IMixedComponent>>();
            Assert.That(collectionOfLazyUnityComponents, Is.Not.Null);

            Assert.That(MixedComponent1.InstanceCount, Is.EqualTo(1));
            Assert.That(MixedComponent2.InstanceCount, Is.EqualTo(1));
            Assert.That(MixedComponent5.InstanceCount, Is.EqualTo(1));

            var list = new List<IMixedComponent>(collectionOfLazyUnityComponents);
            Assert.That(list.Count, Is.EqualTo(5));
        }

        [Test]
        public void UnityCanResolveEnumerableOfLazyTypesRegisteredInUnityTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddExtension(new CompositionIntegration(true));
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            UnityComponent1.InstanceCount = 0;
            unityContainer.RegisterType<IUnityComponent, UnityComponent1>();
            unityContainer.RegisterType<IUnityComponent, UnityComponent2>("component2");

            var collectionOfLazyUnityComponents = unityContainer.Resolve<IEnumerable<Lazy<IUnityComponent>>>();
            Assert.That(collectionOfLazyUnityComponents, Is.Not.Null);

            Assert.That(UnityComponent1.InstanceCount, Is.EqualTo(0));
            var list = new List<Lazy<IUnityComponent>>(collectionOfLazyUnityComponents);
            Assert.That(UnityComponent1.InstanceCount, Is.EqualTo(0));
            Assert.That(list[0].Value, Is.Not.Null);
            Assert.That(list[1].Value, Is.Not.Null);
            Assert.That(UnityComponent1.InstanceCount, Is.EqualTo(1));
            Assert.That(list.Count, Is.EqualTo(2));
        }

        [Test]
        public void UnityCanResolveEnumerableOfTypesRegisteredInUnityTest()
        {
            // Setup
            var unityContainer = new UnityContainer();
            var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            // Add composition support for unity
            unityContainer.AddExtension(new CompositionIntegration(true));
            unityContainer.Configure<CompositionIntegration>().Catalogs.Add(assemblyCatalog);

            UnityComponent1.InstanceCount = 0;
            unityContainer.RegisterType<IUnityComponent, UnityComponent1>();
            unityContainer.RegisterType<IUnityComponent, UnityComponent2>("component2");

            var collectionOfLazyUnityComponents = unityContainer.Resolve<IEnumerable<IUnityComponent>>();
            Assert.That(collectionOfLazyUnityComponents, Is.Not.Null);
            Assert.That(UnityComponent1.InstanceCount, Is.EqualTo(1));

            var list = new List<IUnityComponent>(collectionOfLazyUnityComponents);
            Assert.That(list.Count, Is.EqualTo(2));
        }
    }
}